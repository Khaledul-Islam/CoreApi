using Models.Enums;
using Newtonsoft.Json;
using System.Globalization;
using System.Net;
using System.Text;
using Contracts.ContextAccessor;
using Models.Dtos.User;
using Utilities.Extensions;

namespace Utilities.Response;

public class ApiResponse(bool isSuccess, ApiResultBodyCode apiResultBodyCode, string? message = null)
{
    public bool IsSuccess { get; set; } = isSuccess;

    public ApiResultBodyCode Code { get; set; } = apiResultBodyCode;

    public string? Message { get; set; } = message ?? EnumExtensions.GetDisplayName(apiResultBodyCode);
}

public class ApiResponse<TData>(bool isSuccess, ApiResultBodyCode apiResultBodyCode, TData data, string? message = null)
    : ApiResponse(isSuccess, apiResultBodyCode, message)
    where TData : class
{
    public TData Data { get; set; } = data;
}
