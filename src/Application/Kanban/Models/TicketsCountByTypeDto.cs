namespace Neusta.Jira.Connector.Application.Kanban.Models;

using System.Diagnostics.CodeAnalysis;

[ExcludeFromCodeCoverage]
public class TicketsCountByTypeDto
{
    public int Bug { get; set; }

    public int Story { get; set; }

    public int Task { get; set; }
}