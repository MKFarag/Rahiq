namespace Domain.Entities;

public sealed class BundleItem
{
    public int BundleId { get; set; }
    public int ProductId { get; set; }
    public int QuantityAvailable { get; set; }

    public Bundle Bundle { get; set; } = default!;
    public Product Product { get; set; } = default!;
}
