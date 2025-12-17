namespace Domain.Constants;

public static class DefaultRoles
{
    public partial class Admin
    {
        public const string Id = "019b2c2d-01dc-7a64-a484-4d1a62e4a725";
        public const string Name = nameof(Admin);
        public const string ConcurrencyStamp = "019b2c2d-01dc-7a64-a484-4d1b3c6d8f0f";
    }
    public partial class Customer
    {
        public const string Id = "019b2c2d-01dc-7a64-a484-4d1ce2622b71";
        public const string Name = nameof(Customer);
        public const string ConcurrencyStamp = "019b2c2d-01dc-7a64-a484-4d1ddafd5d61";
    }
}
