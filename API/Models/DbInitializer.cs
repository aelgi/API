using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models
{
    public static class DbInitializer
    {
        public static void Initialize(Context context)
        {
            context.Database.EnsureCreated();

            if (context.Users.Any())
            {
                return;
            }

            var users = new BaseUser[]
            {
                new BaseUser { FirstName = "Test", LastName = "User", Username = "test", Password = "test" },
            };

            foreach(BaseUser u in users)
            {
                context.Users.Add(u);
            }
            context.SaveChanges();
        }
    }
}
