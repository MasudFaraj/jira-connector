namespace Neusta.Jira.Connector.Infrastructure.JiraApi.Repositories;

using Neusta.Jira.Connector.Domain.Interfaces;
using Neusta.Jira.Connector.Domain.Models;
using Neusta.Jira.Connector.Infrastructure.JiraApi.Interfaces;
using Newtonsoft.Json;

public class KanbanRepository(IJiraClient jiraClient) : IKanbanRepository
{
    public async Task<int?> GetBoardIdByJirakeyAsync(string jiraKey, CancellationToken cancellationToken)
    {
        string boardApiUrl = $"rest/agile/1.0/board?projectKeyOrId={jiraKey}";
        HttpResponseMessage boardResponse = await jiraClient.SendRequestAsync(boardApiUrl);

        string boardData = await boardResponse.Content.ReadAsStringAsync(cancellationToken);
        BoardsResponse? boardResponseObj = JsonConvert.DeserializeObject<BoardsResponse>(boardData);

        return boardResponseObj?.Values?.FirstOrDefault()?.Id;
    }

    public async Task<KanbanSprintsResponse?> GetKanbanSprintsByJirakeyAsync(string jiraKey, string state,
        CancellationToken cancellationToken)
    {
        int? boardId = await this.GetBoardIdByJirakeyAsync(jiraKey, cancellationToken);
        
        string sprintApiUrl = $"rest/agile/1.0/board/{boardId}/sprint?state={state}";
        HttpResponseMessage sprintsResponse = await jiraClient.SendRequestAsync(sprintApiUrl);
        
        string sprintsData = await sprintsResponse.Content.ReadAsStringAsync(cancellationToken);
        KanbanSprintsResponse? sprintsResponseObj = JsonConvert.DeserializeObject<KanbanSprintsResponse>(sprintsData);
        
        return sprintsResponseObj;
    }

    public async Task<List<KanbanOderSprintIssuesResponse.Issue>?> GetSprintIssuesByJirakeyAsync(int sprintId,
        CancellationToken cancellationToken)
    {
        string sprintIssuesApiUrl = $"rest/agile/1.0/sprint/{sprintId}/issue?fields=customfield_10006,status,issuetype" +
                                    $"&jql=issuetype not in (subTaskIssueTypes())";
        HttpResponseMessage issuesResponse = await jiraClient.SendRequestAsync(sprintIssuesApiUrl);
        
        string issuesData = await issuesResponse.Content.ReadAsStringAsync(cancellationToken);
        KanbanOderSprintIssuesResponse? issuesResponseObj = 
            JsonConvert.DeserializeObject<KanbanOderSprintIssuesResponse>(issuesData);
        
        return issuesResponseObj?.Issues ?? new List<KanbanOderSprintIssuesResponse.Issue>();
    }

    public async Task<List<KanbanOderSprintIssuesResponse.Issue>?> GetKanbanIssuesByJirakeyAsync(string jiraKey,
        CancellationToken cancellationToken)
    {
        int? boardId = await this.GetBoardIdByJirakeyAsync(jiraKey, cancellationToken);

        string kanbanIssuesApiUrl = $"rest/agile/1.0/board/{boardId}/issue?fields=customfield_10006,status,issuetype" +
                                    $"&jql=issuetype not in (subTaskIssueTypes())";
        HttpResponseMessage issuesResponse = await jiraClient.SendRequestAsync(kanbanIssuesApiUrl);
        
        string issuesData = await issuesResponse.Content.ReadAsStringAsync(cancellationToken);
        KanbanOderSprintIssuesResponse? issuesResponseObj = 
            JsonConvert.DeserializeObject<KanbanOderSprintIssuesResponse>(issuesData);
        
        return issuesResponseObj?.Issues ?? new List<KanbanOderSprintIssuesResponse.Issue>();
    }
}