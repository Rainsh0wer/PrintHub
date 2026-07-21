using PrintHub.Application.Common.Models;
using PrintHub.Application.Features.Quotes.Dtos;

namespace PrintHub.Application.Features.Quotes;

/// <summary>
/// The platform's distinguishing capability (UC-13): configure an order once and
/// compare quotes across every eligible shop, computed by the gRPC Quote Engine.
/// </summary>
public interface IQuoteService
{
    Task<Result<IReadOnlyList<QuoteComparisonDto>>> CompareAsync(int customerId, CompareQuotesRequest request, CancellationToken ct = default);
}
