namespace Domain.Entities;

public sealed class Cart
{
    public int Id { get; set; }
    public string CustomerId { get; set; } = string.Empty;
    public int? ProductId { get; set; }
    public int? BundleId { get; set; }
    public int Quantity { get; set; }
    public DateTime AddedAt { get; set; } = DateTime.UtcNow;

    public Product? Product { get; set; }
    public Bundle? Bundle { get; set; }

    public bool IsProduct => ProductId.HasValue;
    public bool IsBundle => BundleId.HasValue;
    public decimal UnitPrice => Product?.SellingPrice ?? Bundle?.SellingPrice ?? 0;
    public decimal TotalPrice => UnitPrice * Quantity;
}
