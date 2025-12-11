using Infrastructure;

namespace Presentation;

public static class DependencyInjection
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddDependencies(IConfiguration configuration)
        {
            services.AddControllers();
            services.AddInfrastructureDependencies(configuration);



            services
                .AddEndpointsApiExplorer()
                .AddOpenApi();

            return services;
        }
    }
}
