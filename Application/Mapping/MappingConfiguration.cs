namespace Application.Mapping;

public class MappingConfiguration : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
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

        config.NewConfig<Product, OrderProductResponse>()
            .Map(dest => dest.UnitPrice, src => src.SellingPrice);
    }
}
