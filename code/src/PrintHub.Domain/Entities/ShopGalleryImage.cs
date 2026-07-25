using PrintHub.Domain.Common;

namespace PrintHub.Domain.Entities;

/// <summary>A sample-work photo in a shop's portfolio gallery.</summary>
public class ShopGalleryImage : BaseEntity
{
    public int ShopId { get; set; }
    public string Url { get; set; } = null!;
    public string? Caption { get; set; }
    public int DisplayOrder { get; set; }

    public Shop Shop { get; set; } = null!;
}
