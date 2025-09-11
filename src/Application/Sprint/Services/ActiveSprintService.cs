namespace Neusta.Jira.Connector.Application.Sprint.Services;

using AutoMapper;
using Neusta.Jira.Connector.Application.Kanban.Models;
using Neusta.Jira.Connector.Application.Sprint.Interfaces;
using Neusta.Jira.Connector.Application.Sprint.Models;
using Neusta.Jira.Connector.Domain.Interfaces;
using Neusta.Jira.Connector.Domain.Models;
using Newtonsoft.Json;

public class ActiveSprintService(IKanbanRepository kanbanRepository, IMapper mapper) : IActiveSprintService
{
   public async Task<SprintDto?> GetActiveSprintDataByJiraKeyAsync(string jiraKey, CancellationToken cancellationToken)
    {
        KanbanSprintsResponse? sprintsResponseObj = await kanbanRepository.GetKanbanSprintsByJirakeyAsync(jiraKey, "active", cancellationToken);
        KanbanSprintsResponse.Sprint? activeSprint = sprintsResponseObj?.Values?.LastOrDefault();
        return activeSprint != null
            ? mapper.Map<SprintDto>(activeSprint)
            : null;
    }
    public async Task<List<KanbanOderSprintIssuesResponse.Issue>?> GetActiveSprintIssuesByJirakeyAsync(string jiraKey, CancellationToken cancellationToken)
    {
        KanbanSprintsResponse? sprintResponseObj = await kanbanRepository.GetKanbanSprintsByJirakeyAsync(jiraKey, "active", cancellationToken);
        KanbanSprintsResponse.Sprint? activeSprint = sprintResponseObj?.Values?.LastOrDefault();
        return await kanbanRepository.GetSprintIssuesByJirakeyAsync(activeSprint.Id, cancellationToken);
    }
    public async Task<TicketsCountByStatusDto?> GetActiveSprintTicketsCountByStatusAsync(string jiraKey, CancellationToken cancellationToken)
    {
        List<KanbanOderSprintIssuesResponse.Issue>? issues = await this.GetActiveSprintIssuesByJirakeyAsync(jiraKey, cancellationToken);
        int todoTicketsCount = issues.Count(i => i.Fields?.Status?.StatusCategory?.Name == "To Do");
        int inProgressTicketsCount = issues.Count(i => i.Fields?.Status?.StatusCategory?.Name == "In Progress");
        int doneTicketsCount = issues.Count(i => i.Fields?.Status?.StatusCategory?.Name == "Done");
        return new TicketsCountByStatusDto
        {
            Total = issues.Count,
            Todo = todoTicketsCount,
            InProgress = inProgressTicketsCount,
            Done = doneTicketsCount,
        };
    }
    public async Task<StoryPointsSumByStatusDto?> GetActiveSprintStoryPointsSumByStatusAsync(string jiraKey, CancellationToken cancellationToken)
    {
        List<KanbanOderSprintIssuesResponse.Issue>? issues = await this.GetActiveSprintIssuesByJirakeyAsync(jiraKey, cancellationToken);
        
        decimal storyPointsPlanned = issues.Sum(i => i.Fields?.StoryPoints ?? 0);
        decimal storyPointsTodo = issues.Where(i => i.Fields?.Status?.StatusCategory?.Name == "To Do")
            .Sum(i => i.Fields?.StoryPoints ?? 0);
        decimal storyPointsInProgress = issues.Where(i => i.Fields?.Status?.StatusCategory?.Name == "In Progress")
            .Sum(i => i.Fields?.StoryPoints ?? 0);
        decimal storyPointsDone = issues.Where(i => i.Fields?.Status?.StatusCategory?.Name == "Done")
            .Sum(i => i.Fields?.StoryPoints ?? 0);

        return new StoryPointsSumByStatusDto()
        {
            Planned = storyPointsPlanned,
            Todo = storyPointsTodo,
            InProgress = storyPointsInProgress,
            Done = storyPointsDone
        };
    }
}