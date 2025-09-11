namespace Neusta.Jira.Connector.Application.Sprint.Mapping;

using System.Diagnostics.CodeAnalysis;
using AutoMapper;
using Neusta.Jira.Connector.Application.Sprint.Models;
using Neusta.Jira.Connector.Domain.Models;

[ExcludeFromCodeCoverage]
public class SprintDToProfile : Profile
{
    public SprintDToProfile()
    {
        CreateMap<KanbanSprintsResponse.Sprint, SprintDto>().ReverseMap();
    }

}