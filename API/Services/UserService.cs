using API.Exceptions;
using API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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
        Task<UserWithToken> Authenticate(string username, string password);
        Task<IEnumerable<BaseUser>> GetAll();
        Task<BaseUser> GetUserById(string id);
        Task<bool> UpdateUser(string id, BaseUser newUser);
        Task<string> RegisterUser(BaseUser newUser);
        Task<BaseUser> GetCurrentUser(ClaimsPrincipal controllerUser);
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

        public async Task<UserWithToken> Authenticate(string username, string password)
        {
            var user = await Context.Users.Where(x => x.Username == username && x.Password == password).FirstOrDefaultAsync();

            if (user == null) throw new NotFoundException("User does not exist");

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

        public async Task<IEnumerable<BaseUser>> GetAll()
        {
            return await Task.FromResult(Context.Users.AsEnumerable().Select(x =>
            {
                x.Password = null;
                return x;
            }));
        }

        public async Task<BaseUser> GetUserById(string userId)
        {
            var user = await Context.Users.Where(x => x.Id == userId).FirstOrDefaultAsync();
            if (user == null) throw new InvalidIdException();
            return user;
        }

        public async Task<BaseUser> GetCurrentUser(ClaimsPrincipal controllerUser)
        {
            var userId = controllerUser.Identity.Name;
            return await GetUserById(userId);
        }

        public async Task<bool> UpdateUser(string userId, BaseUser newUser)
        {
            var existing = await Context.Users.Where(x => x.Username == newUser.Username && x.Id != userId).AnyAsync();
            if (existing) return false;

            if (userId != newUser.Id) return false;

            Context.Users.Update(newUser);
            await Context.SaveChangesAsync();
            return true;
        }

        public async Task<string> RegisterUser(BaseUser newUser)
        {
            var existing = await Context.Users.Where(x => x.Username == newUser.Username).AnyAsync();
            if (existing) throw new EntryExistsException();

            var entry = await Context.Users.AddAsync(newUser);
            await Context.SaveChangesAsync();
            return entry.Entity.Id.ToString();
        }
    }
}
