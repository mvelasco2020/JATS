using JATS.Data;
using JATS.Models;
using JATS.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace JATS.Services
{
    public class ProjectService : IProjectService
    {

        private readonly ApplicationDbContext _context;

        public ProjectService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task AddNewProjectAsync(Project project)
        {
            _context.Add(project);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> AddProjectManagerAsync(string userId, int projectId)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> AddUserToProjectAsync(string userId, int projectId)
        {
            JTUser user = await _context
                .Users
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user is null)
            {
                return false;
            }

            Project project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == projectId);
            if (!await IsUserOnProjectAsync(userId, projectId))
            {
                try
                {
                    project.Members.Add(user);
                    await _context.SaveChangesAsync();
                    return true;
                }
                catch (Exception)
                {

                    throw;
                }
            }

            return false;
        }

        public async Task ArchiveProjectAsync(Project project)
        {
            project.Archived = true;
            _context.Update(project);
            await _context.SaveChangesAsync();

        }

        public async Task<List<JTUser>> GetAllProjectMembersExceptPMAsync(int projectId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Project>> GetAllProjectsByCompany(int companyId)
        {
            List<Project> projects = await _context
                .Projects
                .Where(p => p.CompanyId == companyId && p.Archived == false)
                .Include(p => p.Members)
                .Include(p => p.Tickets)
                    .ThenInclude(t => t.Comments)
                .Include(p => p.Tickets)
                    .ThenInclude(t => t.Attachments)
                .Include(p => p.Tickets)
                    .ThenInclude(t => t.History)
                .Include(p => p.Tickets)
                    .ThenInclude(t => t.TechnicianUser)
                .Include(p => p.Tickets)
                    .ThenInclude(t => t.OwnerUser)
                .Include(p => p.Tickets)
                    .ThenInclude(t => t.Notifications)
                .Include(p => p.Tickets)
                    .ThenInclude(t => t.TicketStatus)
                .Include(p => p.Tickets)
                    .ThenInclude(t => t.TicketPriority)
                .Include(p => p.Tickets)
                    .ThenInclude(t => t.TicketType)
                .Include(p => p.ProjectPriority)
                .ToListAsync();
            return projects;
        }

        public async Task<List<Project>> GetAllProjectsByPriority(int companyId, string priorityName)
        {
            List<Project> projects = await GetAllProjectsByCompany(companyId);
            int priorityId = await LookupProjectPriorityIdAsync(priorityName);

            return projects.Where(p => p.ProjectPriorityId == priorityId).ToList();
        }

        public async Task<List<Project>> GetArchivedProjectsByCompany(int companyId)
        {

            List<Project> projects = await GetAllProjectsByCompany(companyId);
            return projects.Where(p => p.Archived == true).ToList();
        }

        public async Task<List<JTUser>> GetDevelopersOnProjectAsync(int projectId)
        {
            throw new NotImplementedException();
        }

        public async Task<Project> GetProjectByIdAsync(int projectId, int companyId)
        {
            Project project = await _context
                .Projects
                .Include(p => p.Tickets)
                .Include(p => p.Members)
                .Include(p => p.ProjectPriority)
                .FirstOrDefaultAsync(p => p.CompanyId == companyId && p.Id == projectId);

            return project;

        }

        public async Task<JTUser> GetProjectManagerAsync(int projectId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<JTUser>> GetProjectMembersByRoleAsync(int projectId, string role)
        {
            throw new NotImplementedException();
        }

        public async Task<List<JTUser>> GetSubmittersOnProjectAsync(int projectId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Project>> GetUserProjectsAsync(string userId)
        {
            try
            {
                List<Project> userProjects = (await _context
                    .Users
                    .Include(u => u.Projects)
                        .ThenInclude(u => u.Company)
                    .Include(u => u.Projects)
                        .ThenInclude(u => u.Members)
                    .Include(u => u.Projects)
                        .ThenInclude(u => u.Tickets)
                    .Include(u => u.Projects)
                        .ThenInclude(u => u.Tickets)
                        .ThenInclude(u => u.TechnicianUser)
                    .Include(u => u.Projects)
                        .ThenInclude(u => u.Tickets)
                        .ThenInclude(u => u.OwnerUser)
                    .Include(u => u.Projects)
                        .ThenInclude(u => u.Tickets)
                        .ThenInclude(u => u.TicketPriority)
                    .Include(u => u.Projects)
                        .ThenInclude(u => u.Tickets)
                        .ThenInclude(u => u.TicketStatus)
                    .FirstOrDefaultAsync(u => u.Id == userId))
                    .Projects
                    .ToList();

                return userProjects;
            }
            catch (Exception)
            {

                throw;
            }

            return null;
        }

        public async Task<List<JTUser>> GetUsersNotOnProjectAsync(int projectId, int companyId)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> IsUserOnProjectAsync(string userId, int projectId)
        {
            Project project = await _context
                .Projects
                .Include(p => p.Members)
                .FirstOrDefaultAsync(p => p.Id == projectId);

            bool result = false;
            if (project is null)
            {
                return result;
            }

            result = project.Members.Any(m => m.Id == userId);
            return result;
        }

        public async Task<int> LookupProjectPriorityIdAsync(string priorityName)
        {
            int priorityId = (await _context
                .ProjectPriorities
                .FirstOrDefaultAsync(p => p.Name == priorityName))
                .Id;

            return priorityId;
        }

        public async Task RemoveProjectManagerAsync(int projectId)
        {


        }

        public async Task RemoveUserFromProjectAsync(string userId, int projectId)
        {
            try
            {
                JTUser user = await _context
                    .Users
                    .FirstOrDefaultAsync(u => u.Id == userId);

                Project project = await _context
                    .Projects
                    .FirstOrDefaultAsync(p => p.Id == projectId);

                try
                {
                    if (await IsUserOnProjectAsync(userId, projectId))
                    {
                        project.Members.Remove(user);
                        await _context.SaveChangesAsync();
                    }

                }
                catch (Exception)
                {

                    throw;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"### ERROR ### {ex.Message}");
            }


        }

        public async Task RemoveUsersFromProjectByRoleAsync(string role, int projectId)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateProjectAsync(Project project)
        {
            _context.Update(project);
            await _context.SaveChangesAsync();
        }
    }
}
