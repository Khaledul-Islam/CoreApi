using Models.Enums;
using System.Net;
namespace Utilities.Exceptions;

/// <summary>
/// Represents errors when the requested data could not be found.
/// </summary>
public sealed class NotFoundException(string message = "Not found", object? additionalData = null)
    : BaseWebApiException(message, HttpStatusCode.NotFound, ApiResultBodyCode.NotFound, additionalData);
