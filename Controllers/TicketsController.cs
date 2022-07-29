using JATS.Data;
using JATS.Extensions;
using JATS.Models;
using JATS.Models.Enums;
using JATS.Models.ViewModel;
using JATS.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace JATS.Controllers
{
    [Authorize]
    public class TicketsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<JTUser> _userManager;
        private readonly IProjectService _projectService;
        private readonly ITicketService _ticketService;
        private readonly ILookupService _lookupService;
        private readonly IFileService _fileService;
        private readonly ITicketHistoryService _historyService;

        public TicketsController(ApplicationDbContext context,
                                UserManager<JTUser> userManager,
                                IProjectService projectService,
                                ITicketService ticketService,
                                ILookupService lookupService,
                                IFileService fileService,
                                ITicketHistoryService historyService)
        {
            _context = context;
            _userManager = userManager;
            _projectService = projectService;
            _ticketService = ticketService;
            _lookupService = lookupService;
            _fileService = fileService;
            _historyService = historyService;
        }

        // GET: Tickets
        public async Task<IActionResult> Index()
        {
            List<Ticket> tickets = await _ticketService
                .GetAllTicketsByCompanyAsync(User.Identity.GetCompanyId().Value);

            return View(tickets.Where(t => t.Archived != true && t.ArchivedByProject != true));
        }


        public async Task<IActionResult> MyTickets()
        {
            JTUser user = await _userManager.GetUserAsync(User);

            List<Ticket> tickets = await _ticketService.GetTicketsByUserIdAsync(user.Id, User.Identity.GetCompanyId().Value);
            return View(tickets);
        }
        // GET: Tickets/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var ticket = await _ticketService.GetTicketByIdAsync(id.Value);


            if (ticket == null)
            {
                return NotFound();
            }


            ViewData["TicketTechnician"] = new SelectList(await _projectService.GetProjectMembersByRoleAsync(ticket.ProjectId, nameof(Roles.Technician)), "Id", "FullName");
            ViewData["TicketPriorityId"] = new SelectList((await _lookupService.GetTicketPrioritiesAsync()), "Id", "Name", ticket.TicketPriorityId);
            ViewData["TicketStatusId"] = new SelectList((await _lookupService.GetTicketStatusesAsync()), "Id", "Name", ticket.TicketStatusId);
            ViewData["TicketTypeId"] = new SelectList((await _lookupService.GetTicketTypesAsync()), "Id", "Name", ticket.TicketTypeId);

            ticket.Comments = ticket.Comments?.OrderByDescending(c => c.Created).ToList();
            ticket.History = ticket.History?.OrderByDescending(h => h.Created).ToList();
            return View(ticket);
        }

        // GET: Tickets/Create
        public async Task<IActionResult> Create(int? projId)
        {
            int companyId = User.Identity.GetCompanyId().Value;
            JTUser user = await _userManager.GetUserAsync(User);

            if (User.IsInRole(Roles.Admin.ToString()))
            {
                ViewData["ProjectId"] = new SelectList((await _projectService.GetAllProjectsByCompany(companyId)), "Id", "Name");
            }
            else
            {
                if (projId is not null)
                {
                    Project project = await _projectService.GetProjectByIdAsync(projId.Value, companyId);
                    ViewData["ProjectId"] = new SelectList((await _projectService.GetUserProjectsAsync(user.Id)), "Id", "Name", new { Id = project.Id, Name = project.Name });
                }
                else
                    ViewData["ProjectId"] = new SelectList((await _projectService.GetUserProjectsAsync(user.Id)), "Id", "Name");
            }

            ViewData["TicketPriorityId"] = new SelectList((await _lookupService.GetTicketPrioritiesAsync()), "Id", "Name");
            ViewData["TicketTypeId"] = new SelectList((await _lookupService.GetTicketTypesAsync()), "Id", "Name");
            return View();
        }



        // POST: Tickets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,ProjectId,TicketTypeId,TicketPriorityId,OwnerUserId")] Ticket ticket)
        {
            JTUser user = await _userManager.GetUserAsync(User);
            ticket.Created = DateTimeOffset.Now;
            ticket.OwnerUserId = user.Id;
            ticket.TicketStatusId = (await _ticketService.LookupTicketStatusIdAsync(nameof(TicketStatusEnum.New))).Value;
            if (ModelState.IsValid)
            {
                try
                {
                    await _ticketService.AddNewTicketAsync(ticket);
                    //Todo : ticket history
                    Ticket newTicket = await _ticketService.GetTicketAsNoTrackingAsync(ticket.Id);
                    await _historyService.AddHistoryAsync(null, newTicket, user.Id);

                    return RedirectToAction("Details", new { id = newTicket.Id });
                }
                catch (Exception)
                {

                    throw;
                }
                //Todo : ticket notification
            }
            if (User.IsInRole(Roles.Admin.ToString()))
            {
                ViewData["ProjectId"] = new SelectList((await _projectService.GetAllProjectsByCompany(user.CompanyId)), "Id", "Name");

            }
            else
            {
                ViewData["ProjectId"] = new SelectList((await _projectService.GetUserProjectsAsync(user.Id)), "Id", "Name");
            }
            ViewData["TicketPriorityId"] = new SelectList((await _lookupService.GetTicketPrioritiesAsync()), "Id", "Name");
            ViewData["TicketTypeId"] = new SelectList((await _lookupService.GetTicketTypesAsync()), "Id", "Name");
            return View(ticket);
        }

        // GET: Tickets/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {


            if (id == null)
            {
                return NotFound();
            }

            Ticket ticket = await _ticketService.GetTicketByIdAsync(id.Value);
            JTUser user = _userManager.GetUserAsync(User).Result;


            if (await IsUserAuthorizedToMakeChanges(user, ticket))
            {

                if (ticket == null)
                {
                    return NotFound();
                }
                ViewData["TicketPriorityId"] = new SelectList((await _lookupService.GetTicketPrioritiesAsync()), "Id", "Name", ticket.TicketPriorityId);
                ViewData["TicketStatusId"] = new SelectList((await _lookupService.GetTicketStatusesAsync()), "Id", "Name", ticket.TicketStatusId);
                ViewData["TicketTypeId"] = new SelectList((await _lookupService.GetTicketTypesAsync()), "Id", "Name", ticket.TicketTypeId);
                return View(ticket);
            }
            else
            {
                return Unauthorized();
            }

        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,Created,Updated,Archived,ProjectId,TicketTypeId,TicketPriorityId,TicketStatusId,OwnerUserId,TechnicianUserId")] Ticket ticket)
        {


            if (id != ticket.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                JTUser user = await _userManager.GetUserAsync(User);
                Ticket oldTicket = await _ticketService.GetTicketAsNoTrackingAsync(ticket.Id);

                if (await IsUserAuthorizedToMakeChanges(user, oldTicket))
                {
                    try
                    {
                        ticket.Updated = DateTimeOffset.Now;
                        await _ticketService.UpdateTicketAsync(ticket);
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!await TicketExists(ticket.Id))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }

                    Ticket newTicket = await _ticketService.GetTicketAsNoTrackingAsync(ticket.Id);
                    await _historyService.AddHistoryAsync(oldTicket, newTicket, user.Id);

                    return RedirectToAction(nameof(Details), new { id = id });
                }
                else
                {
                    return Unauthorized();
                }
            }
            ViewData["TicketTechnician"] = new SelectList(await _projectService.GetProjectMembersByRoleAsync(ticket.ProjectId, nameof(Roles.Technician)), "Id", "FullName");
            ViewData["TicketPriorityId"] = new SelectList((await _lookupService.GetTicketPrioritiesAsync()), "Id", "Name", ticket.TicketPriorityId);
            ViewData["TicketStatusId"] = new SelectList((await _lookupService.GetTicketStatusesAsync()), "Id", "Name", ticket.TicketStatusId);
            ViewData["TicketTypeId"] = new SelectList((await _lookupService.GetTicketTypesAsync()), "Id", "Name", ticket.TicketTypeId);
            return View(ticket);
        }


        public async Task<IActionResult> ArchivedTickets()
        {

            var archivedTickets = await _ticketService.GetArchivedTicketsAsync(User.Identity.GetCompanyId().Value);
            return View(archivedTickets);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ArchiveTicket(int id)
        {


            var ticket = await _ticketService.GetTicketByIdAsync(id);
            JTUser user = await _userManager.GetUserAsync(User);

            if (ticket is null) return NotFound();
            if (await IsUserAuthorizedToMakeChanges(user, ticket))
            {

                await _ticketService.ArchiveTicketAsync(ticket);
                return RedirectToAction("Details", new { id = ticket.ProjectId });
            }
            else
            {
                return Unauthorized();
            }
        }

        /*To be deleted 
        [HttpGet]
        public async Task<IActionResult> ArchiveTicket(int? id)
        {


            if ((!await TicketExists(id.Value)) || id is null)

                return NotFound();

            JTUser user = await _userManager.GetUserAsync(User);
            var ticket = await _ticketService.GetTicketByIdAsync(id.Value);
            if (await IsUserAuthorizedToMakeChanges(user, ticket))
            {
                return View(ticket);
            }
            else
            {
                return Unauthorized();
            }
        }
         */


        [Authorize(Roles = "Admin,ProjectManager")]
        public async Task<IActionResult> UnassignedTickets()
        {

            var unAssigneddTickets = await _ticketService
                .GetUnassignedTicketsAsync(User.Identity.GetCompanyId().Value);

            if (User.IsInRole(nameof(Roles.Admin)))
            {
                return View(unAssigneddTickets.Where(t => t.Archived != true && t.ArchivedByProject != true));
            }
            else
            {
                List<Ticket> pmTickets = new();
                foreach (var item in unAssigneddTickets)
                {
                    if (await _projectService.IsAssignedProjectManager(_userManager.GetUserId(User), item.ProjectId))
                        pmTickets.Add(item);
                }
                return View(pmTickets.Where(t => t.Archived != true && t.ArchivedByProject != true));
            }

        }

        [HttpGet]
        public async Task<IActionResult> AssignTechnician(int ticketId)
        {
            AssignTechnicianViewModel model = new();
            model.Ticket = await _ticketService.GetTicketByIdAsync(ticketId);

            if (model.Ticket.Project.isPrimordial == true)
            {
                List<JTUser> users = _context
                    .Users.Where(u => u.CompanyId == User.Identity.GetCompanyId().Value).ToList();
                model.Technicians = new SelectList(users, "Id", "FullName");

            }
            else
            {

                JTUser user = await _userManager.GetUserAsync(User);
                if (await IsUserAuthorizedToMakeChanges(user, model.Ticket))
                {
                    model.Technicians = new SelectList(await _projectService.GetProjectMembersByRoleAsync(model.Ticket.ProjectId, nameof(Roles.Technician)), "Id", "FullName");
                }
                else
                {
                    return Unauthorized();
                }
            }



            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignTechnician(AssignTechnicianViewModel viewModel)
        {
            JTUser user = await _userManager.GetUserAsync(User);
            Ticket oldTicket = await _ticketService.GetTicketAsNoTrackingAsync(viewModel.Ticket.Id);
            if (!await IsUserAuthorizedToMakeChanges(user, oldTicket))
            {
                return Unauthorized();
            }

            if (viewModel.TechnicianId != null)
            {
                try
                {
                    await _ticketService.AssignTicketAsync(viewModel.Ticket.Id, viewModel.TechnicianId);
                }
                catch (Exception)
                {

                    throw;
                }

                Ticket newTicket = await _ticketService.GetTicketAsNoTrackingAsync(viewModel.Ticket.Id);
                await _historyService.AddHistoryAsync(oldTicket, newTicket, user.Id);

                return RedirectToAction(nameof(Details), new { id = viewModel.Ticket.Id });
            }


            return RedirectToAction(nameof(AssignTechnician), new { id = viewModel.Ticket.Id });
        }

        /* to be deleted
        [HttpGet]
        public async Task<IActionResult> RestoreTicket(int? id)
        {
            var ticket = await _ticketService.GetTicketByIdAsync(id.Value);
            if (ticket is null || id is null)
                return NotFound();
            return View(ticket);
        }
         */



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RestoreTicket(int id)
        {
            var ticket = await _ticketService.GetTicketByIdAsync(id);
            if (ticket is null)
                return NotFound();
            ticket.Archived = false;
            await _ticketService.UpdateTicketAsync(ticket);
            return RedirectToAction("Details", new { id = ticket.Id });
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddTicketComment([Bind("Id,TicketId,Comment,UserId,Created")] TicketComment ticketComment)
        {
            JTUser user = await _userManager.GetUserAsync(User);
            Ticket ticket = await _ticketService.GetTicketByIdAsync(ticketComment.TicketId);

            if (!await IsUserAuthorizedToMakeChanges(user, ticket))
            {
                return Unauthorized();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    ticketComment.UserId = user.Id;
                    ticketComment.Created = DateTimeOffset.Now;
                    await _ticketService.AddTicketCommentAsync(ticketComment);

                    await _historyService.AddHistoryAsync(ticketComment.TicketId, nameof(TicketComment), ticketComment.UserId);
                }
                catch (Exception)
                {

                    throw;
                }
            }

            return RedirectToAction("Details", new { id = ticketComment.TicketId });
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddTicketAttachment([Bind("Id,FormFile,Description,TicketId")] TicketAttachment ticketAttachment)
        {
            JTUser user = await _userManager.GetUserAsync(User);
            Ticket ticket = await _ticketService.GetTicketByIdAsync(ticketAttachment.TicketId);
            if (!await IsUserAuthorizedToMakeChanges(user, ticket))
            {
                return Unauthorized();
            }
            string statusMessage;

            if (ModelState.IsValid && ticketAttachment.FormFile != null)
            {
                ticketAttachment.Data = await _fileService.ConvertFileToByteArrayAsync(ticketAttachment.FormFile);
                ticketAttachment.FileName = ticketAttachment.FormFile.FileName;
                ticketAttachment.FileContentType = ticketAttachment.FormFile.ContentType;

                ticketAttachment.Created = DateTimeOffset.Now;
                ticketAttachment.UserId = user.Id;

                await _ticketService.AddTicketAttachmentAsync(ticketAttachment);
                await _historyService.AddHistoryAsync(ticketAttachment.TicketId, nameof(TicketAttachment), ticketAttachment.UserId);
                statusMessage = "Success: New attachment added to Ticket.";
            }
            else
            {
                statusMessage = "Error: Invalid data.";

            }

            return RedirectToAction("Details", new { id = ticketAttachment.TicketId, message = statusMessage });

        }

        public async Task<IActionResult> ShowFile(int id)
        {
            TicketAttachment ticketAttachment = await _ticketService.GetTicketAttachmentByIdAsync(id);
            string fileName = ticketAttachment.FileName;
            byte[] fileData = ticketAttachment.Data;
            string ext = Path.GetExtension(fileName).Replace(".", "");

            Response.Headers.Add("Content-Disposition", $"inline; filename={fileName}");
            return File(fileData, $"application/{ext}");
        }

        //local private methods//
        private async Task<bool> TicketExists(int id)
        {
            int companyId = User.Identity.GetCompanyId().Value;

            return (await _ticketService.GetAllTicketsByCompanyAsync(companyId)).Any(t => t.Id == id);
        }

        private async Task<bool> IsUserAuthorizedToMakeChanges(JTUser user, Ticket ticket)
        {
            var isPm = await _projectService.IsAssignedProjectManager(user.Id, ticket.ProjectId);
            if (User.IsInRole(nameof(Roles.Admin)) ||
                ticket.TechnicianUserId == user.Id ||
                ticket.OwnerUserId == user.Id ||
                ticket.Project.Members.Contains(user) ||
                isPm == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }



    }
}
