using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EagleRock.Auth
{
    /// <summary>
    /// Provides an attribute that can be used above REST methods to require that they contain an API Key in the header.
    /// </summary>
    /// <remarks>
    /// Currently this class does not have any Claims. In a real app the API Key implementation would be very different.
    /// </remarks>
    public class ApiKeyAuthAttribute : Attribute, IAsyncActionFilter
    {
        private const string HeaderApiKeyName = "ApiKey";
        private const string ConfigApiKeyName = "ApiKey";
        
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // Get the API Key from the request header
            if (context.HttpContext.Request.Headers.TryGetValue(HeaderApiKeyName, out var apiKey))
            {
                // Compare the API key to our config value. In a real application this would come from another service
                // or database
                // var allowedApiKeys = context.HttpContext
                //                             .RequestServices
                //                             .GetRequiredService<IConfiguration>()
                //                             .GetValue<string[]>(ConfigApiKeyName);

                var allowedApiKeys = context.HttpContext
                                            .RequestServices
                                            .GetRequiredService<IConfiguration>()
                                            .GetSection(ConfigApiKeyName)
                                            .AsEnumerable()
                                            .Select(x => x.Value);
                    
                if (allowedApiKeys.Any(allowedKey => allowedKey == apiKey))
                {
                    // Run the REST method as normal
                    await next();
                    return;
                }
            }
            
            // The request is not authorised
            context.Result = new UnauthorizedResult();
        }
    }
}