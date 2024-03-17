using AutoMapper;
using TaskManagerApi.Domain.Models;
using TaskManagerApi.Dto.Project;

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
