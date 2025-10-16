using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace SocialMedia.WebAPI.Swagger
{
    public class FlattenIdSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            var type = context.Type;

            var valueProp = type.GetProperty("Value", BindingFlags.Public | BindingFlags.Instance);

            if (valueProp != null && type.GetProperties().Length == 1)
            {
                var propType = valueProp.PropertyType;

                schema.Type = propType.Name switch
                {
                    nameof(Guid) => "string",
                    nameof(Int32) => "integer",
                    nameof(Int64) => "integer",
                    nameof(String) => "string",
                    _ => "object"
                };

                schema.Format = propType.Name switch
                {
                    nameof(Guid) => "uuid",
                    nameof(Int32) => "int32",
                    nameof(Int64) => "int64",
                    _ => null
                };

                schema.Properties.Clear();
                schema.Reference = null;
            }
        }
    }
}
