namespace Neusta.Jira.Connector.Infrastructure.Redmine.Entities;

using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

[ExcludeFromCodeCoverage]
public class BaseEntity
{
    [Key]
    public int Id { get; set; }
}