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
        public async Task<IActionResult> Login([FromBody] LoginCredentials userParam)
        {
            var user = await UserService.Authenticate(userParam.Username, userParam.Password);

            if (user == null)
            {
                return BadRequest(new { message = "Username or password is incorrect" });
            }

            return Ok(user);
        }

        [HttpPost("")]
        public async Task<IActionResult> UpdateUser([FromBody] BaseUser newUser)
        {
            var updated = await UserService.UpdateUser(User.Identity.Name, newUser);
            if (updated)
            {
                return Ok(newUser);
            }
            return BadRequest(new { message = "Details are invalid" });
        }

        [HttpGet]
        public async Task<IActionResult> GetUser()
        {
            var user = await UserService.GetCurrentUser(User);
            if (user != null)
            {
                return Ok(new
                {
                    user.Id,
                    user.FirstName,
                    user.LastName,
                });
            }
            return Unauthorized();
        }

        [AllowAnonymous]
        [HttpPut]
        public async Task<IActionResult> RegisterUser([FromBody] BaseUser newUser)
        {
            var userDeets = await UserService.RegisterUser(newUser);
            return Created($"/users/{userDeets}", userDeets);
        }
    }
}