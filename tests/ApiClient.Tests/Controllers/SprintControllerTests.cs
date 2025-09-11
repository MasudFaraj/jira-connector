using Neusta.Jira.Connector.ApiClient.Controllers;

namespace Neusta.Jira.Connector.ApiClient.Tests.Controllers;

using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Moq;
using Neusta.Jira.Connector.Application.Kanban.Models;
using Neusta.Jira.Connector.Application.Sprint.Models;
using Neusta.Jira.Connector.Application.Sprint.Queries;
using NUnit.Framework;

[TestFixture]
[TestOf(typeof(SprintController))]
public class SprintControllerTests
{

    private Mock<ISender> senderMock;
    private SprintController controller;
    private const string TestJiraKey = "TEST-123";

    [SetUp]
    public void Setup()
    {
        this.senderMock = new Mock<ISender>();
        this.controller = new SprintController(this.senderMock.Object);
    }

    [Test]
    public async Task GetActiveSprintData_ShouldReturnSprintDto_WhenQuerySucceeds()
    {
        // Arrange
        SprintDto expectedResult = new SprintDto();
        this.senderMock.Setup(x => x.Send(It.IsAny<GetActiveSprintDataQuery>(),
                It.IsAny<CancellationToken>())).ReturnsAsync(expectedResult);

        // Act
        var result = await this.controller.GetActiveSprintDataQuery(TestJiraKey, CancellationToken.None);

        // Assert
        Assert.That(result, Is.EqualTo(expectedResult));
        this.senderMock.Verify(x => x.Send(
            It.Is<GetActiveSprintDataQuery>(q => q.JiraKey == TestJiraKey),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task GetActiveSprintTicketsCountByStatus_ShouldReturnTicketsCountDto_WhenQuerySucceeds()
    {
        // Arrange
        TicketsCountByStatusDto expectedResult = new TicketsCountByStatusDto();
        this.senderMock.Setup(x => x.Send(It.IsAny<GetActiveSprintTicketsCountByStatusQuery>(), 
                It.IsAny<CancellationToken>())).ReturnsAsync(expectedResult);

        // Act
        TicketsCountByStatusDto? result = await this.controller.GetActiveSprintTicketsCountByStatusQuery(
            TestJiraKey, CancellationToken.None);

        // Assert
        Assert.That(result, Is.EqualTo(expectedResult));
        this.senderMock.Verify(x => x.Send(
            It.Is<GetActiveSprintTicketsCountByStatusQuery>(
                q => q.JiraKey == TestJiraKey),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task GetActiveSprintStoryPointsSumByStatus_ShouldReturnStoryPointsSumDto_WhenQuerySucceeds()
    {
        // Arrange
        StoryPointsSumByStatusDto expectedResult = new StoryPointsSumByStatusDto();
        this.senderMock.Setup(x => x.Send(It.IsAny<GetActiveSprintStoryPointsSumByStatusQuery>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        // Act
        StoryPointsSumByStatusDto? result = await this.controller.GetActiveSprintStoryPointsSumByStatusQuery(
            TestJiraKey, CancellationToken.None);

        // Assert
        Assert.That(result, Is.EqualTo(expectedResult));
        this.senderMock.Verify(x => x.Send(
            It.Is<GetActiveSprintStoryPointsSumByStatusQuery>(
                q => q.JiraKey == TestJiraKey),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task GetActiveSprintData_ShouldReturnNull_WhenNoActiveSprintExists()
    {
        // Arrange
        this.senderMock.Setup(x => x.Send(It.IsAny<GetActiveSprintDataQuery>(),
                It.IsAny<CancellationToken>())).ReturnsAsync((SprintDto?)null);

        // Act
        SprintDto? result = await this.controller.GetActiveSprintDataQuery(TestJiraKey, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetActiveSprintTicketsCountByStatus_ShouldReturnNull_WhenNoActiveSprintExists()
    {
        // Arrange
        this.senderMock.Setup(x => x.Send(It.IsAny<GetActiveSprintTicketsCountByStatusQuery>(),
                It.IsAny<CancellationToken>())).ReturnsAsync((TicketsCountByStatusDto?)null);

        // Act
        var result = await this.controller.GetActiveSprintTicketsCountByStatusQuery(
            TestJiraKey, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetActiveSprintStoryPointsSumByStatus_ShouldReturnNull_WhenNoActiveSprintExists()
    {
        // Arrange
        this.senderMock.Setup(x => x.Send(It.IsAny<GetActiveSprintStoryPointsSumByStatusQuery>(), 
                It.IsAny<CancellationToken>())).ReturnsAsync((StoryPointsSumByStatusDto?)null);

        // Act
        var result = await this.controller.GetActiveSprintStoryPointsSumByStatusQuery(
            TestJiraKey, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Null);
    }
}