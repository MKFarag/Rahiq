namespace Application.Feathers.Bundles.UpdateBundle;

public record UpdateBundleCommand(int Id, BundleRequest Request) : IRequest<Result>;

