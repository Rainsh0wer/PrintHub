using PrintHub.Application.Common.Models;
using PrintHub.Application.Features.Complaints.Dtos;

namespace PrintHub.Application.Features.Complaints;

/// <summary>
/// The complaint / resolution workflow (UC-24/35/41). A complaint flows
/// Open → ShopResponded → Resolved, or escalates to an administrator whose ruling
/// is final (→ Closed). Refund resolutions credit the customer's wallet.
/// </summary>
public interface IComplaintService
{
    /// <summary>UC-24 — raise a complaint on a completed order (→ Open).</summary>
    Task<Result<ComplaintDto>> RaiseAsync(int customerId, RaiseComplaintRequest request, CancellationToken ct = default);

    /// <summary>UC-24 — the caller's complaints and their status, newest first, paged.</summary>
    Task<Result<PagedResult<ComplaintDto>>> ListMineAsync(int customerId, PageRequest page, CancellationToken ct = default);

    /// <summary>UC-35 — the shop proposes a reprint or refund (Open → ShopResponded).</summary>
    Task<Result<ComplaintDto>> RespondAsync(int complaintId, RespondComplaintRequest request, CancellationToken ct = default);

    /// <summary>UC-24 — the customer accepts the proposed resolution (ShopResponded → Resolved).</summary>
    Task<Result<ComplaintDto>> AcceptAsync(int customerId, int complaintId, CancellationToken ct = default);

    /// <summary>UC-24 — the customer rejects and escalates (Open/ShopResponded → Escalated).</summary>
    Task<Result<ComplaintDto>> EscalateAsync(int customerId, int complaintId, CancellationToken ct = default);

    /// <summary>UC-41 — escalated complaints awaiting adjudication, oldest first, paged.</summary>
    Task<Result<PagedResult<ComplaintDto>>> ListEscalatedAsync(PageRequest page, CancellationToken ct = default);

    /// <summary>UC-41 — the administrator's final ruling (Escalated → Closed).</summary>
    Task<Result<ComplaintDto>> AdjudicateAsync(int adminUserId, int complaintId, AdjudicateComplaintRequest request, CancellationToken ct = default);
}
