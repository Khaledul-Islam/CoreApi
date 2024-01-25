using Microsoft.Extensions.Options;
using Models.Enums;

namespace Config.Settings;

public sealed record ConnectionStrings
{
    public string? MySql { get; init; }
    public string? SqlServer { get; init; }

    public string? Postgres { get; init; }
}

public class ConnectionStringsValidation : IValidateOptions<ConnectionStrings>
{
    public ValidateOptionsResult Validate(string? name, ConnectionStrings options)
    {
        if (string.IsNullOrEmpty(options.SqlServer) && string.IsNullOrEmpty(options.Postgres) && string.IsNullOrEmpty(options.MySql))
        {
            return ValidateOptionsResult.Fail($"{nameof(ConnectionStrings)} can not be empty (Choose between {nameof(DatabaseProvider.Postgres)} or {nameof(DatabaseProvider.SqlServer)} or {nameof(DatabaseProvider.MySql)})");
        }

        return ValidateOptionsResult.Success;
    }
}