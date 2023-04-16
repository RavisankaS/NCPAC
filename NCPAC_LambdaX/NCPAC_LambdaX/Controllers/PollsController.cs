using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NCPAC_LambdaX.Data;
using NCPAC_LambdaX.Models;
using NCPAC_LambdaX.ViewModels;
using NuGet.Configuration;

namespace NCPAC_LambdaX.Controllers
{
    public class PollsController : Controller
    {
        private readonly NCPACContext _context;

        public PollsController(NCPACContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var polls = _context.Polls.Include(p => p.Options).Include(p => p.Commitee).Include(p => p.Votes).AsNoTracking(); 

            var member = await _context.Members
                .Include(m => m.MemberCommitees)
                .Where(e => e.Email == User.Identity.Name || e.WorkEmail == User.Identity.Name)
                .AsNoTracking()
                .FirstOrDefaultAsync();


            if ((User.IsInRole("Admin") || User.IsInRole("Supervisor") || User.IsInRole("Staff")) == false)
            {
                polls = polls.Where(m => (m.Commitee.MemberCommitees.Any(a => a.MemberID == member.ID)));
            }

            return View(polls);
        }

        public IActionResult Create()
        {
            Poll poll = new Poll();
            PopulateDropDownLists(poll);
            return View(poll);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Poll model)
        {
            model.Options = model.Options.Where(o => !string.IsNullOrWhiteSpace(o.Text)).ToList();


            _context.Add(model);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Results(int id)
        {
            var poll = await _context.Polls
                .Include(p => p.Options)
                .Include(p => p.Votes)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (poll == null)
            {
                return NotFound();
            }

            var results = poll.Options.Select(o => new
            {
                OptionText = o.Text,
                VoteCount = poll.Votes.Count(v => v.SelectedOption == o.Id)
            });

            ViewBag.PollQuestion = poll.Question;
            return View(results);
        }

        public async Task<IActionResult> Vote(int id)
        {
            var poll = await _context.Polls.Include(p => p.Options).Include(p => p.Commitee).Include(p =>p.Votes).FirstOrDefaultAsync(p => p.Id == id);

            if (poll == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var vote = await _context.PollVotes.FirstOrDefaultAsync(v => v.PollId == id && v.UserId == userId);
            if (vote != null)
            {
                ViewBag.AlreadyVoted = true;
                return View(await _context.Polls.Include(p => p.Options).FirstOrDefaultAsync(p => p.Id == id));
            }

            return View(poll);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Vote(int id, int optionId)
        {
            var poll = await _context.Polls.Include(p => p.Options).Include(p => p.Commitee).Include(p => p.Votes).FirstOrDefaultAsync(p => p.Id == id);

            if (poll == null)
            {
                return NotFound();
            }

            var option = poll.Options.FirstOrDefault(o => o.Id == optionId);

            if (option == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return BadRequest();
            }

            var vote = await _context.PollVotes.FirstOrDefaultAsync(v => v.PollId == id && v.UserId == userId);

            if (vote != null)
            {
                return RedirectToAction(nameof(Index));
            }

            vote = new PollVote
            {
                PollId = id,
                SelectedOption = optionId,
                UserId = userId
            };

            _context.Add(vote);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private SelectList CommiteeeSelectList()
        {
            return new SelectList(_context.Commitees
                .OrderBy(d => d.CommiteeName), "ID", "CommiteeName");
        }

        private void PopulateDropDownLists(Poll poll = null)
        {
            ViewData["CommiteeID"] = CommiteeeSelectList();
        }


    }
}
