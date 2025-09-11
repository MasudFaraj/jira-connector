namespace Neusta.Jira.Connector.Application.Sprint.Interfaces;

using Neusta.Jira.Connector.Application.Kanban.Models;
using Neusta.Jira.Connector.Application.Sprint.Models;
using Neusta.Jira.Connector.Domain.Models;

public interface IActiveSprintService
{
    public Task<SprintDto?> GetActiveSprintDataByJiraKeyAsync(string jiraKey, CancellationToken cancellationToken);

    public Task<TicketsCountByStatusDto?>
        GetActiveSprintTicketsCountByStatusAsync(string jiraKey, CancellationToken cancellationToken);

    public Task<StoryPointsSumByStatusDto?> GetActiveSprintStoryPointsSumByStatusAsync(string jiraKey,
        CancellationToken cancellationToken);

    public Task<List<KanbanOderSprintIssuesResponse.Issue>?> GetActiveSprintIssuesByJirakeyAsync(string jiraKey,
        CancellationToken cancellationToken);
}