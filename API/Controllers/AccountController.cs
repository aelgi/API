using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using JWT.Algorithms;
using JWT.Serializers;
using JWT;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using API.Models;
using System.Collections.Generic;

namespace API.Controllers
{
    [Route("api/[controller]")]
    public class AccountController
    {
        public AccountController(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IOptions<JWTSettings> optionsAccessor)
        {
            UserManager = userManager;
            SignInManager = signInManager;
            OptionsAccessor = optionsAccessor.Value;
        }

        private UserManager<IdentityUser> UserManager { get; }
        private SignInManager<IdentityUser> SignInManager { get; }
        private JWTSettings OptionsAccessor { get; }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] Credentials credentials)
        {
            var user = new IdentityUser
            {
                UserName = credentials.Email,
                Email = credentials.Email
            };
            var result = await UserManager.CreateAsync(user, credentials.Password);
            if (result.Succeeded)
            {
                await SignInManager.SignInAsync(user, false);
                return new JsonResult(new Dictionary<string, object>
                {
                    { "access_token", GetAccessToken(credentials.Email) },
                    { "id_token", GetIdToken(user) }
                });
            }
            return Errors(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] Credentials creds)
        {
            var result = await SignInManager.PasswordSignInAsync(creds.Email, creds.Password, false, false);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByEmailAsync(creds.Email);
                return new JsonResult(new Dictionary<string, object>
                {
                    { "access_token", GetAccessToken(creds.Email) },
                    { "id_token", GetIdToken(user) }
                });
            }
            return new JsonResult("Unable to sign in") { StatusCode = 401 };
        }   

        private string GetIdToken(IdentityUser user)
        {
            var payload = new Dictionary<string, object>
            {
                { "id", user.Id },
                { "sub", user.Email },
                { "email", user.Email },
                { "emailConfirmed", user.EmailConfirmed },
            };
            return GetToken(payload);
        }

        private string GetAccessToken(string email)
        {
            var payload = new Dictionary<string, object> {
                { "sub", email },
                { "email", email }
            };
            return GetToken(payload);
        }

        private string GetToken(Dictionary<string, object> payload)
        {
            var secret = OptionsAccessor.SecretKey;

            payload.Add("iss", OptionsAccessor.Issuer);
            payload.Add("aud", OptionsAccessor.Audience);
            payload.Add("nbf", ConvertToUnixTimestamp(DateTime.Now));
            payload.Add("iat", ConvertToUnixTimestamp(DateTime.Now));
            payload.Add("exp", ConvertToUnixTimestamp(DateTime.Now.AddDays(7)));
            IJwtAlgorithm algorithm = new HMACSHA256Algorithm();
            IJsonSerializer serializer = new JsonNetSerializer();
            IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
            IJwtEncoder encoder = new JwtEncoder(algorithm, serializer, urlEncoder);

            return encoder.Encode(payload, secret);
        }

        private JsonResult Errors(IdentityResult result)
        {
            var items = result.Errors.Select(x => x.Description).ToArray();
            return new JsonResult(items) { StatusCode = 400 };
        }

        private static double ConvertToUnixTimestamp(DateTime date)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan diff = date.ToUniversalTime() - origin;
            return Math.Floor(diff.TotalSeconds);
        }
    }
}
