using Neusta.Jira.Connector.Application.Sprint.Queries;

namespace Neusta.Jira.Connector.Application.Tests.Sprint.Queries;

using Moq;
using Neusta.Jira.Connector.Application.Kanban.Models;
using Neusta.Jira.Connector.Application.Sprint.Interfaces;
using NUnit.Framework;

[TestFixture]
[TestOf(typeof(GetActiveSprintTicketsCountByStatusQuery))]
public class GetActiveSprintTicketsCountByStatusQueryTests
{

     private Mock<IActiveSprintService> activeSprintServiceMock;
    private GetActiveSprintTicketsCountByStatusQuery.Handler handler;

    [SetUp]
    public void Setup()
    {
        // Arrange
        this.activeSprintServiceMock = new Mock<IActiveSprintService>();
        this.handler = new GetActiveSprintTicketsCountByStatusQuery.Handler(this.activeSprintServiceMock.Object);
    }

    [Test]
    public async Task Handle_WhenCalled_ShouldCallServiceWithCorrectParameters()
    {
        // Arrange
        var query = new GetActiveSprintTicketsCountByStatusQuery { JiraKey = "TEST-123" };
        var expectedResult = new TicketsCountByStatusDto();
        var cancellationToken = CancellationToken.None;

        this.activeSprintServiceMock
            .Setup(x => x.GetActiveSprintTicketsCountByStatusAsync(query.JiraKey, cancellationToken))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await this.handler.Handle(query, cancellationToken);

        // Assert
        Assert.That(result, Is.EqualTo(expectedResult));
        this.activeSprintServiceMock.Verify(
            x => x.GetActiveSprintTicketsCountByStatusAsync(query.JiraKey, cancellationToken),
            Times.Once);
    }

    [Test]
    public async Task Handle_WhenServiceReturnsNull_ShouldReturnNull()
    {
        // Arrange
        var query = new GetActiveSprintTicketsCountByStatusQuery { JiraKey = "TEST-123" };
        var cancellationToken = CancellationToken.None;

        this.activeSprintServiceMock
            .Setup(x => x.GetActiveSprintTicketsCountByStatusAsync(query.JiraKey, cancellationToken))
            .ReturnsAsync((TicketsCountByStatusDto?)null);

        // Act
        var result = await this.handler.Handle(query, cancellationToken);

        // Assert
        Assert.That(result, Is.Null);
        this.activeSprintServiceMock.Verify(
            x => x.GetActiveSprintTicketsCountByStatusAsync(query.JiraKey, cancellationToken),
            Times.Once);
    }

    /*[Test]
    public void Constructor_WithNullService_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new GetActiveSprintTicketsCountByStatusQuery.Handler(null!));
    }*/

}