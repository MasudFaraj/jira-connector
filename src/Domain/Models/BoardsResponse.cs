namespace Neusta.Jira.Connector.Domain.Models;

using System.Diagnostics.CodeAnalysis;

[ExcludeFromCodeCoverage]
public class BoardsResponse
{
    public bool IsLast { get; set; }

    public int MaxResults { get; set; }

    public int StartAt { get; set; }

    public JiraBoard[]? Values { get; set; }
    
    public class JiraBoard
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public string? Self { get; set; }

        public string? Type { get; set; }
    }
}