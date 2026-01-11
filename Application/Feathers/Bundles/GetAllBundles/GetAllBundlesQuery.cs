namespace Application.Feathers.Bundles.GetAllBundles;

public record GetAllBundlesQuery(bool IncludeNotAvailable = false) : IRequest<IEnumerable<BundleResponse>>;

