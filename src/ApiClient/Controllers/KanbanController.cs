namespace Neusta.Jira.Connector.ApiClient.Controllers;

using MediatR;
using Microsoft.AspNetCore.Mvc;
using Neusta.Jira.Connector.Application.Kanban.Models;
using Neusta.Jira.Connector.Application.Kanban.Queries;

public class KanbanController(ISender sender) : ApiControllerBase(sender)
{
    [HttpGet("TicketsCountByStatus")]
    public async Task<TicketsCountByStatusDto?> GetKanbanTicketsCountByStatusQuery(string jiraKey, CancellationToken cancellationToken)
    {
        GetKanbanTicketsCountByStatusQuery query = new()
        {
            JiraKey = jiraKey
        };
        return await this.Sender.Send(query, cancellationToken);
    }
    
    [HttpGet("TicketsCountByType")]
    public async Task<TicketsCountByTypeDto?> GetKanbanTicketsCountByTypeQuery(string jiraKey, CancellationToken cancellationToken)
    {
        GetKanbanTicketsCountByTypeQuery query = new()
        {
            JiraKey = jiraKey
        };
        return await this.Sender.Send(query, cancellationToken);
    }
    
    [HttpGet("VelocityOfLastThreeSprints")]
    public async Task<VelocityDto?> GetVelocityOfLastThreeSprints(string jiraKey, CancellationToken cancellationToken)
    {
        GetVelocityOfLastThreeSprintsQuery query = new()
        {
            JiraKey = jiraKey
        };
        return await this.Sender.Send(query, cancellationToken);
    }
}