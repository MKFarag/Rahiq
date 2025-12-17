#region Usings

using Application;
using Application.Interfaces.Infrastructure;
using Hangfire;
using Infrastructure;
using Infrastructure.Authentication;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Identities;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Presentation.Abstraction;
using System.Text;

#endregion

namespace Presentation;

public static class DependencyInjection
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddDependencies(IConfiguration configuration)
        {
            services.AddControllers();
            services.AddInfrastructureDependencies(configuration);
            services.AddHangfireConfig(configuration);
            services.AddApplicationDependencies();
            services.AddAuthConfig(configuration);

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services
                .AddExceptionHandler<GlobalExceptionHandler>()
                .AddProblemDetails();

            services
                .AddEndpointsApiExplorer()
                .AddOpenApiDocument(options =>
                {
                    options.AddBearerSecurity();
                    options.ConfigureDocument();
                });

            return services;
        }

        private IServiceCollection AddAuthConfig(IConfiguration configuration)
        {
            #region Jwt

            services.AddSingleton<IJwtProvider, JwtProvider>();

            services.AddOptions<JwtOptions>()
                .BindConfiguration(JwtOptions.SectionName)
                .ValidateDataAnnotations()
                .ValidateOnStart();

            #endregion

            #region Roles

            //services.AddTransient<IAuthorizationHandler, PermissionAuthorizationHandler>();
            //services.AddTransient<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();

            #endregion

            #region Add Identity

            services.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            #endregion

            #region Validations

            var settings = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>();

            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(o =>
                {
                    o.SaveToken = true;
                    o.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings?.Key!)),
                        ValidIssuer = settings?.Issuer,
                        ValidAudience = settings?.Audience
                    };
                });

            #endregion

            #region Identity Configurations

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequiredLength = 8;
                options.SignIn.RequireConfirmedEmail = true;
                options.User.RequireUniqueEmail = true;
            });

            services.Configure<DataProtectionTokenProviderOptions>(options =>
            {
                options.TokenLifespan = TimeSpan.FromHours(24);
            });

            #endregion

            #region Service Lifetime

            //services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ISignInService, SignInService>();

            #endregion

            return services;
        }

        private IServiceCollection AddHangfireConfig(IConfiguration configuration)
        {
            services.AddScoped<IJobManager, JobManager>();

            services.AddHangfire(config => config
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(configuration.GetConnectionString("HangfireConnection"))
            );

            services.AddHangfireServer();

            return services;
        }
    }
}
