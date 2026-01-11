namespace Application.Feathers.Bundles.AddBundle;

public record AddBundleCommand(BundleRequest Request, FileData? Image) : IRequest<Result<BundleDetailResponse>>;
