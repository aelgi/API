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
        public IActionResult GetProjects()
        {
            var user = UserService.GetCurrentUser(User);
            var projects = ProjectService.GetAllProjects(user).Select(x => new {
                Id = x.Id,
                Name = x.Name
            });
            return Ok(projects);
        }

        [HttpGet("{projectId}")]
        public IActionResult GetProject(string projectId)
        {
            var user = UserService.GetCurrentUser(User);
            var project = ProjectService.GetProjectById(user, projectId);
            if (project == null)
                return NotFound();
            return Ok(project);
        }

        [HttpPut("")]
        public IActionResult CreateProject([FromBody] Project newProject)
        {
            var user = UserService.GetCurrentUser(User);
            var id = ProjectService.CreateProject(user, newProject);
            return Created($"/projects/{id}", id);
        }
    }
}
