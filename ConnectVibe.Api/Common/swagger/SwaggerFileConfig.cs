using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ConnectVibe.Api.Common.swagger
{
    public class SwaggerFileUploadOperation : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var fileParams = operation.Parameters.Where(p => p.Name.Contains("imageFile")).ToList();
            foreach (var param in fileParams)
            {
                operation.Parameters.Remove(param);
            }

            operation.RequestBody = new OpenApiRequestBody
            {
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    ["multipart/form-data"] = new OpenApiMediaType
                    {
                        Schema = new OpenApiSchema
                        {
                            Type = "object",
                            Properties =
                        {
                            ["imageFile"] = new OpenApiSchema
                            {
                                Type = "string",
                                Format = "binary"
                            }
                        }
                        }
                    }
                }
            };
        }
    }
}