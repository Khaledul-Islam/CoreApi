using Models.Enums;
using System.Net;

namespace Utilities.Exceptions;

/// <summary>
/// Represents errors that occur during execution of application with appropriate status code.
/// </summary>
public abstract class BaseWebApiException : Exception
{
    public HttpStatusCode HttpStatusCode { get; }

    public ApiResultBodyCode ApiResultBodyCode { get; }

    public object? AdditionalData { get; set; }

    protected BaseWebApiException(string? message = null, HttpStatusCode httpStatusCode = HttpStatusCode.InternalServerError, ApiResultBodyCode apiResultBodyCode = ApiResultBodyCode.ServerError, object? additionalData = null)
        : base(message)
    {
        HttpStatusCode = httpStatusCode;
        ApiResultBodyCode = apiResultBodyCode;
        AdditionalData = additionalData;
    }
}