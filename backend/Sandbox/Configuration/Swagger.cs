using Microsoft.OpenApi.Models;
using System.Reflection;

namespace Sandbox.Configuration;

public static class Swagger
{
    public static void AddSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Sandbox API by Roman Koshchei",
                Version = "v1",
                Contact = new OpenApiContact
                {
                    Name = "Roman Koshchei",
                    Email = "romankoshchei@gmail.com",
                    Url = new Uri("https://www.flurium.com")
                }
            });

            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));

            /*
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please add token",
                Name = "Auth token",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = "Bearer"
            });
             options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type= ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        },
                    }, Array.Empty<string>()
                }
            });
            */



            options.SupportNonNullableReferenceTypes();
        });
    }
}
