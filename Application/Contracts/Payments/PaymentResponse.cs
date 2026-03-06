namespace Application.Contracts.Payments;

public record PaymentResponse(
    int OrderId,
    string CustomerId,
    int PaymentId,
    int Amount,
    string? ImageUrl
);
