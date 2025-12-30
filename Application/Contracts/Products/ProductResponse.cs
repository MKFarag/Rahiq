namespace Application.Contracts.Products;

public record ProductResponse(
    int Id,
    string Name,
    string Brand,
    string Category,
    string Type,
    string Description,
    decimal StandardPrice,
    decimal SellingPrice,
    string ImageUrl
);
