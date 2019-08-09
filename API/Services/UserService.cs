using API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace API.Services
{
    public interface IUserService
    {
        UserWithToken Authenticate(string username, string password);
        IEnumerable<BaseUser> GetAll();
        BaseUser GetUserById(string id);
        bool UpdateUser(string id, BaseUser newUser);
        string RegisterUser(BaseUser newUser);
        BaseUser GetCurrentUser(ClaimsPrincipal controllerUser);
    }
    public class UserService : IUserService
    {
        private readonly AppSettings _appSettings;
        public Context Context { get; }

        public UserService(IOptions<AppSettings> appSettings, Context context)
        {
            _appSettings = appSettings.Value;
            Context = context;
        }

        public UserWithToken Authenticate(string username, string password)
        {
            var user = Context.Users.Where(x => x.Username == username && x.Password == password).FirstOrDefault();

            if (user == null) return null;

            var tokenUser = new UserWithToken(user);

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[] {
                    new Claim(ClaimTypes.Name, user.Id.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            tokenUser.Token = tokenHandler.WriteToken(token);

            tokenUser.Password = null;

            return tokenUser;
        }

        public IEnumerable<BaseUser> GetAll()
        {
            return Context.Users.AsEnumerable().Select(x =>
            {
                x.Password = null;
                return x;
            });
        }

        public BaseUser GetUserById(string userId)
        {
            try
            {
                var id = int.Parse(userId);
                return Context.Users.Find(id);
            }
            catch(Exception ex)
            {
                return null;
            }
        }

        public BaseUser GetCurrentUser(ClaimsPrincipal controllerUser)
        {
            var userId = controllerUser.Identity.Name;
            return this.GetUserById(userId);
        }

        public bool UpdateUser(string userId, BaseUser newUser)
        {
            try
            {
                var id = int.Parse(userId);
                var existing = Context.Users.Where(x => x.Username == newUser.Username && x.Id != id).Any();
                if (existing)
                {
                    return false;
                }

                if (id != newUser.Id)
                {
                    return false;
                }

                Context.Users.Update(newUser);
                Context.SaveChanges();
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        public string RegisterUser(BaseUser newUser)
        {
            var existing = Context.Users.Where(x => x.Username == newUser.Username).Any();
            if (existing)
            {
                return null;
            }
            newUser.Projects = new List<Project>();

            var entry = Context.Users.Add(newUser);
            Context.SaveChanges();
            return entry.Entity.Id.ToString();
        }
    }
}
