namespace Application.Feathers.Bundles;

public record GetAllBundlesQuery(bool IncludeNotAvailable = false) : IRequest<IEnumerable<BundleResponse>>;

