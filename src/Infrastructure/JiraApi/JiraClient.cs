namespace Neusta.Jira.Connector.Infrastructure.JiraApi;

using Microsoft.Extensions.Logging;
using Neusta.Jira.Connector.Infrastructure.JiraApi.Interfaces;

public class JiraClient : IJiraClient
{
    private readonly HttpClient httpClient;
    private readonly ILogger<JiraClient> logger;

    public JiraClient(IHttpClientFactory httpClientFactory, ILogger<JiraClient> logger)
    {
        this.httpClient = httpClientFactory.CreateClient("Jira-Client");
        this.logger = logger;
    }

    public async Task<HttpResponseMessage> SendRequestAsync(string uri)
    {
        HttpResponseMessage response;

        try
        {
            HttpRequestMessage requestMessage = new(HttpMethod.Get, uri);
            response = await this.httpClient.SendAsync(requestMessage);
        }
        catch (Exception e)
        {
            this.logger.LogError(e.Message);
            throw;
        }

        response.EnsureSuccessStatusCode();

        return response;
    }
}