namespace Domain.Entities;

public sealed class Payment
{
    public int Id { get; set; }
    public string Image { get; set; } = string.Empty;
    public bool IsProofed { get; set; }
    public decimal Amount { get; set; }
}
