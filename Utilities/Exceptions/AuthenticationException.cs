using System.Net;
using Models.Enums;
namespace Utilities.Exceptions;

/// <summary>
/// Represents errors that occur when authenticate failure or need to authentication .
/// </summary>
public sealed class AuthenticationException(string message = "Authenticate failure.", object? additionalData = null)
    : BaseWebApiException(message, HttpStatusCode.Unauthorized, ApiResultBodyCode.UnAuthorized, additionalData);