namespace Application.Contracts.Notifications;

public record PendingPaymentApprovalInfo(
    int OrderId,
    string CustomerId,
    int PaymentId,
    decimal Amount
);
