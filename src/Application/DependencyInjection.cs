namespace Neusta.Jira.Connector.Application;

using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;
using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Neusta.Jira.Connector.Application.Kanban.Interfaces;
using Neusta.Jira.Connector.Application.Kanban.Services;
using Neusta.Jira.Connector.Application.Sprint.Interfaces;
using Neusta.Jira.Connector.Application.Sprint.Services;
using Neusta.NIP.Shared.WebApi.Helpers;

[ExcludeFromCodeCoverage]
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        Assembly assembly = Assembly.GetExecutingAssembly();
        services.AddAutoMapper(assembly);
        services.AddValidatorsFromAssembly(assembly);
        services.AddCustomBehaviours(assembly);
        services.AddTransient<IActiveSprintService, ActiveSprintService>();
        services.AddTransient<IKanbanService, KanbanService>();
        services.AddHttpClient(
            "printapi",
            c =>
            {
                c.BaseAddress = new Uri(configuration.GetSection("PrintApi:BaseUrl").Value);
                c.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            });

        AddLocalDependencies(services, configuration);

        return services;
    }

    private static void AddLocalDependencies(IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<List<string>>()
            .Configure(options => configuration.Bind("AbsenceTypesForNoExtraHoursOptions", options));
    }
}
