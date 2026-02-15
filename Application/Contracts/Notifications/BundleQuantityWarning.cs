namespace Application.Contracts.Notifications;

public record BundleQuantityWarning(
    int BundleId,
    string BundleName,
    IEnumerable<BundleItemQuantityWarning> BundleItems
);
