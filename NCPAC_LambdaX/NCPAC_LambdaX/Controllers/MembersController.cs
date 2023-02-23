using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using NCPAC_LambdaX.Utilities;
using NCPAC_LambdaX.ViewModels;
using NCPAC_LambdaX.Data;
using NCPAC_LambdaX.Models;
using OfficeOpenXml;
using System.Numerics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Identity;

namespace NCPAC_LambdaX.Controllers
{
    [Authorize]
    public class MembersController : Controller
    {
        private readonly NCPACContext _context;
        public MembersController(NCPACContext context)
        {
            _context = context;
        }

        // GET: Members
        [Authorize(Roles ="Admin,Supervisor")]
        public async Task<IActionResult> Index(string SearchString, string SearchString4, string SearchString2, bool IsNcGrad, bool ShowInactive, int? CommiteeIDVal,
            int? page, int? pageSizeID, string actionButton, string sortDirection = "asc", string sortField = "Members")
        {
            CookieHelper.CookieSet(HttpContext, "MembersController" + "URL", "", -1);

            ViewData["Filtering"] = "";

            string[] sortOptions = new[] { "Member" };

            var members = _context.Members
                .Include(m => m.MemberCommitees).ThenInclude(mc => mc.Commitee)
                .Include(d => d.Province)
                .AsNoTracking();

            if (!String.IsNullOrEmpty(SearchString))
            {
                members = members.Where(p => p.Salutation.ToUpper().Contains(SearchString.ToUpper())
                                       || p.LastName.ToUpper().Contains(SearchString.ToUpper())
                                       || p.FirstName.ToUpper().Contains(SearchString.ToUpper())
                                       || p.MiddleName.ToUpper().Contains(SearchString.ToUpper()));
            }
            if (!String.IsNullOrEmpty(SearchString2))
            {
                members = members.Where(p => p.Email.Contains(SearchString2) || p.WorkEmail.Contains(SearchString2));
            }
            if (IsNcGrad == true)
            {
                members = members.Where(p => p.IsNCGrad == true);
            }
            if (ShowInactive== true)
            {
                members = members.Where(p => p.IsActive == false);
            }
            else
            {
                members = members.Where(p => p.IsActive == true);
            }
            if (!String.IsNullOrEmpty(SearchString4))
            {
                members = members.Where(p => p.MemberCommitees.Any(p => p.Commitee.CommiteeName.ToUpper().Contains(SearchString4.ToUpper())));
                ViewData["Filtering"] = " show";
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
                    members = members
                        .OrderBy(p => p.LastName).ThenBy(p => p.FirstName).ThenBy(p => p.MiddleName);
                }
                else
                {
                    members = members
                       .OrderByDescending(p => p.LastName).ThenBy(p => p.FirstName).ThenBy(p => p.MiddleName);
                }
            }

            ViewData["sortField"] = sortField;
            ViewData["sortDirection"] = sortDirection;

            int pageSize = PageSizeHelper.SetPageSize(HttpContext, pageSizeID, "Members");
            ViewData["pageSizeID"] = PageSizeHelper.PageSizeList(pageSize);

            var pagedData = await PaginatedList<Member>.CreateAsync(members.AsNoTracking(), page ?? 1, pageSize);

            return View(pagedData);
        }

        // GET: Members/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Members == null)
            {
                return NotFound();
            }

            var member = await _context.Members
                .Include(m => m.MemberCommitees).ThenInclude(mc => mc.Commitee)
                .Include(d => d.Province)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID == id);
            if (member == null)
            {
                return NotFound();
            }

            return View(member);
        }

        // GET: Members/Create
        public IActionResult Create()
        {
            var member = new Member();
            PopulateAssignedMemberCommiteesData(member);
            PopulateDropDownLists();
            return View();
        }

        // POST: Members/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,FirstName,MiddleName,LastName,Salutation,StreetAddress,City,ProvinceID,PostalCode,Phone,Email,WorkStreetAddress,WorkCity,WorkProvinceID,WorkPostalCode,WorkPhone,WorkEmail,MailPrefferenceID,EducationalSummary,IsNCGrad,OccupationalSummary,DateJoined")] Member member, string[] selectedOptions)
        {

            //Add the selected memberCommitees
            try
            {
                //Add the selected memberCommitees
                if (selectedOptions != null)
                {
                    foreach (var memberCommitee in selectedOptions)
                    {
                        var memberCommiteeToAdd = new MemberCommitee { MemberID = member.ID, CommiteeID = int.Parse(memberCommitee) };
                        member.MemberCommitees.Add(memberCommiteeToAdd);
                    }
                }
                if (ModelState.IsValid)
                {
                    _context.Add(member);
                    await _context.SaveChangesAsync();
                    //return RedirectToAction(nameof(Index));
                    return RedirectToAction("Details", new { member.ID });
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                ModelState.AddModelError("", "Unable to save changes after multiple attempts. Try again, and if the problem persists, see your system administrator.");
            }
            catch (DbUpdateException dex)
            {
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }
            if (ModelState.IsValid)
            {
                _context.Add(member);
                await _context.SaveChangesAsync();
                //return RedirectToAction(nameof(Index));
                return RedirectToAction("Details", new { member.ID });
            }

            if (ModelState.IsValid)
            {
                _context.Add(member);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            PopulateDropDownLists(member);
            return View(member);
        }

        // GET: Members/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Members == null)
            {
                return NotFound();
            }

            var member = await _context.Members
                .Include(m => m.MemberCommitees).ThenInclude(p => p.Commitee)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (member == null)
            {
                return NotFound();
            }
            PopulateAssignedMemberCommiteesData(member);
            PopulateDropDownLists(member);

            return View(member);
        }

        // POST: Members/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,FirstName,MiddleName,LastName,Salutation,StreetAddress,City,ProvinceID,PostalCode,Phone,Email,WorkStreetAddress,WorkCity,WorkProvinceID,WorkPostalCode,WorkPhone,WorkEmail,MailPrefferenceID,EducationalSummary,IsNCGrad,OccupationalSummary,DateJoined")] string[] selectedOptions)
        {
            var memberToUpdate = await _context.Members
                .Include(m => m.MemberCommitees).ThenInclude(p => p.Commitee)
                .Include(d => d.Province)
                .FirstOrDefaultAsync(m => m.ID == id);

            //Update the plays
            UpdateMemberCommitees(selectedOptions, memberToUpdate);

            //Try updating it with the values posted
            if (await TryUpdateModelAsync<Member>(memberToUpdate, "",
                p => p.FirstName, p => p.MiddleName, p => p.LastName, p => p.DateJoined, p => p.StreetAddress, p => p.WorkStreetAddress, p => p.City, p => p.WorkCity,
                p => p.PostalCode, p => p.WorkPostalCode, p => p.ProvinceID, p => p.WorkProvinceID, p => p.Phone, p => p.WorkPhone, p => p.Email, p => p.WorkEmail, p => p.MailPrefferenceID,
                p => p.IsNCGrad, p => p.EducationalSummary, p => p.OccupationalSummary, p => p.Salutation, p=> p.IsActive))
            {
                
            }
            try
            {
                await _context.SaveChangesAsync();
                //return RedirectToAction(nameof(Index));
                return RedirectToAction("Details", new { memberToUpdate.ID });
            }
            catch (RetryLimitExceededException /* dex */)
            {
                ModelState.AddModelError("", "Unable to save changes after multiple attempts. Try again, and if the problem persists, see your system administrator.");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MemberExists(memberToUpdate.ID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            catch (DbUpdateException dex)
            {
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }

            PopulateDropDownLists(memberToUpdate);
            PopulateAssignedMemberCommiteesData(memberToUpdate);
            return View(memberToUpdate);
        }

        // GET: Members/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Members == null)
            {
                return NotFound();
            }

            var member = await _context.Members
                .Include(m => m.MemberCommitees)
                .ThenInclude(mc => mc.Commitee)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID == id);
            if (member == null)
            {
                return NotFound();
            }

            return View(member);
        }

        // POST: Members/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Members == null)
            {
                return Problem("Entity set 'NCPACContext.Members'  is null.");
            }
            var member = await _context.Members.FindAsync(id);
            if (member != null)
            {
                _context.Members.Remove(member);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> InsertFromExcel(IFormFile theExcel)

        {
            //Note: This is a very basic example and has 
            //no ERROR HANDLING.  It also assumes that
            //duplicate values are allowed, both in the 
            //uploaded data and the DbSet.
            if(theExcel != null)
            {
                ExcelPackage excel;
                using (var memoryStream = new MemoryStream())
                {
                    await theExcel.CopyToAsync(memoryStream);
                    excel = new ExcelPackage(memoryStream);
                }
                var workSheet = excel.Workbook.Worksheets[0];
                var start = workSheet.Dimension.Start;
                var end = workSheet.Dimension.End;

                //Start a new list to hold imported objects
                List<Member> members = new List<Member>();

                string middleName = "";
                bool ncgrad = false;

                for (int row = start.Row; row <= end.Row; row++)
                {
                    if (workSheet.Cells[row, 2].Text.Split(' ').Count() > 2)
                    {
                        middleName = workSheet.Cells[row, 2].Text.Split(' ')[1];
                    }
                    if ((workSheet.Cells[row, 6].Text.Contains("Yes")) || (workSheet.Cells[row, 6].Text.Contains("yes")))
                    {
                        ncgrad = true;
                    }
                    else if ((workSheet.Cells[row, 6].Text.Contains("No")) || (workSheet.Cells[row, 6].Text.Contains("no")))
                    {
                        ncgrad = false;
                    }
                    // Row by row...
                    Member a = new Member
                    {
                        Salutation = workSheet.Cells[row, 1].Text,
                        FirstName = workSheet.Cells[row, 2].Text.Split(' ')[0],
                        LastName = workSheet.Cells[row, 2].Text.Split(' ')[^1],
                        MiddleName = middleName,
                        Email = workSheet.Cells[row, 3].Text,
                        WorkEmail = workSheet.Cells[row, 4].Text,
                        MailPrefferenceID = workSheet.Cells[row, 5].Text,
                        IsNCGrad = ncgrad,
                        DateJoined = DateTime.Parse(workSheet.Cells[row, 7].Text),
                        StreetAddress = workSheet.Cells[row, 8].Text,
                        City = workSheet.Cells[row, 9].Text,
                        PostalCode = workSheet.Cells[row, 11].Text,
                        IsActive = true,
                        ProvinceID = workSheet.Cells[row, 10].Text

                    };
                    members.Add(a);
                }
                _context.Members.AddRange(members);
                _context.SaveChanges();
                TempData["Message"] = "Excel data was succesfully uploaded!";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["Message"] = "Cannot upload an empty excel file.";
                return RedirectToAction("Index");
            }
            

        }

        private bool MemberExists(int id)
        {
          return _context.Members.Any(e => e.ID == id);
        }

        private void PopulateAssignedMemberCommiteesData(Member member)
        {
            //For this to work, you must have Included the MemberCommitees 
            //in the Member
            var allOptions = _context.Commitees;
            var currentOptionIDs = new HashSet<int>(member.MemberCommitees.Select(b => b.CommiteeID));
            var checkBoxes = new List<CheckOptionVM>();
            foreach (var option in allOptions)
            {
                checkBoxes.Add(new CheckOptionVM
                {
                    ID = option.ID,
                    DisplayText = option.CommiteeName,
                    Assigned = currentOptionIDs.Contains(option.ID)
                });
            }
            ViewData["MemberCommiteeOptions"] = checkBoxes;
        }
        private void UpdateMemberCommitees(string[] selectedOptions, Member memberToUpdate)
        {
            if (selectedOptions == null)
            {
                memberToUpdate.MemberCommitees = new List<MemberCommitee>();
                return;
            }

            var selectedOptionsHS = new HashSet<string>(selectedOptions);
            var memberOptionsHS = new HashSet<int>
                (memberToUpdate.MemberCommitees.Select(c => c.CommiteeID));//IDs of the currently selected MemberCommitees
            foreach (var option in _context.Commitees)
            {
                if (selectedOptionsHS.Contains(option.ID.ToString())) //It is checked
                {
                    if (!memberOptionsHS.Contains(option.ID))  //but not currently included
                    {
                        memberToUpdate.MemberCommitees.Add(new MemberCommitee { MemberID = memberToUpdate.ID, CommiteeID = option.ID });
                    }
                }
                else
                {
                    //Checkbox Not checked
                    if (memberOptionsHS.Contains(option.ID)) //but it is currently in the history - so remove it
                    {
                        MemberCommitee memberCommiteeToRemove = memberToUpdate.MemberCommitees.SingleOrDefault(c => c.CommiteeID == option.ID);
                        _context.Remove(memberCommiteeToRemove);
                    }
                }
            }
        }

        private SelectList ProvinceSelectList(string selectedId)
        {
            return new SelectList(_context.Provinces
                .OrderBy(d => d.Name), "ID", "Name", selectedId);
        }

        private SelectList WorkProvinceSelectList(string selectedId)
        {
            return new SelectList(_context.Provinces
                .OrderBy(d => d.Name), "ID", "Name", selectedId);
        }

        private SelectList MailPrefferenceSelectList(string selectedId)
        {
            return new SelectList(_context.MailPrefferences
                .OrderBy(d => d.Name), "ID", "Name", selectedId);
        }

        private void PopulateDropDownLists(Member member = null)
        {
            if ((member?.ProvinceID) != null)
            {
                ViewData["ProvinceID"] = ProvinceSelectList(member.ProvinceID);
            }
            else
            {
                ViewData["ProvinceID"] = ProvinceSelectList(null);
            }

            if ((member?.WorkProvinceID) != null)
            {
                ViewData["WorkProvinceID"] = WorkProvinceSelectList(member.WorkProvinceID);
            }
            else
            {
                ViewData["WorkProvinceID"] = WorkProvinceSelectList(null);
            }

            if ((member?.MailPrefferenceID) != null)
            {
                ViewData["MailPrefferenceID"] = MailPrefferenceSelectList(member.MailPrefferenceID);
            }
            else
            {
                ViewData["MailPrefferenceID"] = MailPrefferenceSelectList(null);
            }
        }
    }
}
