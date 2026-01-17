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
    public bool CanBeCancelled() => Status is OrderStatus.Pending or OrderStatus.Processing;

    public Shipping? Shipping { get; set; }
    public ICollection<OrderItem> OrderItems { get; set; } = [];

    public void StartProcessing()
    {
        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException("Order must be Pending.");

        Status = OrderStatus.Processing;
    }

    public void Ship(int shippingId)
    {
        if (Status != OrderStatus.Processing)
            throw new InvalidOperationException("Order must be Processing.");

        ShippingId = shippingId;
        Status = OrderStatus.Shipped;
    }

    public void Deliver()
    {
        if (Status != OrderStatus.Shipped)
            throw new InvalidOperationException("Order must be Shipped.");

        Status = OrderStatus.Delivered;
    }

    public void Cancel()
    {
        if (Status == OrderStatus.Shipped || Status == OrderStatus.Delivered)
            throw new InvalidOperationException("Cannot cancel shipped or delivered order.");

        Status = OrderStatus.Canceled;
    }
}
