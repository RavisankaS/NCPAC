using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NCPAC_LambdaX.Data;
using NCPAC_LambdaX.Models;
using NCPAC_LambdaX.Utilities;
using NCPAC_LambdaX.ViewModels;
using NuGet.Configuration;

namespace NCPAC_LambdaX.Controllers
{
    [Authorize]
    public class PollsController : Controller
    {
        private readonly NCPACContext _context;

        public PollsController(NCPACContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string SearchString, string SearchString1, bool ShowInactive, int? page, int? pageSizeID, string actionButton, 
                                                string sortDirection = "asc", string sortField = "Polls")
        {

            ViewData["Filtering"] = "";

            string[] sortOptions = new[] { "Polls" };

            var polls = _context.Polls.Include(p => p.Options).Include(p => p.Commitee).ThenInclude(c => c.MemberCommitees).Include(p => p.Votes).AsNoTracking(); 

            var member = await _context.Members
                .Include(m => m.MemberCommitees)
                .Where(e => e.Email == User.Identity.Name || e.WorkEmail == User.Identity.Name)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if ((User.IsInRole("Admin") || User.IsInRole("Supervisor") || User.IsInRole("Staff")) == false)
            {
                polls = polls.Where(m => (m.Commitee.MemberCommitees.Any(a => a.MemberID == member.ID)));
            }

            if (!String.IsNullOrEmpty(SearchString))
            {
                polls = polls.Where(p => p.Question.ToUpper().Contains(SearchString.ToUpper()));
            }

            if (ShowInactive == true)
            {
                polls = polls.Where(p => p.IsActive == false);
            }
            else
            {
                polls = polls.Where(p => p.IsActive == true);
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

            if (sortField == "Polls")
            {
                if (sortDirection == "asc")
                {
                    polls = polls
                        .OrderBy(p => p.Question);
                }
                else
                {
                    polls = polls
                       .OrderByDescending(p => p.Question);
                }
            }

            ViewData["sortField"] = sortField;
            ViewData["sortDirection"] = sortDirection;

            int pageSize = PageSizeHelper.SetPageSize(HttpContext, pageSizeID, "Polls");
            ViewData["pageSizeID"] = PageSizeHelper.PageSizeList(pageSize);

            var pagedData = await PaginatedList<Poll>.CreateAsync(polls.AsNoTracking(), page ?? 1, pageSize);
           

            return View(pagedData);
        }

        [Authorize(Roles = "Admin,Supervisor,Staff")]
        public IActionResult Create()
        {
            Poll poll = new Poll();
            PopulateDropDownLists(poll);
            return View(poll);
        }

        [Authorize(Roles = "Admin,Supervisor,Staff")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Poll model)
        {
            model.Options = model.Options.Where(o => !string.IsNullOrWhiteSpace(o.Text)).ToList();
            model.IsActive = true;


            _context.Add(model);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin,Supervisor,Staff")]
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

        [Authorize]
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

        [Authorize]
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

        [Authorize(Roles = "Admin,Supervisor,Staff")]
        public async Task<IActionResult> Archive(int? id)
        {
            if (id == null || _context.Polls == null)
            {
                return NotFound();
            }

            var member = await _context.Polls
                .Include(m => m.Commitee)                
                .FirstOrDefaultAsync(m => m.Id == id);
            if (member == null)
            {
                return NotFound();
            }

            return View(member);
        }

        [Authorize(Roles = "Admin,Supervisor,Staff")]
        [HttpPost, ActionName("ArchiveConfirm")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ArchiveConfirm(int id)
        {
            if (_context.Polls == null)
            {
                return NotFound();
            }
            var member = await _context.Polls.FindAsync(id);

            if (member != null)
            {
                if (member.IsActive == true)
                {
                    member.IsActive = false;
                }
                else
                {
                    member.IsActive = true;
                }

                _context.Entry(member).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                _context.Entry(member).Reload();
            }

            //var members = membe

            return RedirectToAction(nameof(Index));
        }


    }
}
