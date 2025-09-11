namespace Neusta.Jira.Connector.Infrastructure.Tests.JiraApi;

using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Neusta.Jira.Connector.Infrastructure.JiraApi;
using NUnit.Framework;

[TestFixture]
[TestOf(typeof(JiraClient))]
public class JiraClientTests
{
    private Mock<IHttpClientFactory> mockHttpClientFactory;
    private Mock<ILogger<JiraClient>> mockLogger;
    private Mock<HttpMessageHandler> mockHttpMessageHandler;
    private JiraClient jiraClient;

    [SetUp]
    public void Setup()
    {
        // Initialize mocks
        this.mockHttpClientFactory = new Mock<IHttpClientFactory>();
        this.mockLogger = new Mock<ILogger<JiraClient>>();
        this.mockHttpMessageHandler = new Mock<HttpMessageHandler>();

        // Setup HttpClient with mocked handler
        HttpClient httpClient = new HttpClient(this.mockHttpMessageHandler.Object);
        this.mockHttpClientFactory.Setup(factory => factory.CreateClient("Jira-Client"))
            .Returns(httpClient);

        // Create instance of JiraClient with mocked dependencies
        this.jiraClient = new JiraClient(this.mockHttpClientFactory.Object, this.mockLogger.Object);
    }

    [Test]
    public async Task SendRequestAsync_SuccessfulRequest_ReturnsResponse()
    {
        // Arrange
        const string expectedUri = "https://api.jira.com/test";
        HttpResponseMessage expectedResponse = new HttpResponseMessage(HttpStatusCode.OK);

        this.mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(expectedResponse);

        // Act
        HttpResponseMessage result = await jiraClient.SendRequestAsync(expectedUri);

        // Assert
        Assert.That(result, Is.EqualTo(expectedResponse));
        this.mockHttpMessageHandler.Protected().Verify(
            "SendAsync",
            Times.Once(),
            ItExpr.Is<HttpRequestMessage>(req => 
                req.Method == HttpMethod.Get && 
                req.RequestUri.ToString() == expectedUri),
            ItExpr.IsAny<CancellationToken>()
        );
    }

    [Test]
    public void SendRequestAsync_HttpRequestFails_LogsErrorAndThrowsException()
    {
        // Arrange
        const string expectedUri = "https://api.jira.com/test";
        HttpRequestException expectedException = new HttpRequestException("Network error");

        this.mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ThrowsAsync(expectedException);
        // Act & Assert
        HttpRequestException? exception = Assert.ThrowsAsync<HttpRequestException>(
            async () => await this.jiraClient.SendRequestAsync(expectedUri)
        );
        Assert.That(exception.Message, Is.EqualTo("Network error"));
        this.mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Network error")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()
            ),
            Times.Once
        );
    }

    [Test]
    public void SendRequestAsync_NonSuccessStatusCode_ThrowsHttpRequestException()
    {
        // Arrange
        const string expectedUri = "https://api.jira.com/test";
        HttpResponseMessage errorResponse = new HttpResponseMessage(HttpStatusCode.BadRequest);

        this.mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(errorResponse);

        // Act & Assert
        HttpRequestException? exception = Assert.ThrowsAsync<HttpRequestException>(
            async () => await this.jiraClient.SendRequestAsync(expectedUri)
        );

        Assert.That(exception, Is.Not.Null);
    }
}