using API.Models;
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
    }
}
