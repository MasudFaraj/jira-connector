using Neusta.Jira.Connector.Application.Sprint.Queries;

namespace Neusta.Jira.Connector.Application.Tests.Sprint.Queries;

using Moq;
using Neusta.Jira.Connector.Application.Sprint.Interfaces;
using Neusta.Jira.Connector.Application.Sprint.Models;
using NUnit.Framework;

[TestFixture]
[TestOf(typeof(GetActiveSprintStoryPointsSumByStatusQuery))]
public class GetActiveSprintStoryPointsSumByStatusQueryTests
{

   private Mock<IActiveSprintService> activeSprintServiceMock;
    private GetActiveSprintStoryPointsSumByStatusQuery.Handler handler;

    [SetUp]
    public void Setup()
    {
        // Arrange
        activeSprintServiceMock = new Mock<IActiveSprintService>();
        handler = new GetActiveSprintStoryPointsSumByStatusQuery.Handler(activeSprintServiceMock.Object);
    }

    [Test]
    public async Task Handle_WhenCalled_ShouldCallServiceWithCorrectParameters()
    {
        // Arrange
        var query = new GetActiveSprintStoryPointsSumByStatusQuery { JiraKey = "TEST-123" };
        var expectedResult = new StoryPointsSumByStatusDto();
        var cancellationToken = CancellationToken.None;

        activeSprintServiceMock
            .Setup(x => x.GetActiveSprintStoryPointsSumByStatusAsync(query.JiraKey, cancellationToken))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await handler.Handle(query, cancellationToken);

        // Assert
        Assert.That(result, Is.SameAs(expectedResult));
        activeSprintServiceMock.Verify(
            x => x.GetActiveSprintStoryPointsSumByStatusAsync(query.JiraKey, cancellationToken),
            Times.Once);
    }

    [Test]
    public async Task Handle_WhenServiceReturnsNull_ShouldReturnNull()
    {
        // Arrange
        var query = new GetActiveSprintStoryPointsSumByStatusQuery { JiraKey = "TEST-123" };
        var cancellationToken = CancellationToken.None;

        activeSprintServiceMock
            .Setup(x => x.GetActiveSprintStoryPointsSumByStatusAsync(query.JiraKey, cancellationToken))
            .ReturnsAsync((StoryPointsSumByStatusDto?)null);

        // Act
        var result = await handler.Handle(query, cancellationToken);

        // Assert
        Assert.That(result, Is.Null);
        activeSprintServiceMock.Verify(
            x => x.GetActiveSprintStoryPointsSumByStatusAsync(query.JiraKey, cancellationToken),
            Times.Once);
    }

    /*[Test]
    public void Constructor_WithNullService_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new GetActiveSprintStoryPointsSumByStatusQuery.Handler(null!));
    }*/
}