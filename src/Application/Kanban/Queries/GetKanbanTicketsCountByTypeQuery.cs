namespace Neusta.Jira.Connector.Application.Kanban.Queries;

using MediatR;
using Neusta.Jira.Connector.Application.Kanban.Interfaces;
using Neusta.Jira.Connector.Application.Kanban.Models;

public class GetKanbanTicketsCountByTypeQuery : IRequest<TicketsCountByTypeDto?>
{
    public string JiraKey { get; set; } = string.Empty;
    
    public class Handler : IRequestHandler<GetKanbanTicketsCountByTypeQuery, TicketsCountByTypeDto?>
    {
        private readonly IKanbanService kanbanService;

        public Handler(IKanbanService kanbanService)
        {
            this.kanbanService = kanbanService;
        }
        
        public Task<TicketsCountByTypeDto?> Handle(GetKanbanTicketsCountByTypeQuery request, CancellationToken cancellationToken)
        {
            return this.kanbanService.GetKanbanTicketsCountByTypeAsync(request.JiraKey, cancellationToken);
        }
    }
}