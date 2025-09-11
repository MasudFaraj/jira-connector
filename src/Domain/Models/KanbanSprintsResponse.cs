namespace Neusta.Jira.Connector.Domain.Models;

using System.Diagnostics.CodeAnalysis;

[ExcludeFromCodeCoverage]
public class KanbanSprintsResponse
{
    public bool IsLast { get; set; }

    public int MaxResults { get; set; }

    public int StartAt { get; set; }

    public List<Sprint>? Values { get; set; }
    
    public class Sprint
    {
        public DateTime ActivatedDate { get; set; }

        public DateTime EndDate { get; set; }

        public string? Goal { get; set; }

        public int Id { get; set; }

        public string? Name { get; set; }

        public int OriginBoardId { get; set; }

        public DateTime StartDate { get; set; }

        public string? State { get; set; }
    }
}