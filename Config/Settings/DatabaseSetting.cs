﻿using Microsoft.Extensions.Options;
using Models.Enums;

namespace Config.Settings;

public sealed record DatabaseSetting
{
    public ConnectionStrings ConnectionStrings { get; init; } = null!;
    public DatabaseProvider DatabaseProvider { get; init; } = DatabaseProvider.Postgres;
}

public class DatabaseSettingValidation : IValidateOptions<DatabaseSetting>
{
    public ValidateOptionsResult Validate(string? name, DatabaseSetting options)
    {
        if (options.DatabaseProvider == DatabaseProvider.Postgres
            && string.IsNullOrEmpty(options.ConnectionStrings.Postgres))
        {
            return ValidateOptionsResult.Fail($"{nameof(DatabaseProvider)} set to {nameof(DatabaseProvider.Postgres)} but {nameof(DatabaseProvider.Postgres)} ConnectionString is empty. Note default {nameof(DatabaseProvider)} is {nameof(DatabaseProvider.Postgres)}.(Occures when {nameof(DatabaseProvider)} is not set)");
        }

        if (options.DatabaseProvider == DatabaseProvider.SqlServer
            && string.IsNullOrEmpty(options.ConnectionStrings.SqlServer))
        {
            return ValidateOptionsResult.Fail($"{nameof(DatabaseProvider)} set to {nameof(DatabaseProvider.SqlServer)} but {nameof(DatabaseProvider.SqlServer)} ConnectionString is empty.");
        }

        if (options.DatabaseProvider == DatabaseProvider.MySql
            && string.IsNullOrEmpty(options.ConnectionStrings.MySql))
        {
            return ValidateOptionsResult.Fail($"{nameof(DatabaseProvider)} set to {nameof(DatabaseProvider.MySql)} but {nameof(DatabaseProvider.MySql)} ConnectionString is empty.");
        }

        return ValidateOptionsResult.Success;
    }
}
