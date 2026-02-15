namespace Application.Contracts.Notifications;

public record OrderReportInfo(
    int OrderId,
    string CustomerId,
    DateTime OrderDate,
    decimal Total
);
