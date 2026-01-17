namespace Domain.Entities;

public sealed class Order
{
    public int Id { get; set; }
    public string CustomerId { get; set; } = string.Empty;
    public int? ShippingId { get; set; }
    public DateTime Date { get; set; } = DateTime.UtcNow;
    public OrderStatus Status { get; set; }
    public decimal Total { get; set; }

    public decimal? GrandTotal => Shipping is not null ? Total + Shipping.Cost : null;

    public Shipping? Shipping { get; set; }
    public ICollection<OrderItem> OrderItems { get; set; } = [];
}
