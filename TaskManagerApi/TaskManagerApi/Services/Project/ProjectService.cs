using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagerApi.Data;
using TaskManagerApi.Domain.Dtos.Project;
using TaskManagerApi.Domain.Models;
using TaskManagerApi.Domain;

namespace TaskManagerApi.Services.Project
{
	public class ProjectService : IProjectService
	{
		private readonly IMapper _mapper;
		private readonly DataContext _dataContext;

		public ProjectService(IMapper mapper, DataContext dataContext)
		{
			_mapper = mapper;
			_dataContext = dataContext;
		}

		async Task<ServiceResponse<List<ProjectGetResponseDto>>> IProjectService.GetAll()
		{
			var response = new ServiceResponse<List<ProjectGetResponseDto>>();
			var projects = await _dataContext.Projects.ToListAsync();
			response.Data = projects.Select(p => _mapper.Map<ProjectGetResponseDto>(p)).ToList();
			return response;
		}

		async Task<ServiceResponse<ProjectGetResponseDto>> IProjectService.GetById(int id)
		{
			var response = new ServiceResponse<ProjectGetResponseDto>();
			var projects = await _dataContext.Projects.ToListAsync();
			response.Data = _mapper.Map<ProjectGetResponseDto>(projects.Find(p => p.Id == id));
			if (response.Data is null)
			{
				response.Success = false;
				response.Message = $"Item with ID '{id}' not found";
			}
			return response;
		}

		async Task<ServiceResponse<List<ProjectGetResponseDto>>> IProjectService.Create(ProjectAddRequestDto p)
		{
			var response = new ServiceResponse<List<ProjectGetResponseDto>>();
			var newProject = _mapper.Map<Domain.Models.Project>(p);
			await _dataContext.Projects.AddAsync(newProject);
			await _dataContext.SaveChangesAsync();
			var projects = await _dataContext.Projects.ToListAsync();
			response.Data = projects.Select(p => _mapper.Map<ProjectGetResponseDto>(p)).ToList();
			return response;
		}

		async Task<ServiceResponse<ProjectGetResponseDto>> IProjectService.Update(ProjectUpdateRequestDto updatedProject)
		{
			var response = new ServiceResponse<ProjectGetResponseDto>();
			var projects = await _dataContext.Projects.ToListAsync();
			var project = projects.Find(p => p.Id == updatedProject.Id);
			if (project is null)
			{
				response.Data = null;
				response.Success = false;
				response.Message = $"Item with ID '{updatedProject.Id}' not found";
			}
			else
			{
				project.Name = updatedProject.Name;
				project.Description = updatedProject.Description;
				await _dataContext.SaveChangesAsync();
				response.Data = _mapper.Map<ProjectGetResponseDto>(project);
			}
			return response;
		}

		async Task<ServiceResponse<List<ProjectGetResponseDto>>> IProjectService.Delete(int id)
		{
			var response = new ServiceResponse<List<ProjectGetResponseDto>>();
			var projects = await _dataContext.Projects.ToListAsync();
			var project = projects.Find(p => p.Id == id);
			if (project is null)
			{
				response.Success = false;
				response.Message = $"Item with ID '{id}' not found";
			}
			else
			{
				_dataContext.Projects.Remove(project);
				await _dataContext.SaveChangesAsync();
				projects.Remove(project);
				response.Data = projects.Select(p => _mapper.Map<ProjectGetResponseDto>(p)).ToList();
			}
			return response;
		}
	}
}
