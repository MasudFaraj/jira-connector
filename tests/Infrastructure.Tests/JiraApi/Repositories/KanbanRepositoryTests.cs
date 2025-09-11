namespace Neusta.Jira.Connector.Infrastructure.Tests.JiraApi.Repositories;

using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Neusta.Jira.Connector.Domain.Models;
using Neusta.Jira.Connector.Infrastructure.JiraApi.Interfaces;
using Neusta.Jira.Connector.Infrastructure.JiraApi.Repositories;
using NUnit.Framework;

[TestFixture]
[TestOf(typeof(KanbanRepository))]
public class KanbanRepositoryTests
{
    private Mock<IJiraClient> jiraClientMock;
    private KanbanRepository kanbanRepository;
    private CancellationToken cancellationToken;

    [SetUp]
    public void Setup()
    {
        this.jiraClientMock = new Mock<IJiraClient>();
        this.kanbanRepository = new KanbanRepository(this.jiraClientMock.Object);
        this.cancellationToken = CancellationToken.None;
    }

    [Test]
    public async Task GetBoardIdByJirakeyAsync_ValidResponse_ReturnsBoardId()
    {
        // Arrange
        const string jiraKey = "TEST";
        HttpResponseMessage responseMessage = new HttpResponseMessage
        {
            Content = new StringContent("""
            {
                "values": [
                    {
                        "id": 123,
                        "name": "Test Board"
                    }
                ]
            }
            """)
        };
        this.jiraClientMock.Setup(x => x.SendRequestAsync("rest/agile/1.0/board?projectKeyOrId=TEST"))
            .ReturnsAsync(responseMessage);

        // Act
        int? result = await this.kanbanRepository.GetBoardIdByJirakeyAsync(jiraKey, this.cancellationToken);

        // Assert
        Assert.That(result, Is.EqualTo(123));
        this.jiraClientMock.Verify(x => x.SendRequestAsync("rest/agile/1.0/board?projectKeyOrId=TEST"), Times.Once);
    }

    [Test]
    public async Task GetKanbanSprintsByJirakeyAsync_ValidResponse_ReturnsSprintsResponse()
    {
        // Arrange
        const string jiraKey = "TEST";
        const string state = "active";
        HttpResponseMessage boardResponseMessage = new()
        {
            Content = new StringContent("""{"values": [{"id": 123}]}""")
        };
        HttpResponseMessage sprintsResponseMessage = new()
        {
            Content = new StringContent("""
            {
                "maxResults": 50,
                "startAt": 0,
                "isLast": true,
                "values": [
                    {
                        "id": 456,
                        "state": "active",
                        "name": "Sprint 1"
                    }
                ]
            }
            """)
        };
        this.jiraClientMock.Setup(x => x.SendRequestAsync("rest/agile/1.0/board?projectKeyOrId=TEST"))
            .ReturnsAsync(boardResponseMessage);
        this.jiraClientMock.Setup(x => x.SendRequestAsync("rest/agile/1.0/board/123/sprint?state=active"))
            .ReturnsAsync(sprintsResponseMessage);

        // Act
        KanbanSprintsResponse? result = await this.kanbanRepository.GetKanbanSprintsByJirakeyAsync(
            jiraKey, state, this.cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Values.First().Id, Is.EqualTo(456));
        Assert.That(result.Values.First().Name, Is.EqualTo("Sprint 1"));
    }

    [Test]
    public async Task GetSprintIssuesByJirakeyAsync_ValidResponse_ReturnsIssuesList()
    {
        // Arrange
        const int sprintId = 123;
        HttpResponseMessage responseMessage = new()
        {
            Content = new StringContent("""
            {
                "issues": [
                    {
                        "id": "10001",
                        "fields": {
                            "customfield_10006": 5,
                            "status": { "name": "In Progress" },
                            "issuetype": { "name": "Task" }
                        }
                    }
                ]
            }
            """)
        };
        string expectedUrl = "rest/agile/1.0/sprint/123/issue?fields=customfield_10006,status,issuetype" +
                             "&jql=issuetype not in (subTaskIssueTypes())";
        this.jiraClientMock.Setup(x => x.SendRequestAsync(expectedUrl))
            .ReturnsAsync(responseMessage);
        // Act
        List<KanbanOderSprintIssuesResponse.Issue>? result =
            await this.kanbanRepository.GetSprintIssuesByJirakeyAsync(sprintId, this.cancellationToken);
        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count, Is.EqualTo(1));
        Assert.That(result[0].Id, Is.EqualTo("10001"));
    }

    [Test]
    public async Task GetKanbanIssuesByJirakeyAsync_ValidResponse_ReturnsIssuesList()
    {
        // Arrange
        const string jiraKey = "TEST";
        HttpResponseMessage boardResponseMessage = new()
        {
            Content = new StringContent(@"{""values"": [{""id"": 123}]}")
        };
        HttpResponseMessage issuesResponseMessage = new()
        {
            Content = new StringContent(@"{
                    ""issues"": [
                        {
                            ""id"": ""10001"",
                            ""fields"": {
                                ""customfield_10006"": 5,
                                ""status"": { ""name"": ""In Progress"" },
                                ""issuetype"": { ""name"": ""Task"" }
                            }
                        }
                    ]
                }")
        };
        this.jiraClientMock.Setup(x => x.SendRequestAsync("rest/agile/1.0/board?projectKeyOrId=TEST"))
            .ReturnsAsync(boardResponseMessage);
        string expectedUrl = "rest/agile/1.0/board/123/issue?fields=customfield_10006,status,issuetype" +
                             "&jql=issuetype not in (subTaskIssueTypes())";
        this.jiraClientMock.Setup(x => x.SendRequestAsync(expectedUrl))
            .ReturnsAsync(issuesResponseMessage);

        // Act
        List<KanbanOderSprintIssuesResponse.Issue>? result =
            await this.kanbanRepository.GetKanbanIssuesByJirakeyAsync(jiraKey, this.cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count, Is.EqualTo(1));
        Assert.That(result[0].Id, Is.EqualTo("10001"));
    }

    [Test]
    public async Task GetBoardIdByJirakeyAsync_EmptyResponse_ReturnsNull()
    {
        // Arrange
        const string jiraKey = "TEST";
        HttpResponseMessage responseMessage = new()
        {
            Content = new StringContent("""{"values": []}""")
        };
        this.jiraClientMock.Setup(x => x.SendRequestAsync("rest/agile/1.0/board?projectKeyOrId=TEST"))
            .ReturnsAsync(responseMessage);

        // Act
        int? result = await this.kanbanRepository.GetBoardIdByJirakeyAsync(jiraKey, this.cancellationToken);

        // Assert
        Assert.That(result, Is.Null);
    }
}