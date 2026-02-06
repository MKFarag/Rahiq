namespace Domain.Constants;

public static class Permissions
{
    public static string Type { get; } = nameof(Permissions).ToLower();

    public const string AddBundle = "bundle:add";
    public const string UpdateBundle = "bundle:modify";
    public const string BundleActivation = "bundle:activation";

    public const string AddCategory = "category:add";
    public const string UpdateCategory = "category:modify";
    public const string DeleteCategory = "category:delete";

    public const string ReadOrder = "order:read";
    public const string AddOrder = "order:add";
    public const string CancelOrder = "order:cancel";
    public const string ChangeOrderStatus = "order:change-status";

    public const string PaymentVerify = "payment:verify";

    public const string AddProduct = "product:add";
    public const string UpdateProduct = "product:modify";
    public const string DiscountProduct = "product:discount";
    public const string ChangeProductStatus = "product:change-status";

    public const string ReadRole = "role:read";
    public const string AddRole = "role:add";
    public const string UpdateRole = "role:modify";
    public const string ToggleRoleStatus = "role:toggle-status";

    public const string ReadShipping = "shipping:read";
    public const string AssignShippingDetails = "shipping:assign-details";
    public const string UpdateShipping = "shipping:modify";
    public const string DeleteShipping = "shipping:delete";

    public const string AddType = "type:add";
    public const string UpdateType = "type:modify";
    public const string DeleteType = "type:delete";

    public static IList<string?> GetAll()
        => [.. typeof(Permissions).GetFields().Select(x => x.GetValue(x) as string)];
}
