using AutoMapper;
using PrintHub.Application.Common.Interfaces;
using PrintHub.Application.Common.Models;
using PrintHub.Application.Features.Wallet.Dtos;
using PrintHub.Application.Specifications.Wallet;
using PrintHub.Domain.Entities;
using PrintHub.Domain.Enums;

namespace PrintHub.Application.Features.Wallet;

/// <summary>
/// Wallet balance and top-ups. A top-up is a two-step flow: the customer requests
/// it (a Pending ledger row + transfer instructions), then an administrator
/// confirms the matched bank transfer, which is the only point the balance changes.
/// This keeps the ledger and the stored balance reconcilable.
/// </summary>
public class WalletService : IWalletService
{
    // Demo receiving account (would move to configuration for a real deployment).
    private const string BankName = "MB Bank";
    private const string BankBin = "970422";
    private const string AccountNumber = "0901234567";
    private const string AccountName = "PRINTHUB JSC";

    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public WalletService(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<Result<WalletLedgerDto>> GetLedgerAsync(int userId, PageRequest page, CancellationToken ct = default)
    {
        var user = await _uow.Repository<User>().GetByIdAsync(userId, ct);
        if (user is null)
            return Result<WalletLedgerDto>.NotFound("User not found.");

        var repo = _uow.Repository<WalletTransaction>();
        var total = await repo.CountAsync(new WalletTransactionsByUserCountSpecification(userId), ct);
        var txns = await repo.ListAsync(new WalletTransactionsByUserSpecification(userId, page.Skip, page.Take), ct);

        var items = _mapper.Map<IReadOnlyList<WalletTransactionDto>>(txns);
        var paged = new PagedResult<WalletTransactionDto>(items, total, page.PageNumber, page.PageSize);
        return Result.Success(new WalletLedgerDto(user.WalletBalance, paged));
    }

    public async Task<Result<TopUpResponse>> RequestTopUpAsync(int userId, TopUpRequest request, CancellationToken ct = default)
    {
        if (request.Amount <= 0)
            return Result<TopUpResponse>.Fail("Top-up amount must be greater than zero.");

        var user = await _uow.Repository<User>().GetByIdAsync(userId, ct);
        if (user is null)
            return Result<TopUpResponse>.NotFound("User not found.");

        var refCode = $"TOP{DateTime.UtcNow:yyMMddHHmmss}{Random.Shared.Next(100, 999)}";

        await _uow.Repository<WalletTransaction>().AddAsync(new WalletTransaction
        {
            UserId = userId,
            Type = WalletTransactionType.TopUp,
            Amount = request.Amount,
            BalanceAfter = user.WalletBalance,     // unchanged until the transfer is confirmed
            Status = WalletTransactionStatus.Pending,
            RefCode = refCode,
            Description = "Wallet top-up (awaiting bank transfer)",
            CreatedAt = DateTime.UtcNow
        }, ct);
        await _uow.SaveChangesAsync(ct);

        var qr = $"https://img.vietqr.io/image/{BankBin}-{AccountNumber}-compact2.png" +
                 $"?amount={request.Amount:0}&addInfo={refCode}&accountName={Uri.EscapeDataString(AccountName)}";

        return Result.Success(new TopUpResponse(
            refCode, request.Amount, BankName, AccountNumber, AccountName, refCode, qr));
    }

    public async Task<Result<WalletTransactionDto>> ConfirmTopUpAsync(
        int adminUserId, string refCode, ConfirmTopUpRequest request, CancellationToken ct = default)
    {
        var repo = _uow.Repository<WalletTransaction>();
        var txn = await repo.FirstOrDefaultAsync(new PendingTopUpByRefCodeSpecification(refCode), ct);
        if (txn is null)
            return Result<WalletTransactionDto>.NotFound("No pending top-up was found for that reference code.");

        var user = await _uow.Repository<User>().GetByIdAsync(txn.UserId, ct);
        if (user is null)
            return Result<WalletTransactionDto>.NotFound("The account for this top-up no longer exists.");

        user.WalletBalance += txn.Amount;
        _uow.Repository<User>().Update(user);

        txn.Status = WalletTransactionStatus.Completed;
        txn.BalanceAfter = user.WalletBalance;
        txn.ConfirmedBy = adminUserId;
        txn.ConfirmedAt = DateTime.UtcNow;
        txn.BankReference = request.BankReference;
        repo.Update(txn);

        await _uow.SaveChangesAsync(ct);
        return Result.Success(_mapper.Map<WalletTransactionDto>(txn));
    }
}
