using PrintHub.Application.Common.Models;

namespace PrintHub.Application.Features.Wallet.Dtos;

/// <summary>UC-21 — a customer requests a wallet top-up of a given amount (VND).</summary>
public record TopUpRequest(decimal Amount);

/// <summary>
/// The payment instructions returned for a pending top-up: a bank transfer whose
/// content is the reference code, plus a VietQR image the app can render. The
/// balance only changes once the transfer is matched and confirmed.
/// </summary>
public record TopUpResponse(
    string RefCode,
    decimal Amount,
    string BankName,
    string AccountNumber,
    string AccountName,
    string TransferContent,
    string QrImageUrl);

/// <summary>Admin supplies the bank's own reference when confirming a matched transfer.</summary>
public record ConfirmTopUpRequest(string? BankReference);

public record WalletTransactionDto(
    int Id,
    string Type,
    decimal Amount,
    decimal BalanceAfter,
    string Status,
    string RefCode,
    int? OrderId,
    string? Description,
    DateTime CreatedAt,
    DateTime? ConfirmedAt);

/// <summary>UC-22 — current balance plus a paged, newest-first transaction ledger.</summary>
public record WalletLedgerDto(
    decimal Balance,
    PagedResult<WalletTransactionDto> Transactions);
