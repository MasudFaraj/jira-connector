namespace Neusta.Jira.Connector.Application.Sprint.Models;

using System.Diagnostics.CodeAnalysis;

[ExcludeFromCodeCoverage]
public class SprintDto
{
    public DateTime EndDate { get; set; }

    public string? Goal { get; set; }

    public int Id { get; set; }

    public string? Name { get; set; }

    public DateTime StartDate { get; set; }
}