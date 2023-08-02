using AutoMapper;
using TaskManagerAPI.Dtos.Project;
using TaskManagerAPI.Models;

namespace TaskManagerAPI.AutoMapperProfile
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
