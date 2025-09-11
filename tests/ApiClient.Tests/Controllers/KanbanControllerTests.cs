using Neusta.Jira.Connector.ApiClient.Controllers;

namespace Neusta.Jira.Connector.ApiClient.Tests.Controllers;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Moq;
using Neusta.Jira.Connector.Application.Kanban.Models;
using Neusta.Jira.Connector.Application.Kanban.Queries;
using NUnit.Framework;

[TestFixture]
[TestOf(typeof(KanbanController))]
public class KanbanControllerTests
{

    private Mock<ISender> _senderMock;
    private KanbanController _controller;
    private const string TestJiraKey = "TEST-123";

    [SetUp]
    public void Setup()
    {
        _senderMock = new Mock<ISender>();
        _controller = new KanbanController(_senderMock.Object);
    }

    [Test]
    public async Task GetKanbanTicketsCountByStatusQuery_WhenCalled_ShouldSendQueryAndReturnResult()
    {
        // Arrange
        var expectedResult = new TicketsCountByStatusDto
        {
            Total = 10,
            Todo = 3,
            InProgress = 4,
            Done = 3
        };

        _senderMock.Setup(x => x.Send(
            It.Is<GetKanbanTicketsCountByStatusQuery>(q => q.JiraKey == TestJiraKey),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _controller.GetKanbanTicketsCountByStatusQuery(TestJiraKey, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.EqualTo(expectedResult));
        _senderMock.Verify(x => x.Send(
            It.Is<GetKanbanTicketsCountByStatusQuery>(q => q.JiraKey == TestJiraKey),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task GetKanbanTicketsCountByTypeQuery_WhenCalled_ShouldSendQueryAndReturnResult()
    {
        // Arrange
        var expectedResult = new TicketsCountByTypeDto
        {
            Story = 5,
            Task = 3,
            Bug = 2
        };

        _senderMock.Setup(x => x.Send(
            It.Is<GetKanbanTicketsCountByTypeQuery>(q => q.JiraKey == TestJiraKey),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _controller.GetKanbanTicketsCountByTypeQuery(TestJiraKey, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.EqualTo(expectedResult));
        _senderMock.Verify(x => x.Send(
            It.Is<GetKanbanTicketsCountByTypeQuery>(q => q.JiraKey == TestJiraKey),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task GetVelocityOfLastThreeSprints_WhenCalled_ShouldSendQueryAndReturnResult()
    {
        // Arrange
        var expectedResult = new VelocityDto
        {
            Velocity = 20,
            
        };
        
        _senderMock.Setup(x => x.Send(
            It.Is<GetVelocityOfLastThreeSprintsQuery>(q => q.JiraKey == TestJiraKey),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _controller.GetVelocityOfLastThreeSprints(TestJiraKey, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.EqualTo(expectedResult));
        _senderMock.Verify(x => x.Send(
            It.Is<GetVelocityOfLastThreeSprintsQuery>(q => q.JiraKey == TestJiraKey),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task GetKanbanTicketsCountByStatusQuery_WhenSenderReturnsNull_ShouldReturnNull()
    {
        // Arrange
        _senderMock.Setup(x => x.Send(
            It.IsAny<GetKanbanTicketsCountByStatusQuery>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync((TicketsCountByStatusDto?)null);

        // Act
        var result = await _controller.GetKanbanTicketsCountByStatusQuery(TestJiraKey, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Null);
    }
}