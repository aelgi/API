using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Models;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class LoginCredentials
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        public UsersController(IUserService userService)
        {
            UserService = userService;
        }

        public IUserService UserService { get; }

        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginCredentials userParam)
        {
            var user = UserService.Authenticate(userParam.Username, userParam.Password);

            if (user == null)
            {
                return BadRequest(new { message = "Username or password is incorrect" });
            }

            return Ok(user);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var users = UserService.GetAll();
            return Ok(users);
        }
    }
}