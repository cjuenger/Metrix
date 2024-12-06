using AutoMapper;
using Io.Juenger.GitLabClient.Model;
using Metrix.Core.Aggregates;

namespace Io.Juenger.Scrum.GitLab.Mappings;

public class GitLabMappingProfile : Profile
{
    public GitLabMappingProfile()
    {
        CreateMap<Project, Product>().ReverseMap();
    }
}