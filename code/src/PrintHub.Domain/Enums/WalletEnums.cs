namespace PrintHub.Domain.Enums;

public enum WalletTransactionType
{
    TopUp = 0,
    Payment = 1,
    Refund = 2,
    Adjustment = 3
}

public enum WalletTransactionStatus
{
    Pending = 0,
    Completed = 1,
    Expired = 2,
    Failed = 3
}

public enum VoucherDiscountType
{
    Percent = 0,
    FixedAmount = 1
}
