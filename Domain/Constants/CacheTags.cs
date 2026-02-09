namespace Domain.Constants;

public static class Cache
{
    public partial class Tags
    {
        public const string Bundle = "bundles";
        public const string Cart = "cart";
        public const string Category = "categories";
        public const string Type = "Types";
        public const string Product = "Products";
    }

    public partial class Keys
    {
        public static string Bundles(bool includeNotAvailable) => includeNotAvailable ? "bundle:all" : "bundle:available";
        public static string Bundle(int id) => $"bundle:{id}";

        public static string Cart(string userId) => $"cart:{userId}";

        public const string Categories = "category:all";
        public static string Category(int id) => $"category:{id}";

        public const string Types = "types:all";
        public static string Type(int id) => $"type:{id}";

        public static string Product(int id) => $"product:{id}";
    }

    public static class Expirations
    {
        public static readonly TimeSpan Short = TimeSpan.FromMinutes(5);
        public static readonly TimeSpan Medium = TimeSpan.FromMinutes(30);
        public static readonly TimeSpan Long = TimeSpan.FromHours(1);
        public static readonly TimeSpan VeryLong = TimeSpan.FromHours(24);
    }
}
