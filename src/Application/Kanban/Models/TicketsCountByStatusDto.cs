namespace Neusta.Jira.Connector.Application.Kanban.Models;

using System.Diagnostics.CodeAnalysis;

[ExcludeFromCodeCoverage]
public class TicketsCountByStatusDto
{
    public int Done { get; set; }

    public int InProgress { get; set; }

    public int Todo { get; set; }

    public int Total { get; set; }
}