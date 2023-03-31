﻿using AutoMapper;
using Io.Juenger.GitLabClient.Model;
using Io.Juenger.Scrum.GitLab.Aggregates;

namespace Io.Juenger.Scrum.GitLab.Mappings;

public class GitLabMappingProfile : Profile
{
    public GitLabMappingProfile()
    {
        CreateMap<Project, ProductAggregate>().ReverseMap();
    }
}