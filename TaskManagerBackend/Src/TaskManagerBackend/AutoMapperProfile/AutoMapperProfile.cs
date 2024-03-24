using AutoMapper;
using TaskManagerBackend.Domain.Models;
using TaskManagerBackend.Dto.Project;

namespace TaskManagerBackend.AutoMapperProfile
{
	public class AutoMapperProfile : Profile
	{
		public AutoMapperProfile()
		{
			CreateMap<Project, ProjectGetResponseDto>();
			CreateMap<ProjectAddRequestDto, Project>();
			CreateMap<ProjectUpdateRequestDto, Project>();
		}
	}
}
