using Microsoft.AspNetCore.Mvc;
using TaskManagerAPI.Models;
using TaskManagerAPI.Dtos.Project;

namespace TaskManagerAPI.Services
{
	public interface IProjectService
	{
		public Task<ServiceResponse<List<ProjectGetResponseDto>>> GetAll();

		public Task<ServiceResponse<ProjectGetResponseDto>> GetById(int id);

		public Task<ServiceResponse<List<ProjectGetResponseDto>>> Post(ProjectAddRequestDto p);

		public Task<ServiceResponse<ProjectGetResponseDto>> Put(ProjectUpdateRequestDto updatedProject);

		public Task<ServiceResponse<List<ProjectGetResponseDto>>> Delete(int id);
	}
}
