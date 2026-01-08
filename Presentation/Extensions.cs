#region Usings

using Application.Contracts.Files;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using NSwag;
using NSwag.Generation;
using NSwag.Generation.AspNetCore;
using NSwag.Generation.Processors.Security;
using System.Security.Claims;

#endregion

namespace Presentation;

public static class Extensions
{
    extension(Result result)
    {
        public ObjectResult ToProblem()
        {
            if (result.IsSuccess)
                throw new InvalidOperationException("Cannot create a problem details response for a successful result.");

            var problem = Results.Problem(statusCode: result.Error.StatusCode);

            var problemDetails = problem.GetType().GetProperty(nameof(ProblemDetails))!.GetValue(problem) as ProblemDetails;

            problemDetails!.Extensions = new Dictionary<string, object?>
            {
                {
                    "error", new Dictionary<string, string>
                    {
                        { "code", result.Error.Code },
                        { "description", result.Error.Description }
                    }
                }
            };

            return new ObjectResult(problemDetails);
        }
    }

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

    extension(ControllerBase controller)
    {
        internal IActionResult ToProblem(ValidationResult result)
        {
            var modelState = new ModelStateDictionary();

            foreach (var error in result.Errors)
                modelState.AddModelError(error.PropertyName, error.ErrorMessage);

            return controller.ValidationProblem(modelState);
        }
    }

    extension(ClaimsPrincipal user)
    {
        public string? GetId()
            => user.FindFirstValue(ClaimTypes.NameIdentifier);
    }

    public static FileData ToFileData(this IFormFile formFile)
        => new(formFile.FileName, formFile.ContentType, formFile.Length, formFile.OpenReadStream());

    public static IEnumerable<FileData> ToFileData(this IFormFileCollection formFiles)
        => formFiles.Select(f => f.ToFileData());
}
