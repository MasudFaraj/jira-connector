namespace Neusta.Jira.Connector.Application.Tests.Sprint.Services;

using AutoMapper;
using Moq;
using Neusta.Jira.Connector.Application.Kanban.Models;
using Neusta.Jira.Connector.Application.Sprint.Models;
using Neusta.Jira.Connector.Application.Sprint.Services;
using Neusta.Jira.Connector.Domain.Interfaces;
using Neusta.Jira.Connector.Domain.Models;
using NUnit.Framework;

[TestFixture]
[TestOf(typeof(ActiveSprintService))]
public class ActiveSprintServiceTests
{
    private Mock<IKanbanRepository> kanbanRepositoryMock;
    private Mock<IMapper> mapperMock;
    private ActiveSprintService activeSprintService;
    private CancellationToken cancellationToken;

    [SetUp]
    public void Setup()
    {
        this.kanbanRepositoryMock = new Mock<IKanbanRepository>();
        this.mapperMock = new Mock<IMapper>();
        this.activeSprintService = new ActiveSprintService(this.kanbanRepositoryMock.Object, this.mapperMock.Object);
        this.cancellationToken = CancellationToken.None;
    }

    [Test]
    public async Task GetActiveSprintDataByJiraKeyAsync_WhenSprintExists_ReturnsSprintDto()
    {
        // Arrange
        string jiraKey = "TEST-123";
        KanbanSprintsResponse.Sprint sprint = new KanbanSprintsResponse.Sprint { Id = 1, Name = "Test Sprint" };
        KanbanSprintsResponse sprintResponse = new KanbanSprintsResponse
            { Values = new List<KanbanSprintsResponse.Sprint> { sprint } };
        SprintDto expectedSprintDto = new SprintDto { Id = 1, Name = "Test Sprint" };

        this.kanbanRepositoryMock.Setup(x => x.GetKanbanSprintsByJirakeyAsync(
                                          jiraKey, "active", this.cancellationToken)).ReturnsAsync(sprintResponse);
        this.mapperMock.Setup(x => x.Map<SprintDto>(sprint)).Returns(expectedSprintDto);

        // Act
        SprintDto? result = await this.activeSprintService.GetActiveSprintDataByJiraKeyAsync(jiraKey, this.cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.EqualTo(expectedSprintDto));
    }

    [Test]
    public async Task GetActiveSprintDataByJiraKeyAsync_WhenNoSprintExists_ReturnsNull()
    {
        // Arrange
        string jiraKey = "TEST-123";
        KanbanSprintsResponse sprintResponse = new KanbanSprintsResponse { Values = new List<KanbanSprintsResponse.Sprint>() };

        this.kanbanRepositoryMock.Setup(x => x.GetKanbanSprintsByJirakeyAsync(
                                          jiraKey, "active", this.cancellationToken)).ReturnsAsync(sprintResponse);

        // Act
        SprintDto? result = await this.activeSprintService.GetActiveSprintDataByJiraKeyAsync(jiraKey, this.cancellationToken);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetActiveSprintIssuesByJirakeyAsync_WhenSprintExists_ReturnsIssues()
    {
        // Arrange
        string jiraKey = "TEST-123";
        KanbanSprintsResponse.Sprint sprint = new KanbanSprintsResponse.Sprint { Id = 1 };
        KanbanSprintsResponse sprintResponse = new KanbanSprintsResponse
            { Values = new List<KanbanSprintsResponse.Sprint> { sprint } };
        List<KanbanOderSprintIssuesResponse.Issue> expectedIssues = new List<KanbanOderSprintIssuesResponse.Issue>
        {
            new() { Id = "1", Key = "TEST-1" }
        };

        this.kanbanRepositoryMock.Setup(x => x.GetKanbanSprintsByJirakeyAsync(
                                          jiraKey, "active", this.cancellationToken)).ReturnsAsync(sprintResponse);
        this.kanbanRepositoryMock.Setup(x => x.GetSprintIssuesByJirakeyAsync(
                                          sprint.Id, this.cancellationToken)).ReturnsAsync(expectedIssues);

        // Act
        List<KanbanOderSprintIssuesResponse.Issue>? result =
            await this.activeSprintService.GetActiveSprintIssuesByJirakeyAsync(jiraKey, this.cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.EqualTo(expectedIssues));
    }

    [Test]
    public async Task GetActiveSprintTicketsCountByStatusAsync_ReturnsCorrectCounts()
    {
        // Arrange
        string jiraKey = "TEST-123";
        List<KanbanOderSprintIssuesResponse.Issue> issues = new List<KanbanOderSprintIssuesResponse.Issue>
        {
            CreateIssue("To Do"),
            CreateIssue("To Do"),
            CreateIssue("In Progress"),
            CreateIssue("Done"),
            CreateIssue("Done")
        };

        this.kanbanRepositoryMock.Setup(x => x.GetKanbanSprintsByJirakeyAsync(
                jiraKey, "active", this.cancellationToken)).ReturnsAsync(
                    new KanbanSprintsResponse { Values = new List<KanbanSprintsResponse.Sprint> { new() { Id = 1 } } });
        this.kanbanRepositoryMock.Setup(x => x.GetSprintIssuesByJirakeyAsync(
                1, this.cancellationToken)).ReturnsAsync(issues);

        // Act
        TicketsCountByStatusDto? result =
            await this.activeSprintService.GetActiveSprintTicketsCountByStatusAsync(jiraKey, this.cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Total, Is.EqualTo(5));
        Assert.That(result.Todo, Is.EqualTo(2));
        Assert.That(result.InProgress, Is.EqualTo(1));
        Assert.That(result.Done, Is.EqualTo(2));
    }

    [Test]
    public async Task GetActiveSprintStoryPointsSumByStatusAsync_ReturnsCorrectSums()
    {
        // Arrange
        string jiraKey = "TEST-123";
        List<KanbanOderSprintIssuesResponse.Issue> issues = new List<KanbanOderSprintIssuesResponse.Issue>
        {
            CreateIssueWithPoints("To Do", 3),
            CreateIssueWithPoints("To Do", 2),
            CreateIssueWithPoints("In Progress", 5),
            CreateIssueWithPoints("Done", 3),
            CreateIssueWithPoints("Done", 2)
        };

        this.kanbanRepositoryMock.Setup(x => x.GetKanbanSprintsByJirakeyAsync(
                jiraKey, "active", this.cancellationToken)).ReturnsAsync(
                    new KanbanSprintsResponse { Values = new List<KanbanSprintsResponse.Sprint> { new() { Id = 1 } } });
        this.kanbanRepositoryMock.Setup(x => x.GetSprintIssuesByJirakeyAsync(
                1, this.cancellationToken)).ReturnsAsync(issues);

        // Act
        StoryPointsSumByStatusDto? result =
            await this.activeSprintService.GetActiveSprintStoryPointsSumByStatusAsync(jiraKey, this.cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Planned, Is.EqualTo(15));
        Assert.That(result.Todo, Is.EqualTo(5));
        Assert.That(result.InProgress, Is.EqualTo(5));
        Assert.That(result.Done, Is.EqualTo(5));
    }

    private static KanbanOderSprintIssuesResponse.Issue CreateIssue(string statusCategoryName)
    {
        return new KanbanOderSprintIssuesResponse.Issue
        {
            Fields = new KanbanOderSprintIssuesResponse.Fields
            {
                Status = new KanbanOderSprintIssuesResponse.Status
                {
                    StatusCategory = new KanbanOderSprintIssuesResponse.StatusCategory
                    {
                        Name = statusCategoryName
                    }
                }
            }
        };
    }

    private static KanbanOderSprintIssuesResponse.Issue CreateIssueWithPoints(string statusCategoryName, decimal storyPoints)
    {
        KanbanOderSprintIssuesResponse.Issue issue = CreateIssue(statusCategoryName);
        issue.Fields.StoryPoints = storyPoints;
        return issue;
    }
}