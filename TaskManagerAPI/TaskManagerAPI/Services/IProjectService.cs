using Microsoft.AspNetCore.Mvc;
using TaskManagerAPI.Models;
using TaskManagerAPI.Dtos.Project;

namespace TaskManagerAPI.Services
{
	public interface IProjectService
	{
		public Task<ServiceResponse<List<ProjectGetResponseDto>>> GetAll();

		public Task<ServiceResponse<ProjectGetResponseDto>> GetById(int id);

		public Task<ServiceResponse<List<ProjectGetResponseDto>>> Create(ProjectAddRequestDto p);

		public Task<ServiceResponse<ProjectGetResponseDto>> Update(ProjectUpdateRequestDto updatedProject);

		public Task<ServiceResponse<List<ProjectGetResponseDto>>> Delete(int id);
	}
}
