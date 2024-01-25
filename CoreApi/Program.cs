using Config.Settings;
using FluentValidation.AspNetCore;
using Serilog;
using ServiceExtensions.ApplicationBuilder;
using ServiceExtensions.Serilog;
using ServiceExtensions.ServiceCollection;
using ServiceExtensions.Swagger;
using Utilities.Validations;


var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();

var services = builder.Services;

var configuration = builder.Configuration;

var applicationSettings = configuration
                          .GetSection(nameof(ApplicationSettings))
                          .Get<ApplicationSettings>();

const string CORS_POLICY = "CorsPolicy";

SerilogExtensions.Register(applicationSettings);

// Add services to the container.
ConfigureServices(services);

var app = builder.Build();

// Configure the HTTP request pipeline.
ConfigurePipeline(app);

app.Run();


void ConfigureServices(IServiceCollection services)
{
    services.AddConfiguration(configuration);

    // Database services
    services.AddDbContext(applicationSettings.DatabaseSetting);

    services.AutoMapperRegistration();

    services.AddJwtAuthentication(applicationSettings.JwtSetting);

    services.AddApplicationDependencyRegistration(applicationSettings);

    services.AddSwagger();

    services.AddHttpContextAccessor();

    // Add service and create Policy with options
    services.AddCors(options =>
    {
        options.AddPolicy(name: CORS_POLICY,
            builder => builder
                      .AllowAnyMethod()
                      .AllowAnyHeader()
                      .SetIsOriginAllowed(origin => true)  // Allow any origin
                      .AllowCredentials());                // Allow credentials
    });

    services.AddMvc();
    services.AddControllers()
            .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<TokenRequestValidator>());
}

void ConfigurePipeline(IApplicationBuilder app)
{
    app.UseGlobalExceptionHandler();

    app.UseRouting();

    // Enable Cors
    app.UseCors(CORS_POLICY);

    app.UseAuthentication();
    app.UseAuthorization();

    app.UseEndpoints(endpoints =>
    {
        endpoints.MapControllers();
    });

    app.RegisterSwaggerMidlleware();
}