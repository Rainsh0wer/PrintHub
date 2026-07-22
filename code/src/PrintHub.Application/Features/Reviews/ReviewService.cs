using AutoMapper;
using PrintHub.Application.Common.Interfaces;
using PrintHub.Application.Common.Models;
using PrintHub.Application.Features.Reviews.Dtos;
using PrintHub.Application.Specifications.Reviews;
using PrintHub.Domain.Entities;
using PrintHub.Domain.Enums;

namespace PrintHub.Application.Features.Reviews;

/// <summary>
/// Review creation and listing. Only the customer who placed a completed order may
/// review it, and only once (unique per order). Posting a review folds the rating
/// into the shop's running average so the directory reflects it immediately.
/// </summary>
public class ReviewService : IReviewService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public ReviewService(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<Result<ReviewItemDto>> CreateAsync(int customerId, int orderId, CreateReviewRequest request, CancellationToken ct = default)
    {
        var order = await _uow.Repository<Order>().GetByIdAsync(orderId, ct);
        if (order is null || order.CustomerId != customerId)
            return Result<ReviewItemDto>.NotFound("Order not found.");
        if (order.Status != OrderStatus.Completed)
            return Result<ReviewItemDto>.Conflict("Only a completed order can be reviewed.");

        var reviews = _uow.Repository<Review>();
        if (await reviews.AnyAsync(new ReviewByOrderSpecification(orderId), ct))
            return Result<ReviewItemDto>.Conflict("This order has already been reviewed.");

        var review = new Review
        {
            OrderId = orderId,
            CustomerId = customerId,
            ShopId = order.ShopId,
            Rating = request.Rating,
            Comment = request.Comment,
            IsVisible = true
        };
        await reviews.AddAsync(review, ct);

        // Fold the new rating into the shop's running average.
        var shop = await _uow.Repository<Shop>().GetByIdAsync(order.ShopId, ct);
        if (shop is not null)
        {
            var newCount = shop.RatingCount + 1;
            shop.RatingAverage = (shop.RatingAverage * shop.RatingCount + request.Rating) / newCount;
            shop.RatingCount = newCount;
            _uow.Repository<Shop>().Update(shop);
        }

        await _uow.SaveChangesAsync(ct);

        var saved = await reviews.FirstOrDefaultAsync(new ReviewWithCustomerByIdSpecification(review.Id), ct);
        return Result.Success(_mapper.Map<ReviewItemDto>(saved!));
    }

    public async Task<Result<PagedResult<ReviewItemDto>>> ListForShopAsync(int shopId, PageRequest page, CancellationToken ct = default)
    {
        var reviews = _uow.Repository<Review>();
        var total = await reviews.CountAsync(new VisibleReviewsByShopCountSpecification(shopId), ct);
        var items = await reviews.ListAsync(new VisibleReviewsByShopSpecification(shopId, page.Skip, page.Take), ct);

        var mapped = _mapper.Map<IReadOnlyList<ReviewItemDto>>(items);
        return Result.Success(new PagedResult<ReviewItemDto>(mapped, total, page.PageNumber, page.PageSize));
    }
}
