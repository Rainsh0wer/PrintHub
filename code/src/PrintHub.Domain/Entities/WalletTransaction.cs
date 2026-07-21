using PrintHub.Domain.Common;
using PrintHub.Domain.Enums;

namespace PrintHub.Domain.Entities;

/// <summary>
/// A ledger entry against a user's wallet. Every entry records the resulting
/// balance, so the ledger can be verified independently against the stored
/// balance. RefCode is unique and used to match a bank transfer for top-ups.
/// </summary>
public class WalletTransaction : BaseEntity
{
    public int UserId { get; set; }
    public int? OrderId { get; set; }

    public WalletTransactionType Type { get; set; }

    /// <summary>Signed amount in VND; positive credits, negative debits.</summary>
    public decimal Amount { get; set; }

    /// <summary>Resulting balance after this transaction was applied.</summary>
    public decimal BalanceAfter { get; set; }

    /// <summary>Unique reference code used to match a bank transfer.</summary>
    public string RefCode { get; set; } = null!;

    public WalletTransactionStatus Status { get; set; } = WalletTransactionStatus.Pending;

    public int? ConfirmedBy { get; set; }

    public string? Description { get; set; }
    /// <summary>The bank's own transaction reference for a confirmed top-up.</summary>
    public string? BankReference { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? ConfirmedAt { get; set; }

    // Navigation
    public User User { get; set; } = null!;
    public Order? Order { get; set; }
}
