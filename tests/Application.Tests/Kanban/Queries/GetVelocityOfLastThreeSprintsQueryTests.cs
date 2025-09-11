using Neusta.Jira.Connector.Application.Kanban.Queries;

namespace Neusta.Jira.Connector.Application.Tests.Kanban.Queries;

using Moq;
using Neusta.Jira.Connector.Application.Kanban.Interfaces;
using Neusta.Jira.Connector.Application.Kanban.Models;
using NUnit.Framework;

[TestFixture]
[TestOf(typeof(GetVelocityOfLastThreeSprintsQuery))]
public class GetVelocityOfLastThreeSprintsQueryTests
{

    private Mock<IKanbanService> kanbanServiceMock;
    private GetVelocityOfLastThreeSprintsQuery.Handler handler;
    private const string TestJiraKey = "TEST-123";

    [SetUp]
    public void Setup()
    {
        kanbanServiceMock = new Mock<IKanbanService>();
        handler = new GetVelocityOfLastThreeSprintsQuery.Handler(kanbanServiceMock.Object);
    }

    [Test]
    public async Task Handle_WhenCalled_ShouldCallServiceWithCorrectParameters()
    {
        // Arrange
        var query = new GetVelocityOfLastThreeSprintsQuery { JiraKey = TestJiraKey };
        var expectedVelocity = new VelocityDto(); // Add properties as needed
        var cancellationToken = CancellationToken.None;

        kanbanServiceMock
            .Setup(x => x.GetVelocityOfLastThreeSprintsAsync(TestJiraKey, cancellationToken))
            .ReturnsAsync(expectedVelocity);

        // Act
        var result = await handler.Handle(query, cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.EqualTo(expectedVelocity));
        kanbanServiceMock.Verify(
            x => x.GetVelocityOfLastThreeSprintsAsync(TestJiraKey, cancellationToken),
            Times.Once);
    }

    [Test]
    public async Task Handle_WhenServiceReturnsNull_ShouldReturnNull()
    {
        // Arrange
        var query = new GetVelocityOfLastThreeSprintsQuery { JiraKey = TestJiraKey };
        var cancellationToken = CancellationToken.None;

        kanbanServiceMock
            .Setup(x => x.GetVelocityOfLastThreeSprintsAsync(TestJiraKey, cancellationToken))
            .ReturnsAsync((VelocityDto?)null);

        // Act
        var result = await handler.Handle(query, cancellationToken);

        // Assert
        Assert.That(result, Is.Null);
        kanbanServiceMock.Verify(
            x => x.GetVelocityOfLastThreeSprintsAsync(TestJiraKey, cancellationToken),
            Times.Once);
    }
}