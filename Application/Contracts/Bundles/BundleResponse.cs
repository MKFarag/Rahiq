namespace Application.Contracts.Bundles;

public record BundleResponse(
    int Id,
    string Name,
    int DiscountPercentage,
    int RemainingDays,
    int QuantityAvailable,
    decimal OldPrice,
    decimal SellingPrice
);