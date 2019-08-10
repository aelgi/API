using API.Exceptions;
using API.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Services
{
    public interface ITodoService
    {
        Task<IEnumerable<Item>> GetAllItems(Project project);
        Task<Item> GetItemById(Project project, string itemId);
        Task<string> CreateItem(Project project, Item item);
        Task UpdateCompletedStatus(Item item, bool status);
        Task UpdateName(Item item, string name);
    }

    public class TodoService : ITodoService
    {
        protected Context Context { get; }
        public TodoService(Context context)
        {
            Context = context;
        }

        public async Task<IEnumerable<Item>> GetAllItems(Project project)
        {
            return await Task.FromResult(Context.Items.Where(x => x.ProjectId == project.Id).AsEnumerable());
        }

        public async Task<Item> GetItemById(Project project, string itemId)
        {
            var item = await Context.Items.Where(x => x.Id == itemId).FirstOrDefaultAsync();
            if (item == null) throw new NotFoundException("Could not find item");
            if (item.ProjectId != project.Id) throw new NotFoundException("Item is not attached to a project");

            return item;
        }

        public async Task<string> CreateItem(Project project, Item item)
        {
            item.ProjectId = project.Id;
            var entity = await Context.Items.AddAsync(item);
            await Context.SaveChangesAsync();
            return entity.Entity.Id;
        }

        public async Task UpdateCompletedStatus(Item item, bool status)
        {
            item.Completed = status;
            Context.Items.Update(item);
            await Context.SaveChangesAsync();
        }

        public async Task UpdateName(Item item, string name)
        {
            item.Name = name;
            Context.Items.Update(item);
            await Context.SaveChangesAsync();
        }
    }
}
