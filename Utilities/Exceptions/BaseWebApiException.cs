using Models.Enums;
using System.Net;

namespace Utilities.Exceptions;

/// <summary>
/// Represents errors that occur during execution of application with appropriate status code.
/// </summary>
public abstract class BaseWebApiException(string? message = null,
        HttpStatusCode httpStatusCode = HttpStatusCode.InternalServerError,
        ApiResultBodyCode apiResultBodyCode = ApiResultBodyCode.ServerError, object? additionalData = null)
    : Exception(message)
{
    public HttpStatusCode HttpStatusCode { get; } = httpStatusCode;

    public ApiResultBodyCode ApiResultBodyCode { get; } = apiResultBodyCode;

    public object? AdditionalData { get; set; } = additionalData;
}