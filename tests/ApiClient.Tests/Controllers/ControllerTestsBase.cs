namespace Neusta.Jira.Connector.ApiClient.Tests.Controllers;

using System.Diagnostics.CodeAnalysis;
using MediatR;
using Moq;
using NUnit.Framework;

[ExcludeFromCodeCoverage]
public class ControllerTestsBase
{
    protected Mock<IMediator> mockedMediator = null!;

    [SetUp]
    public void BaseSetUp()
    {
        this.mockedMediator = new Mock<IMediator>();
    }

    protected void SetupMediatorSend<TResponse>(TResponse expected)
    {
        this.mockedMediator.Setup(x => x.Send(It.IsAny<IRequest<TResponse>>(), default)).ReturnsAsync(expected);
    }
}