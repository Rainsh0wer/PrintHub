using System.Text.Json;
using PrintHub.Application.Common.Interfaces;
using PrintHub.Application.Common.Models;
using PrintHub.Application.Features.Quotes.Dtos;
using PrintHub.Application.Specifications.ServiceTypes;
using PrintHub.Application.Specifications.Shops;
using PrintHub.Domain.Entities;
using PrintHub.Domain.Enums;

namespace PrintHub.Application.Features.Quotes;

/// <summary>
/// Computes comparable quotes across eligible shops. Eligibility (offers every
/// requested service, has a non-offline machine for each group) is decided here;
/// pricing is delegated to the gRPC Quote Engine per shop. If the engine is
/// unavailable the service falls back to an indicative in-process price and marks
/// the quote accordingly (graceful degradation).
/// </summary>
public class QuoteService : IQuoteService
{
    private static readonly TimeSpan QuoteValidity = TimeSpan.FromHours(24);

    private readonly IUnitOfWork _uow;
    private readonly IQuoteEngineClient _engine;

    public QuoteService(IUnitOfWork uow, IQuoteEngineClient engine)
    {
        _uow = uow;
        _engine = engine;
    }

    public async Task<Result<IReadOnlyList<QuoteComparisonDto>>> CompareAsync(int customerId, CompareQuotesRequest request, CancellationToken ct = default)
    {
        var typeIds = request.Items.Select(i => i.ServiceTypeId).Distinct().ToArray();
        var serviceTypes = (await _uow.Repository<ServiceType>().ListAsync(new ServiceTypesByIdsSpecification(typeIds), ct))
            .ToDictionary(t => t.Id);

        if (serviceTypes.Count != typeIds.Length)
            return Result<IReadOnlyList<QuoteComparisonDto>>.Fail("One or more selected services are not available.");

        var requiredGroups = serviceTypes.Values.Select(t => t.ServiceGroup).Distinct().ToArray();

        var shops = await _uow.Repository<Shop>().ListAsync(new ActiveShopsWithRateCardSpecification(request.ShopId), ct);
        var eligible = shops.Where(s => IsEligible(s, typeIds, requiredGroups)).ToList();

        var quotes = new List<(Quote quote, Shop shop, QuoteEstimate estimate, bool indicative)>();

        foreach (var shop in eligible)
        {
            var (estimate, indicative) = await EstimateForShopAsync(shop, request.Items, serviceTypes, ct);

            var quote = new Quote
            {
                CustomerId = customerId,
                ShopId = shop.Id,
                SubTotal = estimate.Subtotal,
                EstimatedMinutes = estimate.EstimatedMinutes,
                BreakdownJson = JsonSerializer.Serialize(estimate.Lines),
                IsIndicative = indicative,
                DistanceMeters = null,   // routing service integration is deferred
                ExpiresAt = DateTime.UtcNow.Add(QuoteValidity),
                CreatedAt = DateTime.UtcNow
            };
            await _uow.Repository<Quote>().AddAsync(quote, ct);
            quotes.Add((quote, shop, estimate, indicative));
        }

        await _uow.SaveChangesAsync(ct);   // assigns quote ids

        var results = quotes.Select(q => new QuoteComparisonDto(
            q.quote.Id, q.shop.Id, q.shop.Name, q.shop.District,
            q.shop.RatingAverage, q.shop.RatingCount,
            q.estimate.Subtotal, q.estimate.EstimatedMinutes, null, q.indicative,
            q.estimate.Lines.Select(l => new QuoteLineDto(l.Description, l.EffectiveUnitPrice, l.LineTotal, l.Minutes, l.AppliedRules))));

        results = request.SortBy switch
        {
            QuoteSortBy.Time => results.OrderBy(r => r.EstimatedMinutes),
            QuoteSortBy.Rating => results.OrderByDescending(r => r.Rating),
            _ => results.OrderBy(r => r.Total)
        };

        return Result.Success<IReadOnlyList<QuoteComparisonDto>>(results.ToList());
    }

    private static bool IsEligible(Shop shop, IReadOnlyCollection<int> typeIds, IReadOnlyCollection<ServiceGroup> groups)
    {
        var offered = shop.Services.Where(s => s.IsActive).Select(s => s.ServiceTypeId).ToHashSet();
        if (!typeIds.All(offered.Contains)) return false;

        // A non-offline machine must exist for each required service group.
        return groups.All(g => shop.Machines.Any(m => m.ServiceGroup == g && m.Status != MachineStatus.Offline));
    }

    private async Task<(QuoteEstimate estimate, bool indicative)> EstimateForShopAsync(
        Shop shop, IReadOnlyList<CompareItemInput> items, IReadOnlyDictionary<int, ServiceType> serviceTypes, CancellationToken ct)
    {
        var inputs = new List<QuoteItemInput>();
        foreach (var item in items)
        {
            var rateEntry = shop.Services.First(s => s.ServiceTypeId == item.ServiceTypeId && s.IsActive);
            var type = serviceTypes[item.ServiceTypeId];

            inputs.Add(new QuoteItemInput(
                PricingModel: type.PricingModel.ToString(),
                Quantity: item.Quantity,
                PageCount: item.PageCount ?? 1,
                EstimatedGrams: (double)(item.EstimatedGrams ?? 0),
                UnitPrice: rateEntry.UnitPrice,
                SetupFee: rateEntry.SetupFee,
                LeadTimeMinutes: rateEntry.LeadTimeMinutes,
                SelectedOptions: BuildOptions(item),
                Rules: rateEntry.PriceRules.Where(r => r.IsActive).Select(r => new QuotePriceRuleInput(
                    r.RuleType.ToString(), r.OptionKey, r.Multiplier, r.FlatExtra, r.MinQuantity, r.MaxQuantity)).ToList()));
        }

        var estimate = await _engine.EstimateAsync(new QuoteEstimateInput(inputs), ct);
        if (estimate is not null)
            return (estimate, false);

        // Fallback: indicative price without the engine (no rules applied).
        return (BuildIndicative(inputs, serviceTypes, items), true);
    }

    private static List<string> BuildOptions(CompareItemInput item)
    {
        var options = new List<string>();
        if (!string.IsNullOrWhiteSpace(item.PaperType)) options.Add(item.PaperType);
        if (item.ColorMode.HasValue) options.Add(item.ColorMode.Value.ToString());
        if (item.Sides.HasValue) options.Add(item.Sides.Value.ToString());
        if (!string.IsNullOrWhiteSpace(item.BindingType)) options.Add(item.BindingType);
        if (!string.IsNullOrWhiteSpace(item.MaterialName)) options.Add(item.MaterialName);
        if (!string.IsNullOrWhiteSpace(item.QualityProfile)) options.Add(item.QualityProfile);
        return options;
    }

    private static QuoteEstimate BuildIndicative(
        IReadOnlyList<QuoteItemInput> inputs, IReadOnlyDictionary<int, ServiceType> serviceTypes, IReadOnlyList<CompareItemInput> items)
    {
        decimal subtotal = 0;
        var minutes = 0;
        var lines = new List<QuoteLine>();

        for (var i = 0; i < inputs.Count; i++)
        {
            var input = inputs[i];
            var size = SizeDriver(input);
            var lineTotal = input.UnitPrice * (decimal)size + input.SetupFee;
            var lineMinutes = (int)Math.Ceiling(input.LeadTimeMinutes * size);
            subtotal += lineTotal;
            minutes += lineMinutes;
            lines.Add(new QuoteLine($"Indicative ({input.PricingModel})", input.UnitPrice, lineTotal, lineMinutes, Array.Empty<string>()));
        }

        return new QuoteEstimate(subtotal, minutes, lines);
    }

    private static double SizeDriver(QuoteItemInput input) => input.PricingModel switch
    {
        nameof(PricingModel.PerPage) => Math.Max(1, input.PageCount) * Math.Max(1, input.Quantity),
        nameof(PricingModel.MaterialAndTime) => input.EstimatedGrams,
        _ => Math.Max(1, input.Quantity)
    };
}
