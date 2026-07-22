using AutoMapper;
using PrintHub.Application.Common;
using PrintHub.Application.Common.Interfaces;
using PrintHub.Application.Common.Models;
using PrintHub.Application.Features.Complaints.Dtos;
using PrintHub.Application.Specifications.Complaints;
using PrintHub.Domain.Entities;
using PrintHub.Domain.Enums;

namespace PrintHub.Application.Features.Complaints;

/// <summary>
/// Implements the complaint workflow. Customer-side steps (raise/accept/escalate)
/// are scoped to the complaint owner; shop response is scoped to the shop; final
/// adjudication is admin-only (enforced at the controller). Refund resolutions —
/// whether accepted from the shop or upheld by an administrator — credit the
/// customer's wallet with a Refund ledger entry.
/// </summary>
public class ComplaintService : IComplaintService
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUser _currentUser;
    private readonly IMapper _mapper;

    public ComplaintService(IUnitOfWork uow, ICurrentUser currentUser, IMapper mapper)
    {
        _uow = uow;
        _currentUser = currentUser;
        _mapper = mapper;
    }

    public async Task<Result<ComplaintDto>> RaiseAsync(int customerId, RaiseComplaintRequest request, CancellationToken ct = default)
    {
        var order = await _uow.Repository<Order>().GetByIdAsync(request.OrderId, ct);
        if (order is null || order.CustomerId != customerId)
            return Result<ComplaintDto>.NotFound("Order not found.");
        if (order.Status != OrderStatus.Completed)
            return Result<ComplaintDto>.Conflict("A complaint can only be raised on a completed order.");

        var complaints = _uow.Repository<Complaint>();
        if (await complaints.AnyAsync(new ActiveComplaintByOrderSpecification(request.OrderId), ct))
            return Result<ComplaintDto>.Conflict("There is already an open complaint for this order.");

        var complaint = new Complaint
        {
            OrderId = order.Id,
            CustomerId = customerId,
            ShopId = order.ShopId,
            Reason = request.Reason,
            Description = request.Description,
            AttachmentUrls = request.AttachmentUrls,
            Status = ComplaintStatus.Open
        };
        await complaints.AddAsync(complaint, ct);
        await _uow.SaveChangesAsync(ct);

        return Result.Success(await LoadDetailAsync(complaint.Id, ct));
    }

    public async Task<Result<PagedResult<ComplaintDto>>> ListMineAsync(int customerId, PageRequest page, CancellationToken ct = default)
    {
        var repo = _uow.Repository<Complaint>();
        var total = await repo.CountAsync(new ComplaintsByCustomerCountSpecification(customerId), ct);
        var items = await repo.ListAsync(new ComplaintsByCustomerSpecification(customerId, page.Skip, page.Take), ct);
        var mapped = _mapper.Map<IReadOnlyList<ComplaintDto>>(items);
        return Result.Success(new PagedResult<ComplaintDto>(mapped, total, page.PageNumber, page.PageSize));
    }

    public async Task<Result<ComplaintDto>> RespondAsync(int complaintId, RespondComplaintRequest request, CancellationToken ct = default)
    {
        var complaint = await _uow.Repository<Complaint>().FirstOrDefaultAsync(new ComplaintByIdSpecification(complaintId), ct);
        if (complaint is null)
            return Result<ComplaintDto>.NotFound("Complaint not found.");
        if (!_currentUser.CanOperateShop(complaint.ShopId))
            return Result<ComplaintDto>.Forbidden("You do not have permission to respond to this complaint.");
        if (complaint.Status != ComplaintStatus.Open)
            return Result<ComplaintDto>.Conflict($"A complaint in status '{complaint.Status}' can no longer be responded to.");

        if (request.ProposedResolution == ComplaintResolution.Refund)
        {
            var refund = request.RefundAmount ?? 0m;
            if (refund <= 0 || refund > complaint.Order.TotalAmount)
                return Result<ComplaintDto>.Fail("The refund amount must be greater than zero and no more than the order total.");
            complaint.RefundAmount = refund;
        }

        complaint.ProposedResolution = request.ProposedResolution;
        complaint.ShopResponse = request.ShopResponse;
        complaint.Status = ComplaintStatus.ShopResponded;
        complaint.RespondedAt = DateTime.UtcNow;
        _uow.Repository<Complaint>().Update(complaint);
        await _uow.SaveChangesAsync(ct);

        return Result.Success(await LoadDetailAsync(complaint.Id, ct));
    }

    public async Task<Result<ComplaintDto>> AcceptAsync(int customerId, int complaintId, CancellationToken ct = default)
    {
        var complaint = await _uow.Repository<Complaint>().FirstOrDefaultAsync(new ComplaintByIdSpecification(complaintId), ct);
        if (complaint is null || complaint.CustomerId != customerId)
            return Result<ComplaintDto>.NotFound("Complaint not found.");
        if (complaint.Status != ComplaintStatus.ShopResponded)
            return Result<ComplaintDto>.Conflict("There is no proposed resolution to accept.");

        if (complaint.ProposedResolution == ComplaintResolution.Refund && complaint.RefundAmount is > 0)
            await RefundWalletAsync(customerId, complaint.Order, complaint.RefundAmount.Value, ct);

        complaint.Status = ComplaintStatus.Resolved;
        complaint.ResolvedAt = DateTime.UtcNow;
        complaint.ResolvedBy = customerId;
        _uow.Repository<Complaint>().Update(complaint);
        await _uow.SaveChangesAsync(ct);

        return Result.Success(await LoadDetailAsync(complaint.Id, ct));
    }

    public async Task<Result<ComplaintDto>> EscalateAsync(int customerId, int complaintId, CancellationToken ct = default)
    {
        var complaint = await _uow.Repository<Complaint>().FirstOrDefaultAsync(new ComplaintByIdSpecification(complaintId), ct);
        if (complaint is null || complaint.CustomerId != customerId)
            return Result<ComplaintDto>.NotFound("Complaint not found.");
        if (complaint.Status is not (ComplaintStatus.Open or ComplaintStatus.ShopResponded))
            return Result<ComplaintDto>.Conflict($"A complaint in status '{complaint.Status}' cannot be escalated.");

        complaint.Status = ComplaintStatus.Escalated;
        complaint.EscalatedAt = DateTime.UtcNow;
        _uow.Repository<Complaint>().Update(complaint);
        await _uow.SaveChangesAsync(ct);

        return Result.Success(await LoadDetailAsync(complaint.Id, ct));
    }

    public async Task<Result<PagedResult<ComplaintDto>>> ListEscalatedAsync(PageRequest page, CancellationToken ct = default)
    {
        var repo = _uow.Repository<Complaint>();
        var total = await repo.CountAsync(new EscalatedComplaintsCountSpecification(), ct);
        var items = await repo.ListAsync(new EscalatedComplaintsSpecification(page.Skip, page.Take), ct);
        var mapped = _mapper.Map<IReadOnlyList<ComplaintDto>>(items);
        return Result.Success(new PagedResult<ComplaintDto>(mapped, total, page.PageNumber, page.PageSize));
    }

    public async Task<Result<ComplaintDto>> AdjudicateAsync(int adminUserId, int complaintId, AdjudicateComplaintRequest request, CancellationToken ct = default)
    {
        var complaint = await _uow.Repository<Complaint>().FirstOrDefaultAsync(new ComplaintByIdSpecification(complaintId), ct);
        if (complaint is null)
            return Result<ComplaintDto>.NotFound("Complaint not found.");
        if (complaint.Status != ComplaintStatus.Escalated)
            return Result<ComplaintDto>.Conflict("Only an escalated complaint can be adjudicated.");

        if (request.UpholdRefund)
        {
            var refund = request.RefundAmount ?? complaint.RefundAmount ?? 0m;
            if (refund <= 0 || refund > complaint.Order.TotalAmount)
                return Result<ComplaintDto>.Fail("The refund amount must be greater than zero and no more than the order total.");
            complaint.RefundAmount = refund;
            complaint.ProposedResolution = ComplaintResolution.Refund;
            await RefundWalletAsync(complaint.CustomerId, complaint.Order, refund, ct);
        }
        else
        {
            complaint.ProposedResolution = ComplaintResolution.Rejected;
        }

        complaint.AdminRuling = request.AdminRuling;
        complaint.Status = ComplaintStatus.Closed;
        complaint.ClosedAt = DateTime.UtcNow;
        complaint.ResolvedBy = adminUserId;
        _uow.Repository<Complaint>().Update(complaint);
        await _uow.SaveChangesAsync(ct);

        return Result.Success(await LoadDetailAsync(complaint.Id, ct));
    }

    // ---------------------------------------------------------------- helpers

    private async Task RefundWalletAsync(int userId, Order order, decimal amount, CancellationToken ct)
    {
        var user = await _uow.Repository<User>().GetByIdAsync(userId, ct);
        if (user is null) return;

        user.WalletBalance += amount;
        _uow.Repository<User>().Update(user);
        await _uow.Repository<WalletTransaction>().AddAsync(new WalletTransaction
        {
            UserId = userId,
            OrderId = order.Id,
            Type = WalletTransactionType.Refund,
            Amount = amount,
            BalanceAfter = user.WalletBalance,
            Status = WalletTransactionStatus.Completed,
            RefCode = $"CMP-{order.OrderCode}-{Random.Shared.Next(1000, 9999)}",
            Description = $"Complaint refund for order {order.OrderCode}",
            CreatedAt = DateTime.UtcNow
        }, ct);
    }

    private async Task<ComplaintDto> LoadDetailAsync(int complaintId, CancellationToken ct)
    {
        var complaint = await _uow.Repository<Complaint>().FirstOrDefaultAsync(new ComplaintWithDetailsByIdSpecification(complaintId), ct);
        return _mapper.Map<ComplaintDto>(complaint!);
    }
}
