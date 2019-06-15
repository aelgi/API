using System;
using Microsoft.EntityFrameworkCore;

namespace API.Models
{
    public class GroceryListContext : DbContext
    {
        public GroceryListContext(DbContextOptions<GroceryListContext> options) : base(options) { }

        public DbSet<GroceryItem> GroceryList { get; set; }
    }
}
