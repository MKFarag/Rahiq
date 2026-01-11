namespace Application.Contracts.Bundles;

public record BundleDetailResponse(
    BundleResponse Bundle,
    IEnumerable<ProductResponse> Products
);
