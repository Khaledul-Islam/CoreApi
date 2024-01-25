using Models.Enums;

namespace Utilities.Response;

public class PagedResponse<TData>(int pageNumber, int pageSize, bool isSuccess, ApiResultBodyCode apiResultBodyCode,
        TData data, Uri firstPage, Uri lastPage, Uri nextPage, Uri previousPage, string? message = null)
    : ApiResponse<TData>(isSuccess, apiResultBodyCode, data, message)
    where TData : class
{
    public int PageNumber { get; set; } = pageNumber;

    public int PageSize { get; set; } = pageSize;

    public Uri FirstPage { get; set; } = firstPage;

    public Uri LastPage { get; set; } = lastPage;

    public int TotalPages { get; set; }

    public int TotalRecords { get; set; }

    public Uri NextPage { get; set; } = nextPage;

    public Uri PreviousPage { get; set; } = previousPage;
}
