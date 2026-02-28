using Application.Contracts.Notifications;

namespace Application.Mapping;

public class MappingConfiguration : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<(Product p, Category c, Domain.Entities.Type t), ProductResponse>()
            .Map(dest => dest, src => src.p)
            .Map(dest => dest.Category, src => src.c.Name)
            .Map(dest => dest.Type, src => src.t.Name)
            .Map(dest => dest.StandardPrice, src => src.p.Price);

        config.NewConfig<Product, ProductResponse>()
            .Map(dest => dest.Category, src => src.Category.Name)
            .Map(dest => dest.Type, src => src.Type.Name)
            .Map(dest => dest.StandardPrice, src => src.Price);

        config.NewConfig<Bundle, BundleDetailResponse>()
            .Map(dest => dest.Bundle, src => src)
            .Map(dest => dest.Products, src => src.BundleItems.Select(x => x.Product));

        config.NewConfig<Cart, CartResponse>()
            .Map(dest => dest.CartProducts, src => src.Product)
            .Map(dest => dest.CartBundle, src => src.Bundle);

        config.NewConfig<(User user, IList<string> roles), UserResponse>()
            .Map(dest => dest, src => src.user)
            .Map(dest => dest.Roles, src => src.roles);

        config.NewConfig<Bundle, BundleQuantityWarning>()
            .Map(dest => dest.BundleId, src => src.Id)
            .Map(dest => dest.BundleName, src => src.Name);
        
        config.NewConfig<BundleItem, BundleItemQuantityWarning>()
            .Map(dest => dest.ItemId, src => src.ProductId)
            .Map(dest => dest.ItemName, src => src.Product.Name);

        config.NewConfig<Order, OrderReportInfo>()
            .Map(dest => dest.OrderId, src => src.Id)
            .Map(dest => dest.CustomerId, src => src.CustomerId)
            .Map(dest => dest.OrderDate, src => src.Date)
            .Map(dest => dest.Total, src => src.GrandTotal);

        config.NewConfig<OrderItem, OrderItemResponse>()
            .Map(dest => dest.Name, src => src.IsBundle ? src.Bundle!.Name : src.Product!.Name)
            .Map(dest => dest.ImageUrl, src => src.IsBundle ? src.Bundle!.ImageUrl : src.Product!.ImageUrl);

        config.NewConfig<Order, OrderDetailsResponse>()
            .Map(dest => dest.OrderDate, src => src.Date)
            .Map(dest => dest.Status, src => src.Status.ToString());

        config.NewConfig<Order, OrderResponse>()
            .Map(dest => dest.OrderDate, src => DateOnly.FromDateTime(src.Date))
            .Map(dest => dest.NumberOfItems, src => src.OrderItems.Sum(x => x.Quantity));
    }
}
