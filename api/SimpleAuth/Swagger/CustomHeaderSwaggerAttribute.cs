using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SimpleAuth.Swagger
{
    public class CustomHeaderSwaggerAttribute : IOperationFilter
    {
        private readonly IConfiguration _configuration;

        public CustomHeaderSwaggerAttribute(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            operation.Parameters ??= new List<OpenApiParameter>();
        
            var headerNeeded = _configuration.GetSection("HeadersNeeded").Get<string[]>();
            if (headerNeeded == null) 
                return;
        
            foreach (var header in headerNeeded)
            {
                operation.Parameters.Add(new OpenApiParameter
                {
                    Name = header,
                    In = ParameterLocation.Header,
                    Required = true,
                    Schema = new OpenApiSchema
                    {
                        Type = "string"
                    }
                });
            }
        }
    }
}