namespace Domain.Entities;

public sealed class Payment
{
    public int Id { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsProofed { get; set; }
    public decimal Amount { get; set; }
}
