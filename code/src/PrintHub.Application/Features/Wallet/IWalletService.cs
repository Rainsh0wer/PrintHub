using PrintHub.Application.Common.Models;
using PrintHub.Application.Features.Wallet.Dtos;

namespace PrintHub.Application.Features.Wallet;

/// <summary>
/// Wallet use cases: viewing the balance/ledger (UC-22), requesting a top-up
/// (UC-21, which stays pending until a bank transfer is matched), and the admin
/// confirmation that credits the balance against the recorded reference.
/// </summary>
public interface IWalletService
{
    /// <summary>UC-22 — the caller's balance and paged transaction ledger.</summary>
    Task<Result<WalletLedgerDto>> GetLedgerAsync(int userId, PageRequest page, CancellationToken ct = default);

    /// <summary>UC-21 — create a pending top-up and return bank-transfer instructions.</summary>
    Task<Result<TopUpResponse>> RequestTopUpAsync(int userId, TopUpRequest request, CancellationToken ct = default);

    /// <summary>Admin — confirm a matched transfer by reference code, crediting the wallet.</summary>
    Task<Result<WalletTransactionDto>> ConfirmTopUpAsync(int adminUserId, string refCode, ConfirmTopUpRequest request, CancellationToken ct = default);
}
