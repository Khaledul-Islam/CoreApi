using Config.Settings;
using FluentValidation.AspNetCore;
using Serilog;
using ServiceExtensions.ApplicationBuilder;
using ServiceExtensions.Serilog;
using ServiceExtensions.ServiceCollection;
using ServiceExtensions.Swagger;
using Services.SignalRHubs;
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


void ConfigureServices(IServiceCollection serviceCollection)
{
    serviceCollection.AddConfiguration(configuration);

    // Database services
    serviceCollection.AddDbContext(applicationSettings!.DatabaseSetting);

    // Auto Mapper services
    serviceCollection.AutoMapperRegistration();

    // JWT services
    serviceCollection.AddJwtAuthentication(applicationSettings.JwtSetting);

    serviceCollection.AddApplicationDependencyRegistration(applicationSettings);

    // Swagger services
    serviceCollection.AddSwagger();

    serviceCollection.AddHttpContextAccessor();
    serviceCollection.AddCurrentPrincipleAccessor();

    // Add service and create Policy with options
    serviceCollection.AddCors(options =>
    {
        options.AddPolicy(name: CORS_POLICY,
            corsPolicyBuilder => corsPolicyBuilder
                      .AllowAnyMethod()
                      .AllowAnyHeader()
                      .SetIsOriginAllowed(_ => true)  // Allow any origin
                      .AllowCredentials());                // Allow credentials
    });

    serviceCollection.AddMvc();
    serviceCollection.AddControllers()
            .AddFluentValidation(fv => fv
                .RegisterValidatorsFromAssemblyContaining<TokenRequestValidator>());

    // SignalR services
    serviceCollection.AddSignalRRegistration();
    // Quartz services
    serviceCollection.AddQuartzRegistration(applicationSettings!.DatabaseSetting);
}

void ConfigurePipeline(IApplicationBuilder applicationBuilder)
{
    applicationBuilder.UseGlobalExceptionHandler();

    applicationBuilder.UseRouting();

    // Enable Cors
    applicationBuilder.UseCors(CORS_POLICY);

    applicationBuilder.UseAuthentication();
    applicationBuilder.UseAuthorization();


    applicationBuilder.UseEndpoints(endpoints =>
    {
        endpoints.MapControllers();
        // ----- SignalR -----
        endpoints.MapHub<NotificationHub>($"/hub{HubRoutes.Notification}");
    });

    applicationBuilder.RegisterSwaggerMidlleware();
}