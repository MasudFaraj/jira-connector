namespace Neusta.Jira.Connector.Application.Sprint.Models;

using System.Diagnostics.CodeAnalysis;

[ExcludeFromCodeCoverage]
public class StoryPointsSumByStatusDto
{
    public decimal Done { get; set; }

    public decimal InProgress { get; set; }

    public decimal Planned { get; set; }

    public decimal Todo { get; set; }
}