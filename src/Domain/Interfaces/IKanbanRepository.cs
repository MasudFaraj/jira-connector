namespace Neusta.Jira.Connector.Domain.Interfaces;

using Neusta.Jira.Connector.Domain.Models;

public interface IKanbanRepository
{
    public Task<int?> GetBoardIdByJirakeyAsync(string jiraKey, CancellationToken cancellationToken);
    
    public Task<KanbanSprintsResponse?> 
        GetKanbanSprintsByJirakeyAsync(string jiraKey, string state, CancellationToken cancellationToken);
    
    public Task<List<KanbanOderSprintIssuesResponse.Issue>?> 
        GetSprintIssuesByJirakeyAsync(int sprintId, CancellationToken cancellationToken);
    
    public Task<List<KanbanOderSprintIssuesResponse.Issue>?> 
        GetKanbanIssuesByJirakeyAsync(string jiraKey, CancellationToken cancellationToken);
}