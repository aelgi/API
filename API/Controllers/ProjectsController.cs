using API.Models;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class ProjectsController : ControllerBase
    {
        public IUserService UserService { get; }
        public IProjectService ProjectService { get; }

        public ProjectsController(IUserService userService, IProjectService projectService)
        {
            UserService = userService;
            ProjectService = projectService;
        }

        [HttpGet("")]
        public async Task<IActionResult> GetProjects()
        {
            var user = await UserService.GetCurrentUser(User);
            var projects = await ProjectService.GetAllProjects(user);
            return Ok(projects.Select(x => new
            {
                x.Id,
                x.Name,
            }));
        }

        [HttpGet("{projectId}")]
        public async Task<IActionResult> GetProject(string projectId)
        {
            var user = await UserService.GetCurrentUser(User);
            var project = await ProjectService.GetProjectById(user, projectId);
            return Ok(project);
        }

        [HttpPut("")]
        public async Task<IActionResult> CreateProject([FromBody] Project newProject)
        {
            var user = await UserService.GetCurrentUser(User);
            var id = await ProjectService.CreateProject(user, newProject);
            return Created($"/projects/{id}", id);
        }
    }
}
