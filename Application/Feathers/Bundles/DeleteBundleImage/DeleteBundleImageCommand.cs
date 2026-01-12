namespace Application.Feathers.Bundles.DeleteBundleImage;

public record DeleteBundleImageCommand(int Id) : IRequest<Result>;

