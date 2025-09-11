namespace Neusta.Jira.Connector.Infrastructure.AppPersistence.Entities;

using System.Diagnostics.CodeAnalysis;
using Neusta.Jira.Connector.Infrastructure.Redmine.Entities;

[ExcludeFromCodeCoverage]
public class UserEntity : BaseEntity
{
    public string Token { get; set; }

    public string Upn { get; set; }
}