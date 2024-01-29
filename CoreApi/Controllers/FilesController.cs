using Contracts.Files;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Enums;
using Utilities.Response;

namespace CoreApi.Controllers;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class FilesController(IFileService fileService) : BaseController
{
    [HttpPost]
    public async Task<ApiResponse<object>> StoreFile(IFormFile formFile, string description, CancellationToken cancellationToken)
    {
        return new ApiResponse<object>(true, ApiResultBodyCode.Success, new { Id = await fileService.StoreFileAsync(formFile, description, cancellationToken) });
    }

    [HttpGet("{id:int:min(1)}")]
    public async Task<ActionResult> GetFileById(int id, CancellationToken cancellationToken)
    {
        var file = await fileService.GetFileByIdAsync(id, cancellationToken);
        return File(file.FileStream, file.ContentType, file.FileDownloadName);
    }

    [HttpDelete("{id:int:min(1)}")]
    public async Task<ApiResponse> DeleteFile(int id, CancellationToken cancellationToken)
    {
        await fileService.DeleteFileAsync(id, cancellationToken);
        return new ApiResponse(true, ApiResultBodyCode.Success);
    }
}