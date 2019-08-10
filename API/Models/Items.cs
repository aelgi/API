using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models
{
    public class Item
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool Completed { get; set; }

        public string ProjectId { get; set; }
    }
}
