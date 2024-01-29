using Models.Enums;
using System.Net;
namespace Utilities.Exceptions;

/// <summary>
/// Represents errors that occur when a received Security Token has expiration time in the past.
/// </summary>
public sealed class TokenExpiredException(string message = "Authenticate failure.", object? additionalData = null)
    : BaseWebApiException(message, HttpStatusCode.Unauthorized, ApiResultBodyCode.ExpiredSecurityToken, additionalData);