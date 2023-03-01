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
    public class AnnouncementsController : Controller
    {
        private readonly NCPACContext _context;

        public AnnouncementsController(NCPACContext context)
        {
            _context = context;
        }

        // GET: Announcements
        public async Task<IActionResult> Index()
        {
              return View(await _context.Announcements.ToListAsync());
        }


        // GET: Announcements/Create
        [Authorize(Roles = "Admin,Supervisor,Staff")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Announcements/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin,Supervisor,Staff")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Subject,AnnouncementDescription,TimePosted")] Announcement announcement)
        {
            if (ModelState.IsValid)
            {
                announcement.TimePosted = DateTime.Now;
                _context.Add(announcement);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(announcement);
        }


        // GET: Announcements/Delete/5
        [Authorize(Roles = "Admin,Supervisor,Staff")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Announcements == null)
            {
                return NotFound();
            }

            var announcement = await _context.Announcements
                .FirstOrDefaultAsync(m => m.ID == id);
            if (announcement == null)
            {
                return NotFound();
            }

            return View(announcement);
        }

        // POST: Announcements/Delete/5
        [Authorize(Roles = "Admin,Supervisor,Staff")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Announcements == null)
            {
                return Problem("Entity set 'NCPACContext.Announcements'  is null.");
            }
            var announcement = await _context.Announcements.FindAsync(id);
            if (announcement != null)
            {
                _context.Announcements.Remove(announcement);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AnnouncementExists(int id)
        {
          return _context.Announcements.Any(e => e.ID == id);
        }
    }
}
