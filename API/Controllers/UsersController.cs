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

        [HttpPost("")]
        public IActionResult UpdateUser([FromBody] BaseUser newUser)
        {
            var updated = UserService.UpdateUser(User.Identity.Name, newUser);
            if (updated)
            {
                return Ok(newUser);
            }
            return BadRequest(new { message = "Details are invalid" });
        }

        [HttpGet]
        public IActionResult GetUser()
        {
            var userId = User.Identity.Name;
            var user = UserService.GetUserById(userId);
            if (user != null)
            {
                return Ok(user);
            }
            return Unauthorized();
        }

        [AllowAnonymous]
        [HttpPut]
        public IActionResult RegisterUser([FromBody] BaseUser newUser)
        {
            var userDeets = UserService.RegisterUser(newUser);
            if (userDeets == null)
            {
                return BadRequest();
            }
            return Created($"/users/{userDeets}", userDeets);
        }
    }
}