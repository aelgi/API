using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options) : base(options) { }

        public DbSet<BaseUser> Users { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Item> Items { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<BaseUser>().Property<DateTime>("UpdatedTimestamp");
            builder.Entity<Project>().Property<DateTime>("UpdatedTimestamp");
            builder.Entity<Item>().Property<DateTime>("UpdatedTimestamp");

            base.OnModelCreating(builder);
        }

        public override int SaveChanges()
        {
            ChangeTracker.DetectChanges();

            updateUpdatedProperty<BaseUser>();
            updateUpdatedProperty<Project>();
            updateUpdatedProperty<Item>();

            return base.SaveChanges();
        }

        private void updateUpdatedProperty<T>() where T : class
        {
            var modifiedSourceInfo = ChangeTracker.Entries<T>().Where(x => x.State == EntityState.Added || x.State == EntityState.Modified);

            foreach(var entry in modifiedSourceInfo)
            {
                entry.Property("UpdatedTimestamp").CurrentValue = DateTime.UtcNow;
            }
        }
    }
}
