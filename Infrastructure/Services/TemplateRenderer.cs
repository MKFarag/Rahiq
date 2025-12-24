using Microsoft.Extensions.Hosting;

namespace Infrastructure.Services;

public class TemplateRenderer(IHostEnvironment env) : ITemplateRenderer
{
    public async Task<string> RenderAsync(string templateName, Dictionary<string, string> placeholders)
    {
        var templatePath = Path.Combine(env.ContentRootPath, "Templates", $"{templateName}.html");

        if (!File.Exists(templatePath))
            throw new FileNotFoundException($"Template {templateName} not found at {templatePath}");

        var templateContent = await File.ReadAllTextAsync(templatePath);

        foreach (var item in placeholders)
            templateContent = templateContent.Replace(item.Key, item.Value ?? string.Empty);

        return templateContent;
    }
}