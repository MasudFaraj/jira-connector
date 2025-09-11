namespace Neusta.Jira.Connector.Application.Kanban.Interfaces;

using Neusta.Jira.Connector.Application.Kanban.Models;

public interface IKanbanService
{
    public Task<TicketsCountByStatusDto?> GetKanbanTicketsCountByStatusAsync(string jiraKey, CancellationToken cancellationToken);

    public Task<TicketsCountByTypeDto?> GetKanbanTicketsCountByTypeAsync(string jiraKey, CancellationToken cancellationToken);

    public Task<decimal?> GetSprintVelocityAsync(int sprint, CancellationToken cancellationTok);

    public Task<VelocityDto?> GetVelocityOfLastThreeSprintsAsync(string jiraKey, CancellationToken cancellationToken);
}