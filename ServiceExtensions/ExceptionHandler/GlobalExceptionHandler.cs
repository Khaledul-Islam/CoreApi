using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Models.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Serilog;
using System.Net;
using Utilities.Exceptions;
using Utilities.Extensions;
using Utilities.Response;

namespace ServiceExtensions.ExceptionHandler;

public class GlobalExceptionHandler(RequestDelegate next,
    IWebHostEnvironment env)
{
    public async Task Invoke(HttpContext context)
    {
        string? message = null;
        var httpStatusCode = HttpStatusCode.InternalServerError;
        var apiStatusCode = ApiResultBodyCode.ServerError;
        var jsonSerializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        try
        {
            await next(context);
        }

        catch (BaseWebApiException exception)
        {
            var ipAddress = NetworkExtensions.GetLocalIpAddress();
            var macAddress = NetworkExtensions.GetMacAddress();

            Log.ForContext("IpAddress", ipAddress)
                .ForContext("MacAddress", macAddress)
                .Error(exception, $"Middleware ---> AppException : {exception.Message}");

            httpStatusCode = exception.HttpStatusCode;
            apiStatusCode = exception.ApiResultBodyCode;

            // Generate message
            if (env.IsDevelopment())
            {
                var dic = new Dictionary<string, string?>
                {
                    ["Exception"] = exception.Message,
                    ["StackTrace"] = exception.StackTrace,
                };
                if (exception.InnerException is not null)
                {
                    dic.Add("InnerException.Exception", exception.InnerException.Message);
                    dic.Add("InnerException.StackTrace", exception.InnerException.StackTrace);
                }
                if (exception.AdditionalData is not null)
                {
                    dic.Add("AdditionalData", JsonConvert.SerializeObject(exception.AdditionalData));
                }

                message = JsonConvert.SerializeObject(dic);
            }
            else
            {
                message = exception.Message;
            }

            // Write to response
            if (exception is BadRequestException)
            {
                await WriteToResponseAsync(exception.AdditionalData);
            }
            else
            {
                await WriteToResponseAsync();
            }
        }

        catch (Exception exception)
        {
            var ipAddress = NetworkExtensions.GetLocalIpAddress();
            var macAddress = NetworkExtensions.GetMacAddress();

            Log.ForContext("IpAddress", ipAddress)
                .ForContext("MacAddress", macAddress)
                .Error(exception, $"Middleware ---> AppException : {exception.Message}");

            if (env.IsDevelopment())
            {
                var dic = new Dictionary<string, string?>
                {
                    ["Exception"] = exception.Message,
                    ["StackTrace"] = exception.StackTrace,
                };
                message = JsonConvert.SerializeObject(dic, jsonSerializerSettings);
            }
            await WriteToResponseAsync();
        }

        // Local function
        async Task WriteToResponseAsync(object? data = null)
        {
            if (context.Response.HasStarted)
            {
                throw new InvalidOperationException("The response has already started, the http status code middleware will not be executed.");
            }

            string json;

            if (data is not null)
            {
                json = JsonConvert.SerializeObject(new ApiResponse<object>(false, apiStatusCode, data, message), jsonSerializerSettings);
            }
            else
            {
                json = JsonConvert.SerializeObject(new ApiResponse(false, apiStatusCode, message), jsonSerializerSettings);
            }

            context.Response.StatusCode = (int)httpStatusCode;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(json);
        }
    }
}