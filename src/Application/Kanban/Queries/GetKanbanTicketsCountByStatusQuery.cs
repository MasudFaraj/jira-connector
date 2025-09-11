namespace Neusta.Jira.Connector.Application.Kanban.Queries;

using MediatR;
using Neusta.Jira.Connector.Application.Kanban.Interfaces;
using Neusta.Jira.Connector.Application.Kanban.Models;

public class GetKanbanTicketsCountByStatusQuery : IRequest<TicketsCountByStatusDto?>
{
    public string JiraKey { get; set; } = string.Empty;
    
    public class Handler : IRequestHandler<GetKanbanTicketsCountByStatusQuery, TicketsCountByStatusDto?>
    {
        private readonly IKanbanService kanbanService;

        public Handler(IKanbanService kanbanService)
        {
            this.kanbanService = kanbanService;
        }
        
        public Task<TicketsCountByStatusDto?> Handle(GetKanbanTicketsCountByStatusQuery request, CancellationToken cancellationToken)
        {
            return this.kanbanService.GetKanbanTicketsCountByStatusAsync(request.JiraKey, cancellationToken);
        }
    }
}