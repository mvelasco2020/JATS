﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using JATS.Data;
using JATS.Models;
using JATS.Extensions;
using JATS.Models.ViewModel;
using JATS.Services.Interfaces;
using JATS.Models.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace JATS.Controllers
{
    [Authorize]
    public class ProjectsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IRolesService _rolesService;
        private readonly ILookupService _lookupService;
        private readonly IFileService _fileService;
        private readonly IProjectService _projectService;
        private readonly UserManager<JTUser> _userManager;
        private readonly ICompanyInfoService _compandyInfoService;

        public ProjectsController(ApplicationDbContext context,
                                    IRolesService rolesService,
                                    ILookupService lookupService,
                                    IFileService fileService,
                                    IProjectService projectService,
                                    UserManager<JTUser> userManager,
                                    ICompanyInfoService compandyInfoService)
        {
            _context = context;
            _rolesService = rolesService;
            _lookupService = lookupService;
            _fileService = fileService;
            _projectService = projectService;
            _userManager = userManager;
            _compandyInfoService = compandyInfoService;
        }

        // GET: Projects
        public async Task<IActionResult> Index()
        {
            List<Project> projects = new();
            int companyId = User.Identity.GetCompanyId().Value;

            //Rreturns all projects that are not archived
            projects = await _projectService.GetAllProjectsByCompany(companyId);



            return View(projects);
        }

        public async Task<IActionResult> MyProjects()
        {
            List<Project> projects = await _projectService
                .GetUserProjectsAsync(_userManager.GetUserId(User));

            return View(projects);
        }

        [Authorize(Roles = "Admin,ProjectManager")]
        public async Task<IActionResult> ArchivedProjects()
        {
            List<Project> projects = new();
            int companyId = User.Identity.GetCompanyId().Value;

            //Rreturns all projects that are not archived
            projects = await _projectService.GetAllArchivedProjectsByCompany(companyId);



            return View(projects);
        }

        // GET: Projects/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Projects == null)
            {
                return NotFound();
            }

            var project = await _context.Projects
                .Include(p => p.Company)
                .Include(p => p.ProjectPriority)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        // GET: Projects/Create
        [Authorize(Roles = "Admin,ProjectManager")]
        public async Task<IActionResult> Create()
        {
            int companyId = User.Identity.GetCompanyId().Value;

            AddProjectWithPMViewModel model = new();
            model.PMList = new SelectList(await _rolesService
                .GetUsersInRoleAsync(Roles.ProjectManager.ToString(),
                companyId)
                , "Id"
                , "FullName");

            model.PriorityList = new SelectList(await _lookupService
                .GetProjectPrioritiesAsync()
                , "Id"
                , "Name");
            return View(model);
        }

        // POST: Projects/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AddProjectWithPMViewModel model)
        {

            if (model is not null)
            {
                int companyId = User.Identity.GetCompanyId().Value;
                try
                {
                    if (model.Project.ImageFormFile is not null)
                    {
                        model.Project.ImageFileData = await _fileService
                            .ConvertFileToByteArrayAsync(model.Project.ImageFormFile);

                        model.Project.ImageFileName = model.Project.ImageFormFile.FileName;
                        model.Project.FileContentType = model.Project.ImageFormFile.ContentType;
                    }

                    model.Project.CompanyId = companyId;

                    //Entity Framework will track this change
                    //and give return the object with am Id
                    await _projectService.AddNewProjectAsync(model.Project);


                    if (!string.IsNullOrEmpty(model.PMid))
                    {
                        //project.id is availble bec of the code above
                        await _projectService.AddProjectManagerAsync(model.PMid, model.Project.Id);
                    }
                }
                catch (Exception)
                {

                    throw;
                }

                return RedirectToAction("Index");
            }

            return RedirectToAction("Create");
        }

        // GET: Projects/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {

            int companyId = User.Identity.GetCompanyId().Value;

            AddProjectWithPMViewModel model = new();
            model.Project = await _projectService.GetProjectByIdAsync(id.Value, companyId);
            if (model.Project is null)
            {
                return NotFound();
            }
            model.PMList = new SelectList(await _rolesService
                .GetUsersInRoleAsync(Roles.ProjectManager.ToString(),
                companyId)
                , "Id"
                , "FullName");

            model.PriorityList = new SelectList(await _lookupService
                .GetProjectPrioritiesAsync()
                , "Id"
                , "Name");
            return View(model);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AddProjectWithPMViewModel model)
        {
            if (model is not null)
            {
                int companyId = User.Identity.GetCompanyId().Value;
                try
                {
                    if (model.Project.ImageFormFile is not null)
                    {
                        model.Project.ImageFileData = await _fileService
                            .ConvertFileToByteArrayAsync(model.Project.ImageFormFile);

                        model.Project.ImageFileName = model.Project.ImageFormFile.FileName;
                        model.Project.FileContentType = model.Project.ImageFormFile.ContentType;
                    }

                    await _projectService.UpdateProjectAsync(model.Project);


                    if (!string.IsNullOrEmpty(model.PMid))
                    {
                        await _projectService.AddProjectManagerAsync(model.PMid, model.Project.Id);
                    }
                }
                catch (Exception)
                {

                    throw;
                }

                return RedirectToAction("Index");
            }

            return View(model);
        }

        // GET: Projects/Delete/5
        public async Task<IActionResult> Archive(int? id)
        {
            if (id == null || _context.Projects == null)
            {
                return NotFound();
            }
            var project = await _projectService.GetProjectByIdAsync(id.Value, User.Identity.GetCompanyId().Value);
            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Archive")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ArchiveConfirmed(int id)
        {

            var project = await _projectService.GetProjectByIdAsync(id, User.Identity.GetCompanyId().Value);
            await _projectService.ArchiveProjectAsync(project);

            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Restore(int? id)
        {
            if (id == null || _context.Projects == null)
            {
                return NotFound();
            }
            var project = await _projectService.GetProjectByIdAsync(id.Value, User.Identity.GetCompanyId().Value);
            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Restore")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RestoreConfirmed(int id)
        {

            var project = await _projectService.GetProjectByIdAsync(id, User.Identity.GetCompanyId().Value);
            await _projectService.RestoreProjectAsync(project);

            return RedirectToAction(nameof(Index));
        }
        private bool ProjectExists(int id)
        {
            return (_context.Projects?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
