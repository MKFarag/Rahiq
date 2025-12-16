using Microsoft.AspNetCore.Authentication.JwtBearer;
using NSwag;
using NSwag.Generation;
using NSwag.Generation.AspNetCore;
using NSwag.Generation.Processors.Security;

namespace Presentation.Extensions;

public static class SwaggerExtensions
{
    extension(AspNetCoreOpenApiDocumentGeneratorSettings options)
    {
        public OpenApiDocumentGeneratorSettings AddBearerSecurity()
        {
            options.AddSecurity(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
            {
                Type = OpenApiSecuritySchemeType.Http,
                Scheme = JwtBearerDefaults.AuthenticationScheme.ToLower(),
                BearerFormat = "JWT",
                Description = "Please add your token"
            });

            options.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor(JwtBearerDefaults.AuthenticationScheme));

            return options;
        }

        public OpenApiDocumentGeneratorSettings ConfigureDocument()
        {
            options.Title = "Rahiq API";
            options.Description = $"The official API of Rahiq store.";

            return options;
        }
    }
}
