namespace Neusta.Jira.Connector.Application.Kanban.Services;

using Microsoft.Extensions.Logging;
using Neusta.Jira.Connector.Application.Kanban.Interfaces;
using Neusta.Jira.Connector.Application.Kanban.Models;
using Neusta.Jira.Connector.Domain.Interfaces;
using Neusta.Jira.Connector.Domain.Models;

public class KanbanService(IKanbanRepository kanbanRepository, ILogger<KanbanService> logger) : IKanbanService
{
    public async Task<TicketsCountByStatusDto?> GetKanbanTicketsCountByStatusAsync(string jiraKey, CancellationToken cancellationToken)
    {
        List<KanbanOderSprintIssuesResponse.Issue>? issues = await kanbanRepository.GetKanbanIssuesByJirakeyAsync(jiraKey, cancellationToken);
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
    public async Task<TicketsCountByTypeDto?> GetKanbanTicketsCountByTypeAsync(string jiraKey, CancellationToken cancellationToken)
    {
        List<KanbanOderSprintIssuesResponse.Issue>? issues = await kanbanRepository.GetKanbanIssuesByJirakeyAsync(jiraKey, cancellationToken);
        int storyTicketsCount = issues.Count(issue => issue.Fields?.IssueType?.Name == "Story");
        int taskTicketsCount = issues.Count(issue => issue.Fields?.IssueType?.Name == "Task");
        int bugTicketsCount = issues.Count(issue => issue.Fields?.IssueType?.Name == "Bug");
        return new TicketsCountByTypeDto()
        {
            Story = storyTicketsCount, Task = taskTicketsCount, Bug = bugTicketsCount
        };
    }
    public async Task<decimal?> GetSprintVelocityAsync(int sprintId, CancellationToken cancellationToken)
    {
        List<KanbanOderSprintIssuesResponse.Issue>? issues = await kanbanRepository.GetSprintIssuesByJirakeyAsync(sprintId, cancellationToken);
        return issues?.Where(i => i.Fields?.Status?.StatusCategory?.Name == "Done").Sum(i => i.Fields?.StoryPoints ?? 0);
    }

    public async Task<VelocityDto?> GetVelocityOfLastThreeSprintsAsync(string jiraKey, CancellationToken cancellationToken)
    {
        KanbanSprintsResponse? sprintResponseObj = await kanbanRepository.GetKanbanSprintsByJirakeyAsync(jiraKey, "closed", cancellationToken);
        List<KanbanSprintsResponse.Sprint> lastThreeSprints =
            sprintResponseObj?.Values?.AsEnumerable().Reverse().Take(3).ToList() ?? new List<KanbanSprintsResponse.Sprint>();
       if (!lastThreeSprints.Any())
        {
            logger.LogWarning("No closed sprints found.");
            return null;
        }
        decimal? totalStoryPointsByDone = 0;
        int sprintCount = 0;
        foreach (KanbanSprintsResponse.Sprint sprint in lastThreeSprints)
        {
            try
            {
                decimal? sprintVelocity = await this.GetSprintVelocityAsync(sprint.Id, cancellationToken);
                if (sprintVelocity.HasValue)
                {
                    totalStoryPointsByDone += sprintVelocity;
                    sprintCount++;
                }
            }
            catch (HttpRequestException ex)
            {
                logger.LogError($"Error fetching story points for sprint {sprint.Id}: {ex.Message}");
            }
        }
        return new VelocityDto
        {
            Velocity = Math.Round(totalStoryPointsByDone.Value / sprintCount, 2)
        };
    }
}