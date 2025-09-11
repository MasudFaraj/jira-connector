namespace Neusta.Jira.Connector.ApiClient.Services;

using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Neusta.Jira.Connector.Application.Common.Interfaces;

[ExcludeFromCodeCoverage]
public class CurrentUserService : ICurrentUserService
{
    private const string UpnClaim = "upn";
    private readonly IHttpContextAccessor httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        this.httpContextAccessor = httpContextAccessor;
    }

    public bool IsApplication => this.AccessAsApplication();

    public string Upn
    {
        get
        {
            if (this.AccessAsApplication())
            {
                return string.Empty;
            }

            string? upnClaim = this.httpContextAccessor?.HttpContext?.User.FindFirstValue(UpnClaim);
            string? upnSchemaClaim = this.httpContextAccessor?.HttpContext?.User.FindFirstValue(ClaimTypes.Upn);
            string? upn = upnClaim ?? upnSchemaClaim;

            if (string.IsNullOrEmpty(upn))
            {
                throw new SecurityTokenException("Upn is empty");
            }

            return upn;
        }
    }

    public int UserId => 1;

    private bool AccessAsApplication()
    {
        Claim? claim = this.httpContextAccessor?.HttpContext?.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role);
        return claim?.Value == "access_as_application";
    }
}