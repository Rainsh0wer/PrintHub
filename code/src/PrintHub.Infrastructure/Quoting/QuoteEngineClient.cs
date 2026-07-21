using Grpc.Core;
using Microsoft.Extensions.Logging;
using PrintHub.Application.Common.Interfaces;
using PrintHub.Contracts.Quoting;

namespace PrintHub.Infrastructure.Quoting;

/// <summary>
/// gRPC implementation of <see cref="IQuoteEngineClient"/>. Maps the Application
/// DTOs to protobuf, calls the Quote Engine, and maps the response back. On any
/// RPC failure it returns null so the caller degrades to an indicative price.
/// </summary>
public class QuoteEngineClient : IQuoteEngineClient
{
    private readonly QuoteEstimator.QuoteEstimatorClient _client;
    private readonly ILogger<QuoteEngineClient> _logger;

    public QuoteEngineClient(QuoteEstimator.QuoteEstimatorClient client, ILogger<QuoteEngineClient> logger)
    {
        _client = client;
        _logger = logger;
    }

    public async Task<QuoteEstimate?> EstimateAsync(QuoteEstimateInput input, CancellationToken ct = default)
    {
        var request = new EstimateRequest();
        foreach (var item in input.Items)
        {
            var protoItem = new EstimateItem
            {
                PricingModel = item.PricingModel,
                Quantity = item.Quantity,
                PageCount = item.PageCount,
                EstimatedGrams = item.EstimatedGrams,
                UnitPrice = (double)item.UnitPrice,
                SetupFee = (double)item.SetupFee,
                LeadTimeMinutes = item.LeadTimeMinutes
            };
            protoItem.SelectedOptions.AddRange(item.SelectedOptions);
            foreach (var rule in item.Rules)
            {
                protoItem.Rules.Add(new PricingRuleInput
                {
                    RuleType = rule.RuleType,
                    OptionKey = rule.OptionKey,
                    Multiplier = (double)rule.Multiplier,
                    FlatExtra = (double)rule.FlatExtra,
                    MinQuantity = rule.MinQuantity ?? 0,
                    MaxQuantity = rule.MaxQuantity ?? 0,
                    HasMin = rule.MinQuantity.HasValue,
                    HasMax = rule.MaxQuantity.HasValue
                });
            }
            request.Items.Add(protoItem);
        }

        try
        {
            var response = await _client.EstimateAsync(request, cancellationToken: ct);
            var lines = response.Lines.Select(l => new QuoteLine(
                l.Description,
                (decimal)l.EffectiveUnitPrice,
                (decimal)l.LineTotal,
                l.Minutes,
                l.AppliedRules.ToList())).ToList();

            return new QuoteEstimate((decimal)response.Subtotal, response.EstimatedMinutes, lines);
        }
        catch (RpcException ex)
        {
            _logger.LogWarning(ex, "Quote Engine unavailable ({Status}); falling back to indicative pricing.", ex.StatusCode);
            return null;
        }
    }
}
