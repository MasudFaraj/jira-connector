namespace Neusta.Jira.Connector.Application.Tests.Kanban.Services;

using Microsoft.Extensions.Logging;
using Moq;
using Neusta.Jira.Connector.Application.Kanban.Models;
using Neusta.Jira.Connector.Application.Kanban.Services;
using Neusta.Jira.Connector.Domain.Interfaces;
using Neusta.Jira.Connector.Domain.Models;
using NUnit.Framework;

[TestFixture]
[TestOf(typeof(KanbanService))]
public class KanbanServiceTest
{
    private Mock<IKanbanRepository> kanbanRepositoryMock;
    private Mock<ILogger<KanbanService>> loggerMock;
    private KanbanService kanbanService;

    [SetUp]
    public void Setup()
    {
        this.kanbanRepositoryMock = new Mock<IKanbanRepository>();
        this.loggerMock = new Mock<ILogger<KanbanService>>();
        this.kanbanService = new KanbanService(this.kanbanRepositoryMock.Object, this.loggerMock.Object);
    }

    [Test]
    public async Task GetKanbanTicketsCountByStatusAsync_ReturnsCorrectCounts()
    {
        // Arrange
        const string jiraKey = "TEST-123";
        List<KanbanOderSprintIssuesResponse.Issue> issues =
        [
            CreateIssue(),
            CreateIssue(),
            CreateIssue("In Progress"),
            CreateIssue("Done"),
            CreateIssue("Done")
        ];
        this.kanbanRepositoryMock.Setup(x => x.GetKanbanIssuesByJirakeyAsync(
                jiraKey, It.IsAny<CancellationToken>())).ReturnsAsync(issues);

        // Act
        TicketsCountByStatusDto? result =
            await this.kanbanService.GetKanbanTicketsCountByStatusAsync(jiraKey, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.Total, Is.EqualTo(5));
            Assert.That(result.Todo, Is.EqualTo(2));
            Assert.That(result.InProgress, Is.EqualTo(1));
            Assert.That(result.Done, Is.EqualTo(2));
        });
    }

    [Test]
    public async Task GetKanbanTicketsCountByTypeAsync_ReturnsCorrectCounts()
    {
        // Arrange
        const string jiraKey = "TEST-123";
        List<KanbanOderSprintIssuesResponse.Issue> issues = new()
        {
            CreateIssue(issueType: "Story"),
            CreateIssue(issueType: "Story"),
            CreateIssue(issueType: "Task"),
            CreateIssue(issueType: "Bug"),
            CreateIssue(issueType: "Bug")
        };

        this.kanbanRepositoryMock.Setup(x => x.GetKanbanIssuesByJirakeyAsync(
                jiraKey, It.IsAny<CancellationToken>())).ReturnsAsync(issues);

        // Act
        TicketsCountByTypeDto? result =
            await this.kanbanService.GetKanbanTicketsCountByTypeAsync(jiraKey, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.Story, Is.EqualTo(2));
            Assert.That(result.Task, Is.EqualTo(1));
            Assert.That(result.Bug, Is.EqualTo(2));
        });
    }

    [Test]
    public async Task GetSprintVelocityAsync_ReturnsSumOfDoneStoryPoints()
    {
        // Arrange
        int sprintId = 1;
        List<KanbanOderSprintIssuesResponse.Issue> issues = new()
        {
            CreateIssue("Done", storyPoints: 3),
            CreateIssue("Done", storyPoints: 5),
            CreateIssue("In Progress", storyPoints: 8),
            CreateIssue(storyPoints: 2)
        };

        this.kanbanRepositoryMock.Setup(x => x.GetSprintIssuesByJirakeyAsync(
                         sprintId, It.IsAny<CancellationToken>())).ReturnsAsync(issues);

        // Act
        decimal? result = await this.kanbanService.GetSprintVelocityAsync(sprintId, CancellationToken.None);

        // Assert
        Assert.That(result, Is.EqualTo(8)); // Only sum of "Done" items (3 + 5)
    }

    [Test]
    public async Task GetVelocityOfLastThreeSprintsAsync_CalculatesAverageVelocityCorrectly()
    {
        // Arrange
        const string jiraKey = "TEST-123";
        KanbanSprintsResponse sprints = new()
        {
            Values = new List<KanbanSprintsResponse.Sprint>
            {
                new() { Id = 1, Name = "Sprint 1" },
                new() { Id = 2, Name = "Sprint 2" },
                new() { Id = 3, Name = "Sprint 3" }
            }
        };
        this.kanbanRepositoryMock.Setup(x => x.GetKanbanSprintsByJirakeyAsync(
                jiraKey, "closed", It.IsAny<CancellationToken>())).ReturnsAsync(sprints);

        // Setup sprint velocities
        this.kanbanRepositoryMock.Setup(x => x.GetSprintIssuesByJirakeyAsync(
                1, It.IsAny<CancellationToken>())).ReturnsAsync([CreateIssue("Done", storyPoints: 10)]);
        this.kanbanRepositoryMock.Setup(x => x.GetSprintIssuesByJirakeyAsync(
                2, It.IsAny<CancellationToken>())).ReturnsAsync([CreateIssue("Done", storyPoints: 20)]);
        this.kanbanRepositoryMock.Setup(x => x.GetSprintIssuesByJirakeyAsync(
                3, It.IsAny<CancellationToken>())).ReturnsAsync([CreateIssue("Done", storyPoints: 30)]);
        // Act
        VelocityDto? result = await this.kanbanService.GetVelocityOfLastThreeSprintsAsync(jiraKey, CancellationToken.None);
        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Velocity, Is.EqualTo(20)); // Average of (10 + 20 + 30) / 3
    }

    [Test]
    public async Task GetVelocityOfLastThreeSprintsAsync_ReturnsNull_WhenNoClosedSprints()
    {
        // Arrange
        string jiraKey = "TEST-123";
        KanbanSprintsResponse sprints = new()
        {
            Values = new List<KanbanSprintsResponse.Sprint>()
        };

        this.kanbanRepositoryMock.Setup(x => x.GetKanbanSprintsByJirakeyAsync(
                jiraKey, "closed", It.IsAny<CancellationToken>())).ReturnsAsync(sprints);

        // Act
        VelocityDto? result = await this.kanbanService.GetVelocityOfLastThreeSprintsAsync(jiraKey, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Null);
    }

    private static KanbanOderSprintIssuesResponse.Issue CreateIssue(string status = "To Do", string issueType = "Story",
        decimal storyPoints = 0)
    {
        return new KanbanOderSprintIssuesResponse.Issue
        {
            Fields = new KanbanOderSprintIssuesResponse.Fields
            {
                Status = new KanbanOderSprintIssuesResponse.Status
                {
                    StatusCategory = new KanbanOderSprintIssuesResponse.StatusCategory
                    {
                        Name = status
                    }
                },
                IssueType = new KanbanOderSprintIssuesResponse.IssueType
                {
                    Name = issueType
                },
                StoryPoints = storyPoints
            }
        };
    }
}