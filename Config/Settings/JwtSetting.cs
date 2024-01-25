using Microsoft.Extensions.Options;

namespace Config.Settings;

public sealed record JwtSetting
{
    public string SecretKey { get; init; } = "B374A26A71490437AA024E4FADD5B497FDFF1A8EA6FF12F6FB65AF2720B59CCFF";

    public string EncryptKey { get; init; } = "16CharEncryptKey";

    public string Issuer { get; init; } = string.Empty;

    public string Audience { get; init; } = string.Empty;

    public int NotBeforeMinutes { get; init; } = 0;

    public int AccessTokenExpirationDays { get; init; } = 1;

    public int RefreshTokenExpirationDays { get; init; } = 7;
}

public class JwtSettingValidation : IValidateOptions<JwtSetting>
{
    public ValidateOptionsResult Validate(string name, JwtSetting options)
    {
        if (options.NotBeforeMinutes < 0)
        {
            return ValidateOptionsResult.Fail($"{nameof(JwtSetting.NotBeforeMinutes)} can not be less than 0.");

        }
        if (options.AccessTokenExpirationDays < 1)
        {
            return ValidateOptionsResult.Fail($"{nameof(JwtSetting.AccessTokenExpirationDays)} can not be less tha 1.");
        }

        if (options.RefreshTokenExpirationDays < 1)
        {
            return ValidateOptionsResult.Fail($"{nameof(JwtSetting.RefreshTokenExpirationDays)} can not be less tha 1.");
        }

        return ValidateOptionsResult.Success;
    }
}