namespace Application.Contracts.Order;

public record OrderPaymentResponse(
    int Id,
    string ImageUrl,
    bool IsProofed,
    decimal Amount
);
