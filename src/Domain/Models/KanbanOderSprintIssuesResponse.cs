namespace Neusta.Jira.Connector.Domain.Models;

using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

[ExcludeFromCodeCoverage]
public class KanbanOderSprintIssuesResponse
{
    public string? Expand { get; set; }

    public List<Issue>? Issues { get; set; }

    public int MaxResults { get; set; }

    public int StartAt { get; set; }

    public int Total { get; set; }

    public class Issue
    {
        public string? Expand { get; set; }

        public Fields? Fields { get; set; }

        public string? Id { get; set; }

        public string? Key { get; set; }

        public string? Self { get; set; }
    }

    public class Fields
    {
        public IssueType IssueType { get; set; } = new();

        public Status? Status { get; set; }

        [JsonProperty("customfield_10006")]
        public decimal? StoryPoints { get; set; }
    }

    public class Status
    {
        public string? Description { get; set; }

        public string? IconUrl { get; set; }

        public string? Id { get; set; }

        public string? Name { get; set; }

        public string? Self { get; set; }

        public StatusCategory? StatusCategory { get; set; }
    }

    public class StatusCategory
    {
        public string? ColorName { get; set; }

        public int Id { get; set; }

        public string? Key { get; set; }

        public string? Name { get; set; }

        public string? Self { get; set; }
    }

    public class IssueType
    {
        public string? Description { get; set; }

        public int Id { get; set; }

        public string? Name { get; set; }

        public string? Self { get; set; }

        public bool SubTask { get; set; }
    }
}