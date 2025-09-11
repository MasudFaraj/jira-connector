namespace Neusta.Jira.Connector.Application.Sprint.Queries;

using MediatR;
using Neusta.Jira.Connector.Application.Kanban.Models;
using Neusta.Jira.Connector.Application.Sprint.Interfaces;

public class GetActiveSprintTicketsCountByStatusQuery : IRequest<TicketsCountByStatusDto?>
{
    public string JiraKey { get; set; } = string.Empty;

    public class Handler : IRequestHandler<GetActiveSprintTicketsCountByStatusQuery, TicketsCountByStatusDto?>
    {
        private readonly IActiveSprintService? activeSprintService;

        public Handler(IActiveSprintService? activeSprintService)
        {
            this.activeSprintService = activeSprintService;
        }

        public Task<TicketsCountByStatusDto?> Handle(GetActiveSprintTicketsCountByStatusQuery request, CancellationToken cancellationToken)
        {
            return this.activeSprintService.GetActiveSprintTicketsCountByStatusAsync(request.JiraKey, cancellationToken);
        }
    }
}