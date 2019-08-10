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
            return await Task.FromResult(Context.Projects.Where(x => x.BaseUserId == user.Id).AsEnumerable());
        }

        public async Task<Project> GetProjectById(BaseUser user, string projectId)
        {
            var project = await Context.Projects.FirstOrDefaultAsync(x => x.Id == projectId);
            if (project == null) throw new NotFoundException("Could not find project");

            if (project.BaseUserId != user.Id) throw new NotFoundException("Project is not associated with user");
            return project;
        }

        public async Task<string> CreateProject(BaseUser user, Project project)
        {
            project.BaseUserId = user.Id;
            var entity = await Context.Projects.AddAsync(project);
            await Context.SaveChangesAsync();
            return entity.Entity.Id;
        }
    }
}
