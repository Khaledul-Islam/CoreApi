using Microsoft.AspNetCore.Builder;
using ServiceExtensions.ExceptionHandler;

namespace ServiceExtensions.ApplicationBuilder;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<GlobalExceptionHandler>();
    }
}