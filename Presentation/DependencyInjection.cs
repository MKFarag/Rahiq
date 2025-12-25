#region Usings

using Application;
using Application.Interfaces;
using Domain.Settings;
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
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;
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
            services.AddFluentValidationAutoValidation();
            services.AddMailConfig(configuration);
            services.AddAuthConfig(configuration);

            services.AddScoped<ISignInService, SignInService>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IUrlEncoder, UrlEncoder>();

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .WithOrigins(configuration.GetSection("AllowedOrigins").Get<string[]>()!);
                });
            });

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

            #endregion

            #region Permission based authentication

            //services.AddTransient<IAuthorizationHandler, PermissionAuthorizationHandler>();
            //services.AddTransient<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();

            #endregion

            #region Add Identity

            services
                .AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            #region Configurations

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequiredLength = 8;
                options.SignIn.RequireConfirmedEmail = true;
                options.User.RequireUniqueEmail = true;
            });

            #endregion

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

        private IServiceCollection AddMailConfig(IConfiguration configuration)
        {

            services.AddOptions<EmailTemplateOptions>()
                .Bind(configuration.GetSection(nameof(EmailTemplateOptions)));

            services.AddOptions<AppUrlSettings>()
                .Bind(configuration.GetSection(AppUrlSettings.SectionName));

            services.AddOptions<MailSettings>()
                .Bind(configuration.GetSection(nameof(MailSettings)))
                .ValidateDataAnnotations()
                .ValidateOnStart();

            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<ITemplateRenderer, TemplateRenderer>();
            services.AddScoped<IEmailSender, EmailSender>();

            return services;
        }
    }
}
