using Neusta.Jira.Connector.Application.Sprint.Queries;

namespace Neusta.Jira.Connector.Application.Tests.Sprint.Queries;

using Moq;
using Neusta.Jira.Connector.Application.Sprint.Interfaces;
using Neusta.Jira.Connector.Application.Sprint.Models;
using NUnit.Framework;

[TestFixture]
[TestOf(typeof(GetActiveSprintDataQuery))]
public class GetActiveSprintDataQueryTests
{

     private Mock<IActiveSprintService> _activeSprintServiceMock;
    private GetActiveSprintDataQuery.Handler _handler;
    private CancellationToken _cancellationToken;

    [SetUp]
    public void Setup()
    {
        _activeSprintServiceMock = new Mock<IActiveSprintService>();
        _handler = new GetActiveSprintDataQuery.Handler(_activeSprintServiceMock.Object);
        _cancellationToken = CancellationToken.None;
    }

    [Test]
    public async Task Handle_WhenSprintExists_ReturnsSprintDto()
    {
        // Arrange
        var query = new GetActiveSprintDataQuery { JiraKey = "TEST-123" };
        var expectedSprintDto = new SprintDto
        {
            Id = 1,
            Name = "Test Sprint",
            Goal = "Sprint Goal",
            StartDate = DateTime.Now,
            EndDate = DateTime.Now.AddDays(14)
        };

        _activeSprintServiceMock
            .Setup(x => x.GetActiveSprintDataByJiraKeyAsync(query.JiraKey, _cancellationToken))
            .ReturnsAsync(expectedSprintDto);

        // Act
        var result = await _handler.Handle(query, _cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.EqualTo(expectedSprintDto));
        _activeSprintServiceMock.Verify(
            x => x.GetActiveSprintDataByJiraKeyAsync(query.JiraKey, _cancellationToken),
            Times.Once);
    }

    [Test]
    public async Task Handle_WhenNoSprintExists_ReturnsNull()
    {
        // Arrange
        var query = new GetActiveSprintDataQuery { JiraKey = "TEST-123" };

        _activeSprintServiceMock
            .Setup(x => x.GetActiveSprintDataByJiraKeyAsync(query.JiraKey, _cancellationToken))
            .ReturnsAsync((SprintDto?)null);

        // Act
        var result = await _handler.Handle(query, _cancellationToken);

        // Assert
        Assert.That(result, Is.Null);
        _activeSprintServiceMock.Verify(
            x => x.GetActiveSprintDataByJiraKeyAsync(query.JiraKey, _cancellationToken),
            Times.Once);
    }

    /*[Test]
    public void Constructor_WithNullActiveSprintService_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new GetActiveSprintDataQuery.Handler(null!));
    }*/
}