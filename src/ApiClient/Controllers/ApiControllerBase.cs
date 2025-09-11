namespace Neusta.Jira.Connector.ApiClient.Controllers;

using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public abstract class ApiControllerBase : ControllerBase
{
    protected ApiControllerBase(ISender sender)
    {
        this.Sender = sender;
    }

    protected ISender Sender { get; }
}