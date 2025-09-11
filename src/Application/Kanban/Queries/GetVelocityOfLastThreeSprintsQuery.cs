namespace Neusta.Jira.Connector.Application.Kanban.Queries;

using MediatR;
using Neusta.Jira.Connector.Application.Kanban.Interfaces;
using Neusta.Jira.Connector.Application.Kanban.Models;

public class GetVelocityOfLastThreeSprintsQuery : IRequest<VelocityDto?>
{
    public string JiraKey { get; set; } = string.Empty;
    
    public class Handler : IRequestHandler<GetVelocityOfLastThreeSprintsQuery, VelocityDto?>
    {
        private readonly IKanbanService kanbanService;

        public Handler(IKanbanService kanbanService)
        {
            this.kanbanService = kanbanService;
        }
        
        public Task<VelocityDto?> Handle(GetVelocityOfLastThreeSprintsQuery request, CancellationToken cancellationToken)
        {
            return this.kanbanService.GetVelocityOfLastThreeSprintsAsync(request.JiraKey, cancellationToken);

        }
    }
}