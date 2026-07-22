using PrintHub.Application.Common;
using PrintHub.Application.Common.Interfaces;
using PrintHub.Application.Common.Models;
using PrintHub.Application.Features.Reports.Dtos;
using PrintHub.Application.Specifications.Reports;
using PrintHub.Domain.Entities;
using PrintHub.Domain.Enums;

namespace PrintHub.Application.Features.Reports;

/// <summary>
/// Computes reports from completed orders. Aggregation is done in memory over the
/// filtered order set (the Application layer stays free of EF), which is fine at
/// the scale of this platform. Shop revenue is owner-scoped; the platform report is
/// admin-only (enforced at the controller).
/// </summary>
public class ReportService : IReportService
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUser _currentUser;

    public ReportService(IUnitOfWork uow, ICurrentUser currentUser)
    {
        _uow = uow;
        _currentUser = currentUser;
    }

    public async Task<Result<ShopRevenueReportDto>> GetShopRevenueAsync(int shopId, DateTime? from, DateTime? to, CancellationToken ct = default)
    {
        if (!_currentUser.CanManageShop(shopId))
            return Result<ShopRevenueReportDto>.Forbidden("You do not have permission to view this shop's reports.");

        var shop = await _uow.Repository<Shop>().GetByIdAsync(shopId, ct);
        if (shop is null)
            return Result<ShopRevenueReportDto>.NotFound("Shop not found.");

        var orders = await _uow.Repository<Order>().ListAsync(new CompletedOrdersByShopSpecification(shopId, from, to), ct);

        var gross = orders.Sum(o => o.TotalAmount);
        var commission = orders.Sum(o => o.CommissionAmount);
        var count = orders.Count;

        return Result.Success(new ShopRevenueReportDto
        {
            ShopId = shopId,
            ShopName = shop.Name,
            FromUtc = from,
            ToUtc = to,
            CompletedOrders = count,
            GrossRevenue = gross,
            CommissionTotal = commission,
            NetRevenue = gross - commission,
            AverageOrderValue = count > 0 ? Math.Round(gross / count, 2) : 0m
        });
    }

    public async Task<Result<PlatformReportDto>> GetPlatformAsync(DateTime? from, DateTime? to, CancellationToken ct = default)
    {
        var orders = await _uow.Repository<Order>().ListAsync(new CompletedOrdersInRangeSpecification(from, to), ct);
        var shops = await _uow.Repository<Shop>().ListAllAsync(ct);

        var top = orders
            .GroupBy(o => o.ShopId)
            .Select(g => new { Name = g.First().Shop.Name, Revenue = g.Sum(o => o.TotalAmount) })
            .OrderByDescending(x => x.Revenue)
            .FirstOrDefault();

        return Result.Success(new PlatformReportDto
        {
            FromUtc = from,
            ToUtc = to,
            TotalShops = shops.Count,
            ActiveShops = shops.Count(s => s.Status == ShopStatus.Active),
            CompletedOrders = orders.Count,
            Gmv = orders.Sum(o => o.TotalAmount),
            CommissionEarned = orders.Sum(o => o.CommissionAmount),
            TopShopName = top?.Name,
            TopShopRevenue = top?.Revenue ?? 0m
        });
    }
}
