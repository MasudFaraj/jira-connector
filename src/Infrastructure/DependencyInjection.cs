namespace Neusta.Jira.Connector.Infrastructure;

using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Neusta.Jira.Connector.Domain.Interfaces;
using Neusta.Jira.Connector.Infrastructure.AppPersistence;
using Neusta.Jira.Connector.Infrastructure.AppPersistence.Interfaces;
using Neusta.Jira.Connector.Infrastructure.JiraApi;
using Neusta.Jira.Connector.Infrastructure.JiraApi.Interfaces;
using Neusta.Jira.Connector.Infrastructure.JiraApi.Repositories;

[ExcludeFromCodeCoverage]
public static class DependencyInjection
{
    private const string AppPersistenceDbConnectionName = "AppPersistenceDatabaseConnection";

    private static readonly ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
    {
        builder.AddConsole();
#if DEBUG
        builder.AddEventLog();
#endif
    });

    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration,
        IWebHostEnvironment environment)
    {
        AddAppPersistenceDbContext(services, configuration, environment.IsDevelopment());

        Assembly assembly = Assembly.GetExecutingAssembly();
        services.AddAutoMapper(assembly);
        services.AddScoped<IJiraClient, JiraClient>();
        services.AddScoped<IKanbanRepository, KanbanRepository>();
        services.AddHttpClient("Jira-Client", options =>
        {
            options.BaseAddress = new Uri(configuration["JiraSettings:BaseUrl"]);
            options.DefaultRequestHeaders.Add("Authorization", $"Bearer {configuration["JiraSettings:PersonalApiToken"]}");
        });
        return services;
    }

    private static void AddAppPersistenceDbContext(IServiceCollection services, IConfiguration configuration,
        bool isDevelopment)
    {
        const string defaultServerVersion = "10.5.12";
        string connectionString = configuration.GetConnectionString(AppPersistenceDbConnectionName);
        MariaDbServerVersion dbServerVersion =
            new(GetServerVersion(configuration, AppPersistenceDbConnectionName, defaultServerVersion));

        services.AddDbContextPool<IAppPersistenceDbContext, AppPersistenceDbContext>(options =>
        {
            options.EnableSensitiveDataLogging(isDevelopment);
            options.UseLoggerFactory(loggerFactory);
            options.UseMySql(connectionString, dbServerVersion, dbOptions =>
            {
                dbOptions.EnableRetryOnFailure();
            });
        });
    }

    public static void MigrateDatabase(this IApplicationBuilder app, IConfiguration configuration)
    {
        if (configuration.GetValue<bool>($"{AppPersistenceDbConnectionName}:AutomaticMigrationsEnabled"))
        {
            using IServiceScope scope = app.ApplicationServices.CreateScope();
            AppPersistenceDbContext dbContext =
                (AppPersistenceDbContext)scope.ServiceProvider.GetRequiredService<IAppPersistenceDbContext>();

            dbContext.Database.Migrate();
        }
    }

    private static Version GetServerVersion(IConfiguration configuration, string connectionName, string defaultVersion)
    {
        return new Version(configuration.GetValue($"{connectionName}:ServerVersion", defaultVersion));
    }
}
