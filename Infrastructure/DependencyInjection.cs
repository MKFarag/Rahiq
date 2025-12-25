#region Usings

using Infrastructure.Messaging;
using Infrastructure.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

#endregion

namespace Infrastructure;

public static class DependencyInjection
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddInfrastructureDependencies(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection") ??
                throw new InvalidOperationException("DefaultConnection is not found in appsettings.json");

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(connectionString);
            });

            services.AddCQRSConfig(typeof(Application.DependencyInjection).Assembly);

            return services;
        }

        private IServiceCollection AddCQRSConfig(params Assembly[] assemblies)
        {
            services.AddScoped<ISender, Sender>();

            if (assemblies.Length == 0)
                assemblies = [Assembly.GetCallingAssembly()];

            foreach (var assembly in assemblies)
            {
                var handlers = assembly.GetTypes()
                    .Where(t => t is { IsClass: true, IsAbstract: false, IsGenericType: false })
                    .SelectMany(t => t.GetInterfaces()
                        .Where(i => i.IsGenericType &&
                            (i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>) ||
                                i.GetGenericTypeDefinition() == typeof(IRequestHandler<>)))
                        .Select(i => (Interface: i, Implementation: t)));

                foreach (var (iface, impl) in handlers)
                    services.AddScoped(iface, impl);
            }

            return services;
        }
    }
}
