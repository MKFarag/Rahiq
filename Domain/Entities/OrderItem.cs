
namespace Domain.Entities;

public sealed class OrderItem
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public int? BundleId { get; set; }
    public int? ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }

    public bool IsBundle => BundleId.HasValue;
    public bool IsProduct => ProductId.HasValue;
    public decimal Total => UnitPrice * Quantity;

    public Order Order { get; set; } = default!;
    public Bundle? Bundle { get; set; }
    public Product? Product { get; set; }
}
