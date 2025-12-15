namespace Domain.Entities;

public sealed class Shipping
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public decimal Cost { get; set; }
    public string Code { get; set; } = string.Empty;
}
