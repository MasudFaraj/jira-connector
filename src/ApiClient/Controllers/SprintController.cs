namespace Neusta.Jira.Connector.ApiClient.Controllers;

using MediatR;
using Microsoft.AspNetCore.Mvc;
using Neusta.Jira.Connector.Application.Kanban.Models;
using Neusta.Jira.Connector.Application.Sprint.Models;
using Neusta.Jira.Connector.Application.Sprint.Queries;

public class SprintController(ISender sender) : ApiControllerBase(sender)
{
    [HttpGet("activeSprint")]
    public async Task<SprintDto?> GetActiveSprintDataQuery(string jiraKey, CancellationToken cancellationToken)
    {
        GetActiveSprintDataQuery query = new()
        {
            JiraKey = jiraKey
        };
        return await this.Sender.Send(query, cancellationToken);
    }
    
    [HttpGet("TicketsCountByStatus")]
    public async Task<TicketsCountByStatusDto?> GetActiveSprintTicketsCountByStatusQuery(string jiraKey, CancellationToken cancellationToken)
    {
        GetActiveSprintTicketsCountByStatusQuery query = new()
        {
            JiraKey = jiraKey
        };
        return await this.Sender.Send(query, cancellationToken);
    }
    
    [HttpGet("StoryPointsSumByStatus")]
    public async Task<StoryPointsSumByStatusDto?> GetActiveSprintStoryPointsSumByStatusQuery(string jiraKey, CancellationToken cancellationToken)
    {
        GetActiveSprintStoryPointsSumByStatusQuery query = new()
        {
            JiraKey = jiraKey
        };
        return await this.Sender.Send(query, cancellationToken);
    }
}