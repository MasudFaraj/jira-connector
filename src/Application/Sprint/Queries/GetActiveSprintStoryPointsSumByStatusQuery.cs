namespace Neusta.Jira.Connector.Application.Sprint.Queries;

using MediatR;
using Neusta.Jira.Connector.Application.Sprint.Interfaces;
using Neusta.Jira.Connector.Application.Sprint.Models;

public class GetActiveSprintStoryPointsSumByStatusQuery : IRequest<StoryPointsSumByStatusDto?>
{
    public string JiraKey { get; set; } = string.Empty;

    public class Handler : IRequestHandler<GetActiveSprintStoryPointsSumByStatusQuery, StoryPointsSumByStatusDto?>
    {
        private readonly IActiveSprintService activeSprintService;

        public Handler(IActiveSprintService activeSprintService)
        {
            this.activeSprintService = activeSprintService;
        }

        public Task<StoryPointsSumByStatusDto?> Handle(GetActiveSprintStoryPointsSumByStatusQuery request, CancellationToken cancellationToken)
        {
            return this.activeSprintService.GetActiveSprintStoryPointsSumByStatusAsync(request.JiraKey, cancellationToken);
        }
    }
}