using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models
{
    public class Project
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public int BaseUserId { get; set; }
        public BaseUser User { get; set; }

        public List<Item> Items { get; set; }
    }
}
