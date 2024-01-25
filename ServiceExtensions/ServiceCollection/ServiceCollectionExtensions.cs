using Config.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text;
using Contracts.Crypto;
using Models.Enums;
using Data.Context;
using Microsoft.EntityFrameworkCore;
using ServiceExtensions.Mapper;
using Data.Providers;
using Contracts.Tokens;
using Contracts.Users;
using Services.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Services.Users;
using Utilities.Exceptions;
using Models.Entities.Identity;
using Services.Crypto;
using Contracts.Files;
using Services.Files;

namespace ServiceExtensions.ServiceCollection
{
    public static class ServiceCollectionExtensions
    {
        public static void AddConfiguration(this IServiceCollection services, ConfigurationManager configuration)
        {
            services.AddSingleton(configuration.GetSection(nameof(ApplicationSettings)).Get<ApplicationSettings>()!);
            services.AddSingleton<IValidateOptions<ApplicationSettings>, ApplicationSettingsValidation>();
            services.AddOptions<ApplicationSettings>()
                    .Bind(configuration.GetSection(nameof(ApplicationSettings)))
                    .ValidateDataAnnotations()
                    .ValidateOnStart();

            services.AddSingleton<IValidateOptions<DatabaseSetting>, DatabaseSettingValidation>();
            services.AddOptions<DatabaseSetting>()
                    .Bind(configuration.GetSection($"{nameof(ApplicationSettings)}:{nameof(DatabaseSetting)}"))
                    .ValidateDataAnnotations()
                    .ValidateOnStart();

            services.AddSingleton<IValidateOptions<ConnectionStrings>, ConnectionStringsValidation>();
            services.AddOptions<ConnectionStrings>()
                    .Bind(configuration.GetSection($"{nameof(ApplicationSettings)}:{nameof(DatabaseSetting)}:{nameof(ConnectionStrings)}"))
                    .ValidateDataAnnotations()
                    .ValidateOnStart();

            services.AddSingleton<IValidateOptions<IdentitySetting>, IdentitySettingValidation>();
            services.AddOptions<IdentitySetting>()
                    .Bind(configuration.GetSection($"{nameof(ApplicationSettings)}:{nameof(IdentitySetting)}"))
                    .ValidateDataAnnotations()
                    .ValidateOnStart();

            services.AddSingleton<IValidateOptions<JwtSetting>, JwtSettingValidation>();
            services.AddOptions<JwtSetting>()
                    .Bind(configuration.GetSection($"{nameof(ApplicationSettings)}:{nameof(JwtSetting)}"))
                    .ValidateDataAnnotations()
                    .ValidateOnStart();

            services.AddSingleton<IValidateOptions<LogSetting>, LogSettingValidation>();
            services.AddOptions<LogSetting>()
                    .Bind(configuration.GetSection($"{nameof(ApplicationSettings)}:{nameof(LogSetting)}"))
                    .ValidateDataAnnotations()
                    .ValidateOnStart();

            services.AddSingleton<IValidateOptions<MailSetting>, MailSettingValidation>();
            services.AddOptions<MailSetting>()
                    .Bind(configuration.GetSection($"{nameof(ApplicationSettings)}:{nameof(MailSetting)}"))
                    .ValidateDataAnnotations()
                    .ValidateOnStart();
        }

        public static void AddDbContext(this IServiceCollection services, DatabaseSetting databaseSetting)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                switch (databaseSetting.DatabaseProvider)
                {
                    case DatabaseProvider.Postgres:
                        options
                            .UseNpgsql(databaseSetting.ConnectionStrings.Postgres!);
                        break;

                    case DatabaseProvider.SqlServer:
                        options
                            .UseSqlServer(databaseSetting.ConnectionStrings.SqlServer);
                        break;

                    case DatabaseProvider.MySql:
                        options
                            .UseMySQL(databaseSetting.ConnectionStrings.MySql);
                        break;

                    default:
                        throw new Exception("Database provider not found.");
                }
            });
        }

        public static void AutoMapperRegistration(this IServiceCollection services)
        {
            services.InitializeAutoMapper();
        }
        public static void AddJwtAuthentication(this IServiceCollection services, JwtSetting jwtSettings)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                var secretKey = Encoding.UTF8.GetBytes(jwtSettings.SecretKey);
                var encryptionKey = Encoding.UTF8.GetBytes(jwtSettings.EncryptKey);

                var validationParameters = new TokenValidationParameters
                {
                    ClockSkew = TimeSpan.Zero, // Default : 5 min
                    RequireSignedTokens = true,

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(secretKey),

                    RequireExpirationTime = true,
                    ValidateLifetime = true,

                    ValidateAudience = true, // Default : false
                    ValidAudience = jwtSettings.Audience,

                    ValidateIssuer = true, // Default : false
                    ValidIssuer = jwtSettings.Issuer,

                    TokenDecryptionKey = new SymmetricSecurityKey(encryptionKey)
                };

                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = validationParameters;
                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        if (context.Exception is not null)
                        {
                            throw new TokenExpiredException();
                        }

                        return Task.CompletedTask;
                    },

                    OnTokenValidated = async context =>
                    {
                        var userService = context.HttpContext.RequestServices.GetRequiredService<IUserService>();

                        var claimsIdentity = context.Principal?.Identity as ClaimsIdentity;
                        if (claimsIdentity?.Claims.Any() is not true)
                        {
                            context.Fail("This token has no claims.");
                        }

                        var userIdClaim = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier);
                        if (userIdClaim is null)
                        {
                            context.Fail("This token has no user id claim.");
                        }

                        var user = await userService.GetByIdAsync(Convert.ToInt32(userIdClaim.Value),CancellationToken.None);
                        if (user is null)
                        {
                            context.Fail("User does not exist.");
                        }

                        if (!user.IsActive)
                        {
                            context.Fail("User is not active.");
                        }
                    },

                    OnChallenge = context =>
                    {
                        if (context.AuthenticateFailure is not null)
                        {
                            throw new AuthenticationException(additionalData: context.AuthenticateFailure);
                        }

                        throw new ForbiddenException();
                    },
                };
            });
        }

        public static void AddApplicationDependencyRegistration(this IServiceCollection services, ApplicationSettings applicationSettings)
        {
            // Data Services
            AddDataProvidersDependencyRegistration(services);

            // Domain Services
            services.AddScoped(typeof(IAuthTokenService), typeof(AuthTokenService));
            services.AddTransient<IUserService, UserService>();
            services.AddScoped<IPasswordHasherService<User>, PasswordHasherService<User>>();

            services.AddScoped(typeof(IFileService),
                applicationSettings.DatabaseSetting.StoreFilesOnDatabase
                    ? typeof(FileOnDatabaseService)
                    : typeof(FileOnFileSystemService));
        }

        private static void AddDataProvidersDependencyRegistration(IServiceCollection services)
        {
            services.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));
        }

    }
}
