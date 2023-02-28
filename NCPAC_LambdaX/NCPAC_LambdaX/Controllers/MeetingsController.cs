using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NCPAC_LambdaX.Data;
using NCPAC_LambdaX.Models;
using NCPAC_LambdaX.Utilities;
using NCPAC_LambdaX.ViewModels;

namespace NCPAC_LambdaX.Controllers
{
    [Authorize]
    public class MeetingsController : Controller
    {
        private readonly NCPACContext _context;

        public MeetingsController(NCPACContext context)
        {
            _context = context;
        }

        // GET: Meetings
        public async Task<IActionResult> Index(string SearchString, bool ShowCompleted, bool ShowCanceled, int? CommiteeID,
            int? page, int? pageSizeID, string actionButton, string sortDirection = "asc", string sortField = "Meeting")
        {
            ViewData["Filtering"] = "";
            PopulateDropDownLists();

            string[] sortOptions = new[] { "Meeting", "TimeFrom" };

            var meetings = _context.Meetings
            .Include(m => m.Commitee)
            .ThenInclude(m => m.MemberCommitees)
            .AsNoTracking();

            var member = await _context.Members
                .Include(m => m.MemberCommitees)
                .Where(e => e.Email == User.Identity.Name || e.WorkEmail == User.Identity.Name)
                .AsNoTracking()
                .FirstOrDefaultAsync();
            

            if((User.IsInRole("Admin") || User.IsInRole("Supervisor") || User.IsInRole("Staff")) == false)
            {
                meetings = meetings.Where(m => (m.Commitee.MemberCommitees.Any(c => c.MemberID == member.ID)) || (m.CommiteeID == null));
            }

            if (!String.IsNullOrEmpty(SearchString))
            {
                meetings = meetings.Where(p => p.MeetingTitle.ToUpper().Contains(SearchString.ToUpper()));
            }
            if (CommiteeID.HasValue)
            {
                meetings = meetings.Where(p => p.CommiteeID == CommiteeID);
                ViewData["Filtering"] = "";
            }
            if (ShowCompleted == true)
            {
                meetings = meetings.Where(p => p.IsArchived == true);
            }
            else
            {
                meetings = meetings.Where(p => p.IsArchived == false);
            }

            if (ShowCanceled == true)
            {
                meetings = meetings.Where(p => p.IsCancelled == true);
            }
            else
            {
                meetings = meetings.Where(p => p.IsCancelled == false);
            }
            if (!String.IsNullOrEmpty(actionButton)) //Form Submitted!
            {
                page = 1;//Reset page start

                if (sortOptions.Contains(actionButton))//Change of sort is requested
                {
                    if (actionButton == sortField) //Reverse order on same field
                    {
                        sortDirection = sortDirection == "asc" ? "desc" : "asc";
                    }
                    sortField = actionButton;//Sort by the button clicked
                }
            }

            if (sortField == "Meeting")
            {
                if (sortDirection == "asc")
                {
                    meetings = meetings
                        .OrderBy(p => p.MeetingTitle);
                }
                else
                {
                    meetings = meetings
                       .OrderByDescending(p => p.MeetingTitle);
                }
            }

            if (sortField == "TimeFrom")
            {
                if (sortDirection == "asc")
                {
                    meetings = meetings
                        .OrderByDescending(p => p.TimeFrom);
                }
                else
                {
                    meetings = meetings
                       .OrderBy(p => p.TimeFrom);
                }
            }


            ViewData["sortField"] = sortField;
            ViewData["sortDirection"] = sortDirection;

            int pageSize = PageSizeHelper.SetPageSize(HttpContext, pageSizeID, "Meeting");
            ViewData["pageSizeID"] = PageSizeHelper.PageSizeList(pageSize);

            var pagedData = await PaginatedList<Meeting>.CreateAsync(meetings.AsNoTracking(), page ?? 1, pageSize);

            return View(pagedData);
        }

        // GET: Meetings/Details/5

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Meetings == null)
            {
                return NotFound();
            }

            var meeting = await _context.Meetings
            .Include(m => m.Commitee)
            .ThenInclude(m => m.MemberCommitees)
            .FirstOrDefaultAsync(m => m.ID == id);
            if (meeting == null)
            {
                return NotFound();
            }

            return View(meeting);
        }

        // GET: Meetings/Create
        [Authorize(Roles = "Admin,Supervisor,Staff")]
        public IActionResult Create()
        {
            PopulateDropDownLists();
            return View();
        }

        // POST: Meetings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin,Supervisor,Staff")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,MeetingTitle,Description,MeetingLink,TimeFrom,TimeTo,IsArchived,CommiteeID,IsCancelled")] Meeting meeting)
        {

            if (ModelState.IsValid)
            {

                _context.Add(meeting);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            PopulateDropDownLists(meeting);
            return View(meeting);
        }

        // GET: Meetings/Edit/5
        [Authorize(Roles = "Admin,Supervisor,Staff")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Meetings == null)
            {
                return NotFound();
            }

            var meeting = await _context.Meetings.FindAsync(id);
            if (meeting == null)
            {
                return NotFound();
            }
            PopulateDropDownLists(meeting);
            return View(meeting);
        }

        // POST: Meetings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Supervisor,Staff")]
        public async Task<IActionResult> Edit(int id, [Bind("ID,MeetingTitle,Description,MeetingLink,TimeFrom,TimeTo,IsArchived,CommiteeID,IsCancelled")] Meeting meeting)
        {
            if (id != meeting.ID)
            {
                return NotFound();
            }



            if (ModelState.IsValid)
            {
                try
                {

                    _context.Update(meeting);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MeetingExists(meeting.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            PopulateDropDownLists(meeting);
            return View(meeting);
        }

        // GET: Meetings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Meetings == null)
            {
                return NotFound();
            }

            var meeting = await _context.Meetings
                .FirstOrDefaultAsync(m => m.ID == id);
            if (meeting == null)
            {
                return NotFound();
            }

            return View(meeting);
        }

        // POST: Meetings/Delete/5
        [Authorize(Roles = "Admin,Supervisor,Staff")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Meetings == null)
            {
                return Problem("Entity set 'NCPACContext.Meetings'  is null.");
            }
            var meeting = await _context.Meetings.FindAsync(id);
            if (meeting != null)
            {
                _context.Meetings.Remove(meeting);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private SelectList CommiteeList(int? selectedId)
        {
            return new SelectList(_context
                .Commitees
                .OrderBy(m => m.CommiteeName), "ID", "CommiteeName", selectedId);
        }

        private void PopulateDropDownLists(Meeting meeting = null)
        {
            ViewData["CommiteeID"] = CommiteeList(meeting?.CommiteeID);
        }

        private bool MeetingExists(int id)
        {
          return _context.Meetings.Any(e => e.ID == id);
        }
    }
}
