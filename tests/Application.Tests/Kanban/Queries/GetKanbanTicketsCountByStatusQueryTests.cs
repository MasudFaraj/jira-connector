using Neusta.Jira.Connector.Application.Kanban.Queries;

namespace Neusta.Jira.Connector.Application.Tests.Kanban.Queries;

using Moq;
using Neusta.Jira.Connector.Application.Kanban.Interfaces;
using Neusta.Jira.Connector.Application.Kanban.Models;
using NUnit.Framework;

[TestFixture]
[TestOf(typeof(GetKanbanTicketsCountByStatusQuery))]
public class GetKanbanTicketsCountByStatusQueryTests
{

    private Mock<IKanbanService> kanbanServiceMock;
    private GetKanbanTicketsCountByStatusQuery.Handler handler;
    
    [SetUp]
    public void Setup()
    {
        // Arrange
        kanbanServiceMock = new Mock<IKanbanService>();
        handler = new GetKanbanTicketsCountByStatusQuery.Handler(kanbanServiceMock.Object);
    }
    
    [Test]
    public async Task Handle_WithValidJiraKey_ShouldReturnTicketsCountByStatus()
    {
        // Arrange
        var jiraKey = "TEST-123";
        var expectedResult = new TicketsCountByStatusDto
        {
            // Add expected properties here based on your DTO structure
        };
        
        kanbanServiceMock.Setup(x => x.GetKanbanTicketsCountByStatusAsync(
                jiraKey, 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);
            
        var query = new GetKanbanTicketsCountByStatusQuery { JiraKey = jiraKey };
        
        // Act
        var result = await handler.Handle(query, CancellationToken.None);
        
        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.EqualTo(expectedResult));
        kanbanServiceMock.Verify(x => x.GetKanbanTicketsCountByStatusAsync(
            jiraKey, 
            It.IsAny<CancellationToken>()), 
            Times.Once);
    }
    
    [Test]
    public async Task Handle_WhenServiceReturnsNull_ShouldReturnNull()
    {
        // Arrange
        var jiraKey = "TEST-123";
        kanbanServiceMock.Setup(x => x.GetKanbanTicketsCountByStatusAsync(
                jiraKey, 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((TicketsCountByStatusDto?)null);
            
        var query = new GetKanbanTicketsCountByStatusQuery { JiraKey = jiraKey };
        
        // Act
        var result = await handler.Handle(query, CancellationToken.None);
        
        // Assert
        Assert.That(result, Is.Null);
        kanbanServiceMock.Verify(x => x.GetKanbanTicketsCountByStatusAsync(
            jiraKey, 
            It.IsAny<CancellationToken>()), 
            Times.Once);
    }
    
    [Test]
    public async Task Handle_WithEmptyJiraKey_ShouldStillCallService()
    {
        // Arrange
        var jiraKey = string.Empty;
        var query = new GetKanbanTicketsCountByStatusQuery { JiraKey = jiraKey };
        
        kanbanServiceMock.Setup(x => x.GetKanbanTicketsCountByStatusAsync(
                jiraKey, 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((TicketsCountByStatusDto?)null);
        
        // Act
        var result = await handler.Handle(query, CancellationToken.None);
        
        // Assert
        Assert.That(result, Is.Null);
        kanbanServiceMock.Verify(x => x.GetKanbanTicketsCountByStatusAsync(
            jiraKey, 
            It.IsAny<CancellationToken>()), 
            Times.Once);
    }
}