namespace Domain.Entities;

public sealed class Bundle
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int DiscountPercentage { get; set; }
    public int QuantityAvailable { get; set; }
    public string? ImageUrl { get; set; }
    public DateOnly EndAt { get; set; }

    public bool IsActive => EndAt >= DateOnly.FromDateTime(DateTime.UtcNow) && QuantityAvailable > 0;
    public int RemainingDays => Math.Max(0, (EndAt.ToDateTime(TimeOnly.MinValue) - DateTime.UtcNow).Days);
    public decimal OldPrice => BundleItems.Sum(x => x.Product.Price);
    public decimal SellingPrice => OldPrice.ApplyDiscount(DiscountPercentage);

    public ICollection<BundleItem> BundleItems { get; set; } = default!;
}
