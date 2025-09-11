using Neusta.Jira.Connector.Application.Kanban.Queries;

namespace Neusta.Jira.Connector.Application.Tests.Kanban.Queries;

using Moq;
using Neusta.Jira.Connector.Application.Kanban.Interfaces;
using Neusta.Jira.Connector.Application.Kanban.Models;
using NUnit.Framework;

[TestFixture]
[TestOf(typeof(GetKanbanTicketsCountByTypeQuery))]
public class GetKanbanTicketsCountByTypeQueryTests
{

     private Mock<IKanbanService> _kanbanServiceMock;
    private GetKanbanTicketsCountByTypeQuery.Handler _handler;
    private const string TestJiraKey = "TEST-123";

    [SetUp]
    public void Setup()
    {
        // Arrange
        _kanbanServiceMock = new Mock<IKanbanService>();
        _handler = new GetKanbanTicketsCountByTypeQuery.Handler(_kanbanServiceMock.Object);
    }

    [Test]
    public async Task Handle_WhenServiceReturnsData_ShouldReturnTicketsCountByTypeDto()
    {
        // Arrange
        var expectedResult = new TicketsCountByTypeDto();
        _kanbanServiceMock
            .Setup(x => x.GetKanbanTicketsCountByTypeAsync(TestJiraKey, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var query = new GetKanbanTicketsCountByTypeQuery { JiraKey = TestJiraKey };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.EqualTo(expectedResult));
        _kanbanServiceMock.Verify(
            x => x.GetKanbanTicketsCountByTypeAsync(TestJiraKey, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Test]
    public async Task Handle_WhenServiceReturnsNull_ShouldReturnNull()
    {
        // Arrange
        _kanbanServiceMock
            .Setup(x => x.GetKanbanTicketsCountByTypeAsync(TestJiraKey, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TicketsCountByTypeDto?)null);

        var query = new GetKanbanTicketsCountByTypeQuery { JiraKey = TestJiraKey };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Null);
        _kanbanServiceMock.Verify(
            x => x.GetKanbanTicketsCountByTypeAsync(TestJiraKey, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Test]
    public async Task Handle_WithEmptyJiraKey_ShouldStillCallService()
    {
        // Arrange
        var query = new GetKanbanTicketsCountByTypeQuery { JiraKey = string.Empty };

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        _kanbanServiceMock.Verify(
            x => x.GetKanbanTicketsCountByTypeAsync(string.Empty, It.IsAny<CancellationToken>()),
            Times.Once);
    }

}