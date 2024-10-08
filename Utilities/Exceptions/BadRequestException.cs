﻿using Models.Enums;
using System.Net;
namespace Utilities.Exceptions;

/// <summary>
/// Represents errors that occur when invalid arguments passed.
/// </summary>
public sealed class BadRequestException(string message = "Bad Request", object? additionalData = null)
    : BaseWebApiException(message, HttpStatusCode.BadRequest, ApiResultBodyCode.BadRequest, additionalData);
