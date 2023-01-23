using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NCPAC_LambdaX.Data;
using NCPAC_LambdaX.Models;

namespace NCPAC_LambdaX.Controllers
{
    public class CommiteesController : Controller
    {
        private readonly NCPACContext _context;

        public CommiteesController(NCPACContext context)
        {
            _context = context;
        }

        // GET: Commitees
        public async Task<IActionResult> Index()
        {
              return View(await _context.Commitees.ToListAsync());
        }

        // GET: Commitees/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Commitees == null)
            {
                return NotFound();
            }

            var commitee = await _context.Commitees
                .Include(m => m.MemberCommitees).ThenInclude(mc => mc.Member)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID == id);
            if (commitee == null)
            {
                return NotFound();
            }

            return View(commitee);
        }

        // GET: Commitees/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Commitees/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,CommiteeName,Division")] Commitee commitee)
        {
            if (ModelState.IsValid)
            {
                _context.Add(commitee);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(commitee);
        }

        // GET: Commitees/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Commitees == null)
            {
                return NotFound();
            }

            var commitee = await _context.Commitees.FindAsync(id);
            if (commitee == null)
            {
                return NotFound();
            }
            return View(commitee);
        }

        // POST: Commitees/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,CommiteeName,Division")] Commitee commitee)
        {
            if (id != commitee.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(commitee);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CommiteeExists(commitee.ID))
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
            return View(commitee);
        }

        // GET: Commitees/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Commitees == null)
            {
                return NotFound();
            }

            var commitee = await _context.Commitees
                .FirstOrDefaultAsync(m => m.ID == id);
            if (commitee == null)
            {
                return NotFound();
            }

            return View(commitee);
        }

        // POST: Commitees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Commitees == null)
            {
                return Problem("Entity set 'NCPACContext.Commitees'  is null.");
            }
            var commitee = await _context.Commitees.FindAsync(id);
            if (commitee != null)
            {
                _context.Commitees.Remove(commitee);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CommiteeExists(int id)
        {
          return _context.Commitees.Any(e => e.ID == id);
        }
    }
}
