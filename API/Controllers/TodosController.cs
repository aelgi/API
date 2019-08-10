using API.Models;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers
{
    public class TodoUpdateFields
    {
        public bool? Completed { get; set; }
        public string Name { get; set; }
    }

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

        [HttpPost("{itemId}")]
        public async Task<IActionResult> UpdateItem(string projectId, string itemId, [FromBody] TodoUpdateFields newItem)
        {
            var user = await UserService.GetCurrentUser(User);
            var project = await ProjectService.GetProjectById(user, projectId);
            var item = await TodoService.GetItemById(project, itemId);

            if (newItem.Completed.HasValue)
            {
                await TodoService.UpdateCompletedStatus(item, newItem.Completed.Value);
                item = await TodoService.GetItemById(project, itemId);
            }

            if (newItem.Name != null)
            {
                await TodoService.UpdateName(item, newItem.Name);
                item = await TodoService.GetItemById(project, itemId);
            }

            return Ok(item);
        }
    }
}
