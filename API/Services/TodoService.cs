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
            var projectItems = await Context.Projects.Include(x => x.Items).Where(x => x.Id == project.Id).FirstOrDefaultAsync();
            if (projectItems == null) throw new NotFoundException("Could not find project");
            return projectItems.Items;
        }

        public async Task<Item> GetItemById(Project project, string itemId)
        {
            int id;

            try
            {
                id = int.Parse(itemId);
            }
            catch(Exception)
            {
                throw new InvalidIdException();
            }

            var item = await Context.Items.FindAsync(id);
            if (item == null) throw new NotFoundException("Could not locate item");
            return item;
        }

        public async Task<string> CreateItem(Project project, Item item)
        {
            var linkedUser = await Context.Users.Include(x => x.Projects).ThenInclude(x => x.Items).Where(x => x.Id == project.BaseUserId).FirstAsync();
            if (linkedUser == null) throw new NotFoundException("Could not find user");
            var linkedProject = linkedUser.Projects.Find(x => x.Id == project.Id);
            if (linkedProject == null) throw new NotFoundException("Could not find referenced project");

            linkedProject.Items.Add(item);
            Context.Users.Update(linkedUser);
            await Context.SaveChangesAsync();
            return item.Id.ToString();
        }
    }
}
