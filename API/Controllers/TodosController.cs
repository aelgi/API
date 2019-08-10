using API.Models;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Authorize]
    [Route("projects/{projectId}/[controller]")]
    [ApiController]
    public class TodosController : ControllerBase
    {
        public IUserService UserService { get; }
        public IProjectService ProjectService { get; }
        public ITodoService TodoService { get; }

        public TodosController(IUserService userService, IProjectService projectService, ITodoService todoService)
        {
            UserService = userService;
            ProjectService = projectService;
            TodoService = todoService;
        }

        [HttpGet("")]
        public async Task<IActionResult> GetItems(string projectId)
        {
            var user = await UserService.GetCurrentUser(User);
            var project = await ProjectService.GetProjectById(user, projectId);
            var items = await TodoService.GetAllItems(project);
            return Ok(items.Select(x => new { x.Id, x.Name, x.Completed }));
        }

        [HttpPut("")]
        public async Task<IActionResult> CreateItem(string projectId, [FromBody] Item newItem)
        {
            var user = await UserService.GetCurrentUser(User);
            var project = await ProjectService.GetProjectById(user, projectId);
            var itemId = await TodoService.CreateItem(project, newItem);
            return Created($"/projects/{projectId}/todos/{itemId}", itemId);
        }
    }
}
