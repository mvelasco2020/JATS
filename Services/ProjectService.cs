using JATS.Data;
using JATS.Models;
using JATS.Models.Enums;
using JATS.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace JATS.Services
{
    public class ProjectService : IProjectService
    {

        private readonly ApplicationDbContext _context;
        private readonly IRolesService _roleService;

        public ProjectService(ApplicationDbContext context,
                              IRolesService roleService)
        {
            _context = context;
            _roleService = roleService;
        }
        public async Task AddNewProjectAsync(Project project)
        {
            _context.Add(project);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> AddProjectManagerAsync(string userId, int projectId)
        {

            JTUser currentPM = await GetProjectManagerAsync(projectId);
            Project project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == projectId);
            if (currentPM is not null)
            {
                try
                {
                    await RemoveProjectManagerAsync(projectId);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{ex.Message}");

                    return false;
                }
            }

            try
            {
                bool result = await AddUserToProjectAsync(userId, projectId);
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}");
                return false;
            }

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

            try
            {
                project.Archived = true;
                await UpdateProjectAsync(project);
                foreach (var ticket in project.Tickets)
                {
                    ticket.ArchivedByProject = true;
                    _context.Update(ticket);
                    await _context.SaveChangesAsync();
                }


            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<List<JTUser>> GetAllProjectMembersExceptPMAsync(int projectId)
        {
            List<JTUser> technicians = await GetProjectMembersByRoleAsync(projectId,
                Roles.Technician.ToString());

            List<JTUser> submitters = await GetProjectMembersByRoleAsync(projectId,
                Roles.Submitter.ToString());

            List<JTUser> admins = await GetProjectMembersByRoleAsync(projectId,
                Roles.Admin.ToString());

            List<JTUser> teamMembers = technicians.Concat(submitters).Concat(admins).ToList();
            return teamMembers;

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

        public async Task<List<Project>> GetAllArchivedProjectsByCompany(int companyId)
        {
            List<Project> projects = await _context
                .Projects
                .Where(p => p.CompanyId == companyId && p.Archived == true)
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


        public async Task<List<JTUser>> GetTechniciansOnProjectAsync(int projectId)
        {
            throw new NotImplementedException();
        }

        public async Task<Project> GetProjectByIdAsync(int projectId, int companyId)
        {
            Project project = await _context
                .Projects
                .Include(p => p.Tickets)
                .ThenInclude(t => t.TicketPriority)
                .Include(p => p.Tickets)
                .ThenInclude(t => t.TicketType)
                .Include(p => p.Tickets)
                .ThenInclude(t => t.TicketStatus)
                .Include(p => p.Tickets)
                .ThenInclude(t => t.TechnicianUser)
                .Include(p => p.Tickets)
                .ThenInclude(t => t.OwnerUser)
                .Include(p => p.Members)
                .Include(p => p.ProjectPriority)
                .FirstOrDefaultAsync(p => p.CompanyId == companyId && p.Id == projectId);

            return project;

        }

        public async Task<JTUser> GetProjectManagerAsync(int projectId)
        {
            Project project = await _context
                .Projects
                .Include(p => p.Members)
                .FirstOrDefaultAsync(p => p.Id == projectId);

            foreach (var member in project?.Members)
            {
                if (await _roleService.IsUserInRoleAsync(member, Roles.ProjectManager.ToString()))
                {
                    return member;
                }
            }

            return null;
        }

        public async Task<List<JTUser>> GetProjectMembersByRoleAsync(int projectId, string role)
        {
            Project project = await _context
                .Projects
                .Include(p => p.Members)
                .FirstOrDefaultAsync(p => p.Id == projectId);

            List<JTUser> membersInRole = new();

            foreach (var member in project.Members)
            {
                if (await _roleService.IsUserInRoleAsync(member, role))
                {
                    membersInRole.Add(member);
                }
            }

            return membersInRole;

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
            List<JTUser> usersinCompany = await _context
                .Users
                .Where(u => u.Projects.All(p => p.Id != projectId))
                .Where(u => u.CompanyId == companyId)
                .ToListAsync();

            return usersinCompany;
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

            Project project = await _context
                .Projects
                .Include(p => p.Members)
                .FirstOrDefaultAsync(p => p.Id == projectId);


            try
            {
                foreach (var member in project?.Members)
                {
                    if (await _roleService.IsUserInRoleAsync(member, Roles.ProjectManager.ToString()))
                    {
                        await RemoveUserFromProjectAsync(member.Id, projectId);

                    }
                }
            }
            catch (Exception)
            {

                throw;
            }

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
            try
            {
                List<JTUser> members = await GetProjectMembersByRoleAsync(projectId, role);
                Project project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == projectId);

                foreach (var user in members)
                {
                    try
                    {
                        project.Members.Remove(user);
                        await _context.SaveChangesAsync();
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task UpdateProjectAsync(Project project)
        {
            _context.Update(project);
            await _context.SaveChangesAsync();
        }

        public async Task RestoreProjectAsync(Project project)
        {
            try
            {
                project.Archived = false;
                await UpdateProjectAsync(project);
                foreach (var ticket in project.Tickets)
                {
                    ticket.ArchivedByProject = false;
                    _context.Update(ticket);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<bool> IsAssignedProjectManager(string userId, int projectId)
        {
            try
            {
                string projectManagerId = (await GetProjectManagerAsync(projectId))?.Id;
                if (userId == projectManagerId)
                {
                    return true;
                }
                else return false;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
