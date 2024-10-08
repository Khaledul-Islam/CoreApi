﻿using Microsoft.Extensions.Options;

namespace Config.Settings;

public sealed record FileSetting
{
    public bool StoreFilesOnDatabase { get; init; } = true;
    public string SystemFilePath { get; init; } = "Resources";

}

public class FileSettingValidation : IValidateOptions<FileSetting>
{
    public ValidateOptionsResult Validate(string? name, FileSetting options)
    {
        if (!options.StoreFilesOnDatabase
            && string.IsNullOrEmpty(options.SystemFilePath))
        {
            return ValidateOptionsResult.Fail($"{nameof(FileSetting.StoreFilesOnDatabase)} set to false but {nameof(FileSetting.SystemFilePath)} is empty.");
        }

        return ValidateOptionsResult.Success;
    }
}