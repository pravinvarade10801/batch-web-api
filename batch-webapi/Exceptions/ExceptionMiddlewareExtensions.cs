using batch_webapi.Controllers;
using batch_webapi.Data.ViewModels;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace batch_webapi.Exceptions
{
    public static class ExceptionMiddlewareExtensions
    {       
        public static void ConfigureBuildInExceptionHandler(this IApplicationBuilder app)
        {
            
                app.UseExceptionHandler(appError =>
                {
                    appError.Run(async context =>
                    {
                        
                        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        context.Response.ContentType = "application/json";

                        var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                        var contextRequest = context.Features.Get<IHttpRequestFeature>();

                        if (contextFeature != null)
                        {
                            var errorVMString = new ErrorVM()
                            {
                                CorrelationId = contextRequest.Path,
                                Errors = new List<Error>()
                                {
                                new Error()
                                {
                                    Source = contextFeature.Error.Source,
                                    Description = contextFeature.Error.Message
                                }
                                }

                            }.ToString();
                            
                            await context.Response.WriteAsync(errorVMString);
                        }
                    });
                });
            
        }

        public static void ConfigureCustomExceptionHandler(this IApplicationBuilder app)
        {
            app.UseMiddleware<CustomExceptionMiddleware>();
        }
    }

}
