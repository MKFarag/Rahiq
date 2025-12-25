using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Application;

public static class DependencyInjection
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddApplicationDependencies()
        {
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            services.AddMapsterConfig();

            return services;
        }

        private IServiceCollection AddMapsterConfig()
        {
            var MappingConfig = TypeAdapterConfig.GlobalSettings;
            MappingConfig.Scan(Assembly.GetExecutingAssembly());

            services.AddSingleton<IMapper>(new Mapper(MappingConfig));

            return services;
        }
    }
}
