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
using NCPAC_LambdaX.Utilities;
using NCPAC_LambdaX.ViewModels;

namespace NCPAC_LambdaX.Controllers
{
    [Authorize]
    public class ActionItemsController : Controller
    {
        private readonly NCPACContext _context;

        public ActionItemsController(NCPACContext context)
        {
            _context = context;
        }

        // GET: ActionItems
        public async Task<IActionResult> Index(string SearchString, string SearchString2, string SearchString3,  bool IsCompleted, 
            int? page, int? pageSizeID, string actionButton, string sortDirection = "dsc", string sortField = "TimeUntil")
        {

            ViewData["Filtering"] = "";

            string[] sortOptions = new[] { "Member", "TimeAppointed", "TimeUntil" };

            var actionItems = _context.ActionItems
            .Include(m => m.Member)
            .ThenInclude(m => m.MemberCommitees)
            .Include(m => m.Meeting)
            .AsNoTracking();

                        
            if ((User.IsInRole("Admin") || User.IsInRole("Supervisor") || User.IsInRole("Staff")) == false)
            {
                actionItems = actionItems.Where(m => (m.Member.Email == User.Identity.Name) || (m.Member.WorkEmail == User.Identity.Name));
            }

            if (!String.IsNullOrEmpty(SearchString3))
            {
                actionItems = actionItems.Where(p => p.Member.Salutation.ToUpper().Contains(SearchString3.ToUpper())
                                       || p.Member.LastName.ToUpper().Contains(SearchString3.ToUpper())
                                       || p.Member.FirstName.ToUpper().Contains(SearchString3.ToUpper())
                                       || p.Member.MiddleName.ToUpper().Contains(SearchString3.ToUpper()));
                ViewData["Filtering"] = "show";
            }
            if (!String.IsNullOrEmpty(SearchString2))
            {
                actionItems = actionItems.Where(p => p.Description.ToLower().Contains(SearchString2.ToLower()));
                ViewData["Filtering"] = "show";
            }
            if (!String.IsNullOrEmpty(SearchString))
            {
                actionItems = actionItems.Where(p => p.ActionItemTitle.ToLower().Contains(SearchString2.ToLower()));
                ViewData["Filtering"] = "show";
            }
            if (IsCompleted == true)
            {
                actionItems = actionItems.Where(p => p.IsCompleted == true);
                ViewData["Filtering"] = "show";
            }
            else
            {
                actionItems = actionItems.Where(p => p.IsCompleted == false);
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

            if (sortField == "Member")
            {
                if (sortDirection == "asc")
                {
                    actionItems = actionItems
                        .OrderBy(p => p.Member.LastName).ThenBy(p => p.Member.FirstName).ThenBy(p => p.Member.MiddleName);
                }
                else
                {
                    actionItems = actionItems
                       .OrderByDescending(p => p.Member.LastName).ThenBy(p => p.Member.FirstName).ThenBy(p => p.Member.MiddleName);
                }
            }
            if (sortField == "TimeAppointed")
            {
                if (sortDirection == "asc")
                {
                    actionItems = actionItems
                        .OrderBy(p => p.Member.LastName).ThenBy(p => p.Member.FirstName).ThenBy(p => p.Member.MiddleName);
                }
                else
                {
                    actionItems = actionItems
                       .OrderByDescending(p => p.Member.LastName).ThenBy(p => p.Member.FirstName).ThenBy(p => p.Member.MiddleName);
                }
            }
            if (sortField == "TimeAppointed")
            {
                if (sortDirection == "asc")
                {
                    actionItems = actionItems
                        .OrderBy(p => p.TimeAppointed);
                }
                else
                {
                    actionItems = actionItems
                       .OrderByDescending(p => p.TimeAppointed);
                }
            }
            else
            {
                if (sortDirection == "asc")
                {
                    actionItems = actionItems
                        .OrderBy(p => p.TimeUntil);
                }
                else
                {
                    actionItems = actionItems
                       .OrderByDescending(p => p.TimeUntil);
                }
            }

            ViewData["sortField"] = sortField;
            ViewData["sortDirection"] = sortDirection;

            int pageSize = PageSizeHelper.SetPageSize(HttpContext, pageSizeID, "Members");
            ViewData["pageSizeID"] = PageSizeHelper.PageSizeList(pageSize);

            var pagedData = await PaginatedList<ActionItem>.CreateAsync(actionItems.AsNoTracking(), page ?? 1, pageSize);

            return View(pagedData);
        }

        // GET: ActionItems/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.ActionItems == null)
            {
                return NotFound();
            }


            var actionItem = await _context.ActionItems
                .Include(a => a.Member)
                .Include(d => d.ActionItemDocuments)
                .Include(a => a.Meeting)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (actionItem == null)
            {
                return NotFound();
            }

            if ((User.IsInRole("Admin") || User.IsInRole("Supervisor") || User.IsInRole("Staff")) == false)
            {
                if (((User.Identity.Name == actionItem.Member.Email) || (User.Identity.Name == actionItem.Member.WorkEmail)) == false)
                {
                    return NotFound();
                }
            }

            return View(actionItem);
        }

        // GET: ActionItems/Create
        [Authorize(Roles = "Admin,Supervisor,Staff")]
        public IActionResult Create()
        {
            ViewData["MemberID"] = new SelectList(_context.Members, "ID", "MemberName");
            PopulateDropDownLists();
            return View();
        }

        // POST: ActionItems/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin,Supervisor,Staff")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,ActionItemTitle,Description,MeetingID,MemberID,TimeAppointed,TimeUntil")] ActionItem actionItem, List<IFormFile> theFiles)
        {
            await AddDocumentsAsync(actionItem, theFiles);
            _context.Add(actionItem);
            await _context.SaveChangesAsync();
            PopulateDropDownLists();
            return RedirectToAction(nameof(Index));     
        }

        // GET: ActionItems/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.ActionItems == null)
            {
                return NotFound();
            }

            var actionItem = await _context.ActionItems
                .Include(d => d.ActionItemDocuments)
                .FirstOrDefaultAsync(d => d.ID == id);
            if (actionItem == null)
            {
                return NotFound();
            }
            PopulateDropDownLists(actionItem);
            return View(actionItem);
        }

        // POST: ActionItems/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,ActionItemTitle,Description,MeetingID,MemberID,TimeAppointed,TimeUntil,IsCompleted")] ActionItem actionItemToUpdate, List<IFormFile> theFiles)
        {
            if (id != actionItemToUpdate.ID)
            {
                return NotFound();
            }

            try
            {
                await AddDocumentsAsync(actionItemToUpdate, theFiles);
                _context.Update(actionItemToUpdate);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ActionItemExists(actionItemToUpdate.ID))
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

        // GET: ActionItems/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.ActionItems == null)
            {
                return NotFound();
            }

            var actionItem = await _context.ActionItems
                .Include(a => a.Member)
                .Include(a => a.Meeting)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (actionItem == null)
            {
                return NotFound();
            }

            return View(actionItem);
        }

        // POST: ActionItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.ActionItems == null)
            {
                return Problem("Entity set 'NCPACContext.ActionItems'  is null.");
            }
            var actionItem = await _context.ActionItems.FindAsync(id);
            if (actionItem != null)
            {
                _context.ActionItems.Remove(actionItem);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<FileContentResult> Download(int id)
        {
            var theFile = await _context.UploadedFiles
                .Include(d => d.FileContent)
                .Where(f => f.ID == id)
                .FirstOrDefaultAsync();
            return File(theFile.FileContent.Content, theFile.MimeType, theFile.FileName);
        }

        private async Task AddDocumentsAsync(ActionItem actionItem, List<IFormFile> theFiles)
        {
            foreach (var f in theFiles)
            {
                if (f != null)
                {
                    string mimeType = f.ContentType;
                    string fileName = Path.GetFileName(f.FileName);
                    long fileLength = f.Length;
                    //Note: you could filter for mime types if you only want to allow
                    //certain types of files.  I am allowing everything.
                    if (!(fileName == "" || fileLength == 0))//Looks like we have a file!!!
                    {
                        ActionItemDocument d = new();
                        using (var memoryStream = new MemoryStream())
                        {
                            await f.CopyToAsync(memoryStream);
                            d.FileContent.Content = memoryStream.ToArray();
                        }
                        d.MimeType = mimeType;
                        d.FileName = fileName;
                        actionItem.ActionItemDocuments.Add(d);
                    };
                }
            }
        }

        private bool ActionItemExists(int id)
        {
          return _context.ActionItems.Any(e => e.ID == id);
        }

        private SelectList MemberSelectList(int selectedId)
        {
            return new SelectList(_context.Members
                .OrderBy(d => d.LastName), "ID", "MemberName", selectedId);
        }

        private SelectList MemberCSelectList()
        {
            return new SelectList(_context.Members
                .OrderBy(d => d.LastName), "ID", "MemberName");
        }

        private SelectList MeetingSelectList(int selectedId)
        {
            return new SelectList(_context.Meetings
                .OrderBy(d => d.TimeFrom), "ID", "MeetingTitle", selectedId);
        }

        private SelectList MeetingsSelectList()
        {
            return new SelectList(_context.Meetings
                .OrderBy(d => d.TimeFrom), "ID", "MeetingTitle");
        }

        private void PopulateDropDownLists(ActionItem actionItem = null)
        {
            
            if ((actionItem?.MemberID) != null)
            {
                ViewData["MemberID"] = MemberSelectList(actionItem.MemberID);
            }
            else
            {
                ViewData["MemberID"] = MemberCSelectList();
            }

            if ((actionItem?.MeetingID) != null)
            {
                ViewData["MeetingID"] = MeetingSelectList(actionItem.MeetingID);
            }
            else
            {
                ViewData["MeetingID"] = MeetingsSelectList();
            }

        }
    
    }
}
