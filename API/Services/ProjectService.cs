using API.Exceptions;
using API.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Services
{
    public interface IProjectService
    {
        Task<IEnumerable<Project>> GetAllProjects(BaseUser user);
        Task<Project> GetProjectById(BaseUser user, string projectId);
        Task<string> CreateProject(BaseUser user, Project project);
    }

    public class ProjectService : IProjectService
    {
        protected Context Context { get; }

        public ProjectService(Context context)
        {
            Context = context;
        }

        public async Task<IEnumerable<Project>> GetAllProjects(BaseUser user)
        {
            var userProjects = await Context.Users.Include(x => x.Projects).Where(x => x.Id == user.Id).FirstAsync();
            return userProjects.Projects;
        }

        public async Task<Project> GetProjectById(BaseUser user, string projectId)
        {
            try
            {
                var id = int.Parse(projectId);
                var project = await Context.Projects.Include(x => x.Items).Where(x => x.Id == id).FirstOrDefaultAsync();
                if (project == null) throw new NotFoundException("Could not find project");
                if (project.BaseUserId != user.Id) throw new NotFoundException("Project is not associated with user");
                project.User = null;
                return project;
            }
            catch(Exception)
            {
                throw new NotFoundException();
            }
        }

        public async Task<string> CreateProject(BaseUser user, Project project)
        {
            var linkedUser = await Context.Users.Include(x => x.Projects).Where(x => x.Id == user.Id).FirstAsync();
            linkedUser.Projects.Add(project);
            Context.Users.Update(linkedUser);
            await Context.SaveChangesAsync();
            return project.Id.ToString();
        }
    }
}
