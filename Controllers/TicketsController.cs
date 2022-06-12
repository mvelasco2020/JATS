using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using JATS.Data;
using JATS.Models;
using Microsoft.AspNetCore.Identity;
using JATS.Extensions;
using JATS.Services.Interfaces;
using JATS.Models.Enums;
using System.Collections;
using Microsoft.AspNetCore.Authorization;

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

        public TicketsController(ApplicationDbContext context,
                                UserManager<JTUser> userManager,
                                IProjectService projectService,
                                ITicketService ticketService,
                                ILookupService lookupService)
        {
            _context = context;
            _userManager = userManager;
            _projectService = projectService;
            _ticketService = ticketService;
            _lookupService = lookupService;
        }

        // GET: Tickets
        public async Task<IActionResult> Index()
        {
            List<Ticket> tickets = await _ticketService.GetAllTicketsByCompanyAsync(User.Identity.GetCompanyId().Value);

            // var tickets = await _context.Tickets.ToListAsync();
            return View(tickets.Where(t => t.Archived == false));
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

            return View(ticket);
        }

        // GET: Tickets/Create
        public async Task<IActionResult> Create()
        {
            int companyId = User.Identity.GetCompanyId().Value;
            JTUser user = await _userManager.GetUserAsync(User);

            if (User.IsInRole(Roles.Admin.ToString()))
            {
                ViewData["ProjectId"] = new SelectList((await _projectService.GetAllProjectsByCompany(companyId)), "Id", "Name");

            }
            else
            {
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
                await _ticketService.AddNewTicketAsync(ticket);
                //Todo : ticket history
                //Todo : ticket notification
                return RedirectToAction(nameof(Index));
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
            if (id == null || _context.Tickets == null)
            {
                return NotFound();
            }

            Ticket ticket = await _ticketService.GetTicketByIdAsync(id.Value);

            if (ticket == null)
            {
                return NotFound();
            }
            ViewData["TicketPriorityId"] = new SelectList((await _lookupService.GetTicketPrioritiesAsync()), "Id", "Name", ticket.TicketPriorityId);
            ViewData["TicketStatusId"] = new SelectList((await _lookupService.GetTicketStatusesAsync()), "Id", "Name", ticket.TicketStatusId);
            ViewData["TicketTypeId"] = new SelectList((await _lookupService.GetTicketTypesAsync()), "Id", "Name", ticket.TicketTypeId);
            return View(ticket);
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
                //Todo add history

                return RedirectToAction(nameof(Index));
            }
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
            if (ticket is null) return NotFound();
            await _ticketService.ArchiveTicketAsync(ticket);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> ArchiveTicket(int? id)
        {


            if ((!await TicketExists(id.Value)) || id is null)

                return NotFound();


            return View(await _ticketService.GetTicketByIdAsync(id.Value));
        }


        [HttpGet]
        public async Task<IActionResult> RestoreTicket(int? id)
        {
            var ticket = await _ticketService.GetTicketByIdAsync(id.Value);
            if (ticket is null || id is null)
                return NotFound();
            return View(ticket);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RestoreTicket(int id)
        {
            var ticket = await _ticketService.GetTicketByIdAsync(id);
            if (ticket is null)
                return NotFound();
            ticket.Archived = false;
            await _ticketService.UpdateTicketAsync(ticket);
            return RedirectToAction("Index");
        }




        private async Task<bool> TicketExists(int id)
        {
            int companyId = User.Identity.GetCompanyId().Value;

            return (await _ticketService.GetAllTicketsByCompanyAsync(companyId)).Any(t => t.Id == id);
        }
    }
}
