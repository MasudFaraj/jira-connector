namespace Neusta.Jira.Connector.Application.Sprint.Queries;

using MediatR;
using Neusta.Jira.Connector.Application.Sprint.Interfaces;
using Neusta.Jira.Connector.Application.Sprint.Models;

public class GetActiveSprintDataQuery : IRequest<SprintDto?>
{
    public string JiraKey { get; set; } = string.Empty;

    public class Handler : IRequestHandler<GetActiveSprintDataQuery, SprintDto?>
    {
        private readonly IActiveSprintService activeSprintService;

        public Handler(IActiveSprintService activeSprintService)
        {
            this.activeSprintService = activeSprintService;
        }

        public Task<SprintDto?> Handle(GetActiveSprintDataQuery request, CancellationToken cancellationToken)
        {
            return this.activeSprintService.GetActiveSprintDataByJiraKeyAsync(request.JiraKey, cancellationToken);
        }
    }
}