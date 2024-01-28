using Contracts.Files;
using Data.Context;
using Data.Providers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.Entities.Files;
using Utilities.Exceptions;

namespace Services.Files;

public class FileOnDatabaseService(ApplicationDbContext context) : Repository<FileModel, int>(context), IFileService
{
    public async Task<int> StoreFileAsync(IFormFile formFile, string description, CancellationToken cancellationToken)
    {
        if (formFile is null)
        {
            throw new BadRequestException("File is empty object");
        }
        var fileName = Path.GetFileNameWithoutExtension(formFile.FileName);
        var extension = Path.GetExtension(formFile.FileName);
        var fileModel = new FileModel
        {
            FileType = formFile.ContentType,
            Extension = extension,
            Name = fileName,
            Description = description,
            CreatedOn = DateTime.Now
        };

        using (var dataStream = new MemoryStream())
        {
            await formFile.CopyToAsync(dataStream,cancellationToken);
            fileModel.Data = dataStream.ToArray();
        }

        await AddAsync(fileModel, cancellationToken);
        await SaveChangesAsync(cancellationToken);
        return fileModel.Id;
    }

    public async Task<List<int>> StoreFilesAsync(List<IFormFile> formFiles, string description, CancellationToken cancellationToken)
    {

        if (formFiles is null)
        {
            throw new BadRequestException("Files is empty object");
        }
        var fileModels = new List<FileModel>();

        foreach (var formFile in formFiles)
        {
            var fileName = Path.GetFileNameWithoutExtension(formFile.FileName);
            var extension = Path.GetExtension(formFile.FileName);
            var fileModel = new FileModel
            {
                FileType = formFile.ContentType,
                Extension = extension,
                Name = fileName,
                Description = description,
                CreatedOn = DateTime.Now
            };

            using (var dataStream = new MemoryStream())
            {
                await formFile.CopyToAsync(dataStream,cancellationToken);
                fileModel.Data = dataStream.ToArray();
            }

            fileModels.Add(fileModel);
        }

        await AddRangeAsync(fileModels, cancellationToken);
        await SaveChangesAsync(cancellationToken);
        return fileModels.Select(a => a.Id).ToList();
    }

    public async Task<FileStreamResult> GetFileByIdAsync(int id, CancellationToken cancellationToken)
    {
        var entity = await GetByIdAsync(id, cancellationToken);
        if (entity == null || entity.Data == null)
        {
            var error = entity == null ? nameof(entity) : nameof(entity.Data);
            throw new NullReferenceException($"{error} is null");
        }
        // Create FileStreamResult
        var fileStreamResult = new FileStreamResult(new MemoryStream(entity.Data!), entity.FileType);
        fileStreamResult.FileDownloadName = entity.Name + entity.Extension;

        return fileStreamResult;
    }

    public async Task DeleteFileAsync(int id, CancellationToken cancellationToken)
    {
        var entity = await GetByIdAsync(id,cancellationToken);
        if (entity == null || entity.Data == null)
        {
            var error = entity == null ? nameof(entity) : nameof(entity.Data);
            throw new NullReferenceException($"{error} is null");
        }
        await Remove(entity);
    }
}
