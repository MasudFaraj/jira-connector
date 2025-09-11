namespace Neusta.Jira.Connector.ApiClient;

using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Neusta.Jira.Connector.ApiClient.Services;
using Neusta.Jira.Connector.Infrastructure;
using Neusta.Jira.Connector.Application;
using Neusta.Jira.Connector.Application.Common.Interfaces;
using Neusta.NIP.Shared.WebApi.Authentication;
using Neusta.NIP.Shared.WebApi.ErrorHandling;
using Neusta.NIP.Shared.WebApi.Helpers;
using Neusta.NIP.Shared.WebApi.Interfaces;
using Neusta.NIP.Shared.WebApi.Swagger;
using Prometheus;
using Serilog;

[ExcludeFromCodeCoverage]
public class Program
{
    private const string DefaultCorsPolicy = "DefaultCorsPolicy";

    private const string SwaggerSearchPattern = "Neusta.Jira.*.xml";

    private const string SwaggerApiName = "User Service";
    private const string KeyVaultSection = "KeyVaultConfig";

    private const int Kb100 = 102400;
    private const int Mb10 = 10485780;

    public static async Task Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
        ConfigurationManager configuration = builder.Configuration;
        IServiceCollection services = builder.Services;

        Log.Logger = CustomLoggingExtensions.CreateCustomLogger(configuration);
        builder.Host.UseSerilog();

        Console.WriteLine(builder.Configuration.GetDebugView()); // Enables debugging of env vars.

        // Add services to the container.
        services.AddApplication(builder.Configuration);
        services.AddInfrastructure(builder.Configuration, builder.Environment);
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddTransient<ICustomErrorHandler, CustomErrorHandler>();

        Assembly assembly = Assembly.GetExecutingAssembly();

        services.AddSwaggerExamples(assembly);

        services.AddHealthChecks()
            .AddCheck("Self", () => HealthCheckResult.Healthy())
            .ForwardToPrometheus();

        services.AddHttpContextAccessor();

        // Customise default API behaviour
        services.Configure<ApiBehaviorOptions>(options =>
            options.SuppressModelStateInvalidFilter = true);

        services.AddResponseCaching(options =>
        {
            options.MaximumBodySize = Kb100;
            options.SizeLimit = Mb10;
            options.UseCaseSensitivePaths = true;
        });

        services.AddControllers(options =>
        {
            options.CacheProfiles.Add("Default15Minutes",
                new CacheProfile
                {
                    Duration = 900,
                    VaryByQueryKeys = new[] { "*" }
                });
        });

        AddDefaultCors(builder.Services);

        services.AddAzureAdUserAuthentication(configuration);
        services.AddAzureAdAppAuthentication(configuration);
        services.AddDownstreamWebApiOptions(configuration);
        services.AddSharedAuthorization();

        services.AddCustomSwagger(SwaggerSearchPattern);

        WebApplication app = builder.Build();

        app.MigrateDatabase(builder.Configuration);

        //Be aware of middleware order
        app.UseSerilogRequestLogging();

        if (!app.Environment.IsProduction())
        {
            app.UseStaticFiles();
            app.UseSwaggerUI(c => c.InjectStylesheet("/Swagger/custom.css"));
            app.UseSwagger(SwaggerApiName);
        }

        app.UseExceptionHandler("/Error");
        app.UseRouting();
        app.UseHttpMetrics();
        app.UseResponseCaching();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseCustomClaimMiddleware(); //Needs to be after UseAuthentication and UseAuthorization
        app.UseCors(DefaultCorsPolicy);

        app.MapMetrics();
        app.MapControllers();
        app.MapHealthChecks("/health",
            new HealthCheckOptions { ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse });

        try
        {
            Log.Information("Starting web host");
            await app.RunAsync();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Host terminated unexpectedly");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    private static void AddDefaultCors(IServiceCollection services)
    {
        services.AddCors(
            options =>
            {
                options.AddPolicy(
                    DefaultCorsPolicy,
                    policyBuilder =>
                    {
                        policyBuilder
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .AllowAnyOrigin()
                            .WithExposedHeaders("Content-Disposition")
                            .SetIsOriginAllowed(_ => true);
                    });
            });
    }
}
