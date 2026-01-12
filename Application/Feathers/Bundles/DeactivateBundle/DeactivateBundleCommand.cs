namespace Application.Feathers.Bundles.DeactivateBundle;

public record DeactivateBundleCommand(int Id) : IRequest<Result>;

