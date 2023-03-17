using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using VacationAPI.DTOs;

namespace VacationAPI
{
    /// <summary>
    /// Provides custom example values for DTO models in SwaggerUI documentation
    /// </summary>
    public class CustomSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (context.Type == typeof(LoginDTO))
            {
                schema.Example = new OpenApiObject
                {
                    ["userName"] = new OpenApiString("johndoe"),
                    ["password"] = new OpenApiString("password")
                };

            }
        }
    }
}
