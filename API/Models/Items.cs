using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models
{
    public class Items
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Completed { get; set; }

        public int ProjectId { get; set; }
        public Project Project { get; set; }
    }
}
