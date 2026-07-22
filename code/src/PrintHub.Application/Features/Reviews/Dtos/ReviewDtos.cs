namespace PrintHub.Application.Features.Reviews.Dtos;

/// <summary>UC-23 — a customer rates a completed order (one review per order).</summary>
public record CreateReviewRequest(int Rating, string? Comment);

public record ReviewItemDto(
    int Id,
    int OrderId,
    int ShopId,
    int Rating,
    string? Comment,
    string CustomerName,
    string? ShopReply,
    DateTime? RepliedAt,
    DateTime CreatedAt);
