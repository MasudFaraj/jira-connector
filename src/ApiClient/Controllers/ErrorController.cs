namespace Neusta.Jira.Connector.ApiClient.Controllers;

using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc;
using Neusta.NIP.Shared.WebApi.Interfaces;

#pragma warning disable CS1591
[ApiController]
[ApiExplorerSettings(IgnoreApi = true)]
[ExcludeFromCodeCoverage]
public class ErrorController : ControllerBase
{
    private readonly ICustomErrorHandler customErrorHandler;

    public ErrorController(ICustomErrorHandler customErrorHandler)
    {
        this.customErrorHandler = customErrorHandler;
    }

    [Route("/Error")]
    public ObjectResult Error()
    {
        return this.customErrorHandler.Handle("nus", this.HttpContext, this.ProblemDetailsFactory);
    }
}