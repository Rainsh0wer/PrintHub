using Grpc.Core;
using PrintHub.Contracts.Quoting;
using PrintHub.QuoteEngine.Pricing;

namespace PrintHub.QuoteEngine.Services;

/// <summary>
/// The gRPC Quote Engine. A stateless calculator: for each item it selects the
/// pricing strategy for the item's model and sums the line totals and estimated
/// minutes. Pricing logic lives here, isolated from the ordering system.
/// </summary>
public class QuoteEstimatorService : QuoteEstimator.QuoteEstimatorBase
{
    private readonly IReadOnlyDictionary<string, IPricingStrategy> _strategies;
    private readonly ILogger<QuoteEstimatorService> _logger;

    public QuoteEstimatorService(IEnumerable<IPricingStrategy> strategies, ILogger<QuoteEstimatorService> logger)
    {
        _strategies = strategies.ToDictionary(s => s.PricingModel);
        _logger = logger;
    }

    public override Task<EstimateResponse> Estimate(EstimateRequest request, ServerCallContext context)
    {
        var response = new EstimateResponse();
        double subtotal = 0;
        var minutes = 0;

        foreach (var item in request.Items)
        {
            if (!_strategies.TryGetValue(item.PricingModel, out var strategy))
                throw new RpcException(new Status(StatusCode.InvalidArgument,
                    $"Unknown pricing model '{item.PricingModel}'."));

            var line = strategy.Calculate(item);
            subtotal += line.LineTotal;
            minutes += line.Minutes;
            response.Lines.Add(line);
        }

        response.Subtotal = Math.Round(subtotal, 2);
        response.EstimatedMinutes = minutes;

        _logger.LogInformation("Estimated {Items} item(s): subtotal={Subtotal}, minutes={Minutes}",
            request.Items.Count, response.Subtotal, response.EstimatedMinutes);

        return Task.FromResult(response);
    }
}
