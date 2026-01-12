namespace Application.Feathers.Bundles.ReactivateBundle;

public record ReactivateBundleCommand(int Id, DateOnly Date, int QuantityAvailable) : IRequest<Result>;

