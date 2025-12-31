namespace Application.Mapping;

public class MappingConfiguration : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Product, ProductResponse>()
            .Map(dest => dest.Category, src => src.Category.Name)
            .Map(dest => dest.Type, src => src.Type.Name)
            .Map(dest => dest.StandardPrice, src => src.Price);
    }
}
