using API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Services
{
    public interface IProjectService
    {
        IEnumerable<Project> GetAllProjects(BaseUser user);
        Project GetProjectById(BaseUser user, string projectId);
        string CreateProject(BaseUser user, Project project);
    }

    public class ProjectService : IProjectService
    {
        protected Context Context { get; }

        public ProjectService(Context context)
        {
            Context = context;
        }

        public IEnumerable<Project> GetAllProjects(BaseUser user)
        {
            if (user == null) return new List<Project>();
            return Context.Projects.AsEnumerable();
        }

        public Project GetProjectById(BaseUser user, string projectId)
        {
            if (user == null) return null;
            try
            {
                var projects = user.Projects;
                var id = int.Parse(projectId);
                return projects.Find(x => x.Id == id);
            }
            catch(Exception ex)
            {
                return null;
            }
        }

        public string CreateProject(BaseUser user, Project project)
        {
            if (user.Projects == null)
            {
                user.Projects = new List<Project>();
            }
            Context.Users.Update(user);
            user.Projects.Add(project);
            Context.SaveChanges();
            return project.Id.ToString();
        }
    }
}
