namespace Neusta.Jira.Connector.Application.Common.Interfaces;

public interface ICurrentUserService
{
    bool IsApplication { get; }

    string Upn { get; }

    int UserId { get; }
}