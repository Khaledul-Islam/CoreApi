using Config.Settings;
using Contracts.Files;
using Data.Context;
using Data.Providers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.Entities.Files;
using Utilities.Exceptions;

namespace Services.Files;

public class FileOnFileSystemService(ApplicationDbContext context, ApplicationSettings applicationSettings) : Repository<FileModel, int>(context), IFileService
{
    public async Task<int> StoreFileAsync(IFormFile formFile, string description, CancellationToken cancellationToken)
    {
        if (formFile is null)
        {
            throw new BadRequestException("File is empty object");
        }

        var basePath = Path.Combine(applicationSettings.FileSetting.SystemFilePath);
        var basePathExists = Directory.Exists(basePath);
        if (basePathExists is false)
        {
            Directory.CreateDirectory(basePath);
        }
        var fileName = Path.GetFileNameWithoutExtension(formFile.FileName);
        var filePath = Path.Combine(basePath, formFile.FileName);
        var extension = Path.GetExtension(formFile.FileName);
        if (File.Exists(filePath))
        {
            fileName = $"{fileName}_{DateTime.UtcNow.Ticks}";
            filePath = Path.Combine(basePath, fileName);
        }

        await using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await formFile.CopyToAsync(stream,cancellationToken);
        }

        var fileModel = new FileModel()
        {
            Name = fileName,
            FileType = formFile.ContentType,
            Extension = extension,
            Description = description,
            FilePath = filePath,
            CreatedOn = DateTime.Now
        };

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
            var basePath = Path.Combine("Resources");
            var basePathExists = Directory.Exists(basePath);
            if (basePathExists is false)
            {
                Directory.CreateDirectory(basePath);
            }

            var fileName = Path.GetFileNameWithoutExtension(formFile.FileName);
            var filePath = Path.Combine(basePath, formFile.FileName);
            var extension = Path.GetExtension(formFile.FileName);
            if (File.Exists(filePath))
            {
                fileName = $"{fileName}_{DateTime.UtcNow.Ticks}";
                filePath = Path.Combine(basePath, fileName);
            }

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await formFile.CopyToAsync(stream);
            }

            var fileModel = new FileModel
            {
                Name = fileName,
                FileType = formFile.ContentType,
                Extension = extension,
                Description = description,
                FilePath = filePath,
                CreatedOn = DateTime.Now
            };

            fileModels.Add(fileModel);
        }

        await AddRangeAsync(fileModels, cancellationToken);
        await SaveChangesAsync(cancellationToken);
        return fileModels.Select(a => a.Id).ToList();
    }

    public async Task<FileStreamResult> GetFileByIdAsync(int id, CancellationToken cancellationToken)
    {
        var entity = await GetByIdAsync(id, cancellationToken);
        if (entity ==null || entity.FilePath == null)
        {
            var error = entity == null ? nameof(entity) : nameof(entity.FilePath);
            throw new  NullReferenceException($"{error} is null");
        }
        var memory = new MemoryStream();
        await using (var stream = new FileStream(entity.FilePath!, FileMode.Open))
        {
            await stream.CopyToAsync(memory,cancellationToken);
        }
        memory.Position = 0;

        // Create FileStreamResult
        var fileStreamResult = new FileStreamResult(memory, entity.FileType);
        fileStreamResult.FileDownloadName = entity.Name + entity.Extension;

        return fileStreamResult;
    }

    public async Task DeleteFileAsync(int id, CancellationToken cancellationToken)
    {
        var entity = await GetByIdAsync(id, cancellationToken);
        if (entity == null || entity.FilePath == null)
        {
            var error = entity == null ? nameof(entity) : nameof(entity.FilePath);
            throw new NullReferenceException($"{error} is null");
        }
        if (File.Exists(entity.FilePath))
        {
            File.Delete(entity.FilePath);
        }
        await Remove(entity);
    }
}
