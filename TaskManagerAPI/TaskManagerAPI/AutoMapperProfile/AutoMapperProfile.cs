using AutoMapper;
using TaskManagerApi.Domain.Dtos.Project;
using TaskManagerApi.Domain.Models;

namespace TaskManagerApi.AutoMapperProfile
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
