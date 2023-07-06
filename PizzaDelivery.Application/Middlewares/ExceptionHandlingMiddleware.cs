using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.AspNetCore.Http.Features;

namespace PizzaDelivery.Application.HandleExceptions;

public class ExceptionHandlingMiddleware 
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);

            if (context.Response.StatusCode >= 400)
            {
                int statusCode = context.Response.StatusCode;
                string statusDescription = context.Features.Get<IHttpResponseFeature>()?.ReasonPhrase;
                if (string.IsNullOrEmpty(statusDescription))
                {
                    statusDescription = GetDefaultStatusDescription(statusCode);
                }
                var exception = new Exception($"HTTP - {statusDescription}");
                throw exception;
            }
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var error = new PizzaDelivery.Application.Helpers.Error
        {
            Message = exception.Message,
            ExceptionType = exception.GetType().Name,
            StatusCode = exception switch
            {
                UnauthorizedAccessException => HttpStatusCode.Unauthorized,
                _ => HttpStatusCode.InternalServerError
            },
            //StackTrace = exception.StackTrace
        };

        _logger.LogError(error.ToString());
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        await context.Response.WriteAsJsonAsync(error);
    }


    private string GetDefaultStatusDescription(int statusCode)
    {
        if (Enum.IsDefined(typeof(HttpStatusCode), statusCode))
        {
            return ((HttpStatusCode)statusCode).ToString();
        }

        return "Unknown";
    }
}

