using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models
{
    public class BaseUser
    {
        public BaseUser(BaseUser user)
        {
            Id = user.Id;
            FirstName = user.FirstName;
            LastName = user.LastName;
            Username = user.Username;
            Password = user.Password;
        }
        public BaseUser() { }

        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
    public class UserWithToken : BaseUser
    {
        public UserWithToken(BaseUser user) : base(user) { }
        public UserWithToken() { }

        public string Token { get; set; }
    }
}
