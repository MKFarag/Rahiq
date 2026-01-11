namespace Application.Feathers.Bundles.GetBundle;

public record GetBundleQuery(int Id) : IRequest<Result<BundleDetailResponse>>;
