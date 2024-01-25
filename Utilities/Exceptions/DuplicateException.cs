using System.Net;
using Models.Enums;
namespace Utilities.Exceptions;

/// <summary>
/// Represents errors that occur when an attempt was made to save a duplicate record .
/// </summary>
public sealed class DuplicateException(string message = "Duplication", object? additionalData = null)
    : BaseWebApiException(message, HttpStatusCode.Conflict, ApiResultBodyCode.Duplication, additionalData);