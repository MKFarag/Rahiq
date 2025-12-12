namespace Domain.Entities;

public sealed class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public int TypeId { get; set; }
    public string Description { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int DiscountPercentage { get; set; }
    public bool IsAvailable { get; set; }

    public Category Category { get; set; } = default!;
    public Type Type { get; set; } = default!;
}
