using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NCPAC_LambdaX.Data;
using NCPAC_LambdaX.Models;

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
        public async Task<IActionResult> Index()
        {
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

            return View(meetings);
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
