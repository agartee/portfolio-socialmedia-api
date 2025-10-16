using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SocialMedia.WebAPI.Swagger
{
    public class FlattenIdParameterOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null)
                return;

            foreach (var param in operation.Parameters.ToList())
            {
                if (param.Name.EndsWith(".Value"))
                {
                    param.Name = param.Name.Replace(".Value", "");
                }
            }
        }
    }
}
