using PrintHub.Application.Common.Specifications;
using PrintHub.Domain.Entities;
using PrintHub.Domain.Enums;

namespace PrintHub.Application.Specifications.Wallet;

/// <summary>A user's wallet ledger, newest first. Paged.</summary>
public sealed class WalletTransactionsByUserSpecification : BaseSpecification<WalletTransaction>
{
    public WalletTransactionsByUserSpecification(int userId, int skip, int take)
        : base(w => w.UserId == userId)
    {
        ApplyOrderByDescending(w => w.CreatedAt);
        ApplyPaging(skip, take);
    }
}

/// <summary>Count of a user's wallet transactions — the paging companion.</summary>
public sealed class WalletTransactionsByUserCountSpecification : BaseSpecification<WalletTransaction>
{
    public WalletTransactionsByUserCountSpecification(int userId)
        : base(w => w.UserId == userId)
    {
    }
}

/// <summary>A pending top-up matched by its unique reference code — for admin confirmation.</summary>
public sealed class PendingTopUpByRefCodeSpecification : BaseSpecification<WalletTransaction>
{
    public PendingTopUpByRefCodeSpecification(string refCode)
        : base(w => w.RefCode == refCode
                    && w.Type == WalletTransactionType.TopUp
                    && w.Status == WalletTransactionStatus.Pending)
    {
    }
}
