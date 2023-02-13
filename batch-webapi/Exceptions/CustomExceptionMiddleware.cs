using Azure;
using batch_webapi.Data.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;

namespace batch_webapi.Exceptions
{
    public class CustomExceptionMiddleware : IMiddleware
    {

        private readonly ILogger<CustomExceptionMiddleware> _logger;
        public CustomExceptionMiddleware(ILogger<CustomExceptionMiddleware> logger)
        {
            _logger = logger;
        }


        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);

            }
            catch (HttpStatusCodeException ex)
            {
                await HandleExceptionAsync(context, ex);
            }
            catch (Exception ex)
            {

                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext httpContext, Exception ex)
        {
            _logger.LogError(ex, ex.Message);

            httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            httpContext.Response.ContentType = "application/json";
            var response = new ErrorVM()
            {
                StatusCode = httpContext.Response.StatusCode,
                CorrelationId = httpContext.Request.Path,
                Errors = new List<Error>()
                            {
                                new Error()
                                {
                                    Source = httpContext.Request.Path,
                                    Description = ex.Message.ToString()
                                }
                            }
            }.ToString();

            return httpContext.Response.WriteAsync(response);
        }

        private Task HandleExceptionAsync(HttpContext context, HttpStatusCodeException exception)
        {
            _logger.LogError(exception.Message,context.Request.Path);
            ErrorVM errorVM = null;
            
            if (exception is HttpStatusCodeException)
            {
               
                context.Response.StatusCode = (int)exception.StatusCode;

                errorVM = new ErrorVM()
                {
                    StatusCode = (int)exception.StatusCode,
                    CorrelationId = "",
                    Errors = new List<Error>()
                            {
                                new Error()
                                {
                                    Source = context.Request.Path,
                                    Description = exception.Message
                                }
                            }
                };
            }
            else
            {
                errorVM = new ErrorVM()
                {
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    CorrelationId = "",
                    Errors = new List<Error>()
                            {
                                new Error()
                                {
                                    Source = context.Request.Path,
                                    Description = exception.Message
                                }
                            }
                };
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            }
            string json = System.Text.Json.JsonSerializer.Serialize(errorVM);
            context.Response.ContentType = "application/json";
            return context.Response.WriteAsync(json);
        }

    }
}
