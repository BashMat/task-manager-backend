using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskManagerAPI.Dtos.Project;
using TaskManagerAPI.Models;
using TaskManagerAPI.Services;

namespace TaskManagerAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ProjectController : ControllerBase
	{
		private readonly IProjectService _projectService;

		public ProjectController(IProjectService projectService)
		{
			_projectService = projectService;
		}

		[HttpGet]
		public async Task<ActionResult<ServiceResponse<List<ProjectGetResponseDto>>>> GetAll()
		{
			return Ok(await _projectService.GetAll());
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<ServiceResponse<ProjectGetResponseDto>>> GetById(int id)
		{
			var response = await _projectService.GetById(id);
			if (response.Success)
			{
				return Ok(response);
			}
			return NotFound(response);
		}

		[HttpPost]
		public async Task<ActionResult<ServiceResponse<List<ProjectGetResponseDto>>>> Post(ProjectAddRequestDto p)
		{
			return Ok(await _projectService.Post(p));
		}

		[HttpPut]
		public async Task<ActionResult<ProjectGetResponseDto>> Put(ProjectUpdateRequestDto updatedProject)
		{
			var response = await _projectService.Put(updatedProject);
			if (response.Success)
			{
				return Ok(response);
			}
			return NotFound(response);
		}

		[HttpDelete("{id}")]
		public async Task<ActionResult<List<ProjectGetResponseDto>>> Delete(int id)
		{
			var response = await _projectService.Delete(id);
			if (response.Success)
			{
				return Ok(response);
			}
			return NotFound(response);
		}
	}
}
