namespace Domain.Entities;

public sealed class Order
{
    public int Id { get; set; }
    public string CustomerId { get; set; } = string.Empty;
    public int ShippingId { get; set; }
    public DateTime Date { get; set; } = DateTime.UtcNow;
    public string Phone { get; set; } = string.Empty;
    public OrderStatus Status { get; set; }
    public decimal Total { get; set; }

    public Shipping Shipping { get; set; } = default!;
    public ICollection<OrderItem> OrderItems { get; set; } = [];
}
