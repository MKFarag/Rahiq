namespace Domain.Entities;

public sealed class Bundle
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int DiscountPercentage { get; set; }
    public DateOnly EndAt { get; set; }

    public ICollection<BundleItem> BundleItems { get; set; } = default!;
}
