using System.Reflection;
using Microsoft.OpenApi;

namespace InventoryService.Api.Swagger;

public static class SwaggerConfigurationExtensions
{
    private const string DocumentName = "v1";

    public static IServiceCollection AddInventorySwagger(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc(DocumentName, new OpenApiInfo
            {
                Title = "Inventory Service API",
                Version = "v1",
                Description = "A clean ASP.NET Core REST API for product inventory management."
            });

            options.CustomSchemaIds(type =>
                type.FullName?.Replace("+", ".") ?? type.Name);

            var xmlFileName = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlFilePath = Path.Combine(AppContext.BaseDirectory, xmlFileName);

            if (File.Exists(xmlFilePath))
            {
                options.IncludeXmlComments(xmlFilePath);
            }
        });

        return services;
    }

    public static WebApplication UseInventorySwagger(this WebApplication app)
    {
        if (!app.Environment.IsDevelopment())
        {
            return app;
        }

        app.UseSwagger();

        app.UseSwaggerUI(options =>
        {
            options.DocumentTitle = "Inventory Service API";
            options.SwaggerEndpoint($"/swagger/{DocumentName}/swagger.json", "Inventory Service API v1");
            options.DisplayRequestDuration();
        });

        return app;
    }
}