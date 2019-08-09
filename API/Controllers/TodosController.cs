using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class TodosController : ControllerBase
    {
        public IUserService UserService { get; }

        public TodosController(IUserService userService)
        {
            UserService = userService;
        }
    }
}
