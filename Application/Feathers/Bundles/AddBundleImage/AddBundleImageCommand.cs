namespace Application.Feathers.Bundles.AddBundleImage;

public record AddBundleImageCommand(int Id, FileData Image) : IRequest<Result>;

