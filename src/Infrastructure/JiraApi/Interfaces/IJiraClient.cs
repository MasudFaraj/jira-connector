namespace Neusta.Jira.Connector.Infrastructure.JiraApi.Interfaces;

public interface IJiraClient
{
    Task<HttpResponseMessage> SendRequestAsync(string uri);
}