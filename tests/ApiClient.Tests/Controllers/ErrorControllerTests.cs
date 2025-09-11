namespace Neusta.Jira.Connector.ApiClient.Tests.Controllers;

using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Moq;
using Neusta.Jira.Connector.ApiClient.Controllers;
using Neusta.NIP.Shared.WebApi.Interfaces;
using NUnit.Framework;

[ExcludeFromCodeCoverage]
[TestFixture]
public class ErrorControllerTests
{
    private ErrorController controller = null!;

    private Mock<ICustomErrorHandler> mockedCustomErrorHandler = null!;
    private Mock<ProblemDetailsFactory> mockedProblemDetailsFactory = null!;

    [SetUp]
    public void SetUp()
    {
        this.mockedCustomErrorHandler = new Mock<ICustomErrorHandler>();
        this.mockedProblemDetailsFactory = new Mock<ProblemDetailsFactory>();

        this.controller = new ErrorController(this.mockedCustomErrorHandler.Object)
        {
            ProblemDetailsFactory = this.mockedProblemDetailsFactory.Object
        };
    }

    [Test]
    public void CallsCustomErrorHandler()
    {
        DefaultHttpContext context = new();
        this.controller.ControllerContext.HttpContext = context;

        this.controller.Error();

        this.mockedCustomErrorHandler.Verify(x => x.Handle("nus", context, this.mockedProblemDetailsFactory.Object));
    }

    [Test]
    public void ReturnsResultFromCustomErrorHandler()
    {
        ObjectResult expected = this.SetupCustomErrorHandlerHandle();

        ObjectResult objectResult = this.controller.Error();

        Assert.That(objectResult, Is.EqualTo(expected));
    }

    private ObjectResult SetupCustomErrorHandlerHandle()
    {
        ObjectResult objectResult = new(null);
        this.mockedCustomErrorHandler
            .Setup(x => x.Handle(It.IsAny<string>(), It.IsAny<HttpContext>(), It.IsAny<ProblemDetailsFactory>()))
            .Returns(objectResult);

        return objectResult;
    }
}