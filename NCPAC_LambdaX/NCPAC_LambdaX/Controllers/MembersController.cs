using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using MVC_Music.Utilities;
using MVC_Music.ViewModels;
using NCPAC_LambdaX.Data;
using NCPAC_LambdaX.Models;

namespace NCPAC_LambdaX.Controllers
{
    public class MembersController : Controller
    {
        private readonly NCPACContext _context;

        public MembersController(NCPACContext context)
        {
            _context = context;
        }

        // GET: Members
        public async Task<IActionResult> Index(string SearchString, string SearchString4, string SearchString2, string SearchString3, bool IsNcGrad, int? CommiteeIDVal,
            int? page, int? pageSizeID, string actionButton, string sortDirection = "asc", string sortField = "Members")
        {
            CookieHelper.CookieSet(HttpContext, "MembersController" + "URL", "", -1);

            ViewData["Filtering"] = "";

            string[] sortOptions = new[] { "Member", "Date Joined", "Home Address" };

            var members = _context.Members
                .Include(m => m.MemberCommitees).ThenInclude(mc => mc.Commitee)
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
            if (!String.IsNullOrEmpty(SearchString3))
            {
                members = members.Where(p => p.HomeAddress.Contains(SearchString3));
            }
            if (IsNcGrad == true)
            {
                members = members.Where(p => p.IsNCGrad == true);
            }
            if (CommiteeIDVal.HasValue)
            {
                members = members.Where(p => p.MemberCommitees.Any(p => p.CommiteeID == CommiteeIDVal));
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

            if (sortField == "Date Joined")
            {
                if (sortDirection == "asc")
                {
                    members = members
                        .OrderBy(p => p.DateJoined);
                }
                else
                {
                    members = members
                       .OrderByDescending(p => p.DateJoined);
                }
            }
            else if (sortField == "Home Address")
            {
                if (sortDirection == "asc")
                {
                    members = members
                        .OrderBy(p => p.StreetAddress).ThenBy(p => p.City).ThenBy(p => p.PostalCode).ThenBy(p => p.Province);
                }
                else
                {
                    members = members
                       .OrderByDescending(p => p.StreetAddress).ThenBy(p => p.City).ThenBy(p => p.PostalCode).ThenBy(p => p.Province);
                }
            }
            else //Sorting by Patient Name
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

            int pageSize = PageSizeHelper.SetPageSize(HttpContext, pageSizeID, "Musicians");
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
            return View();
        }

        // POST: Members/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,FirstName,MiddleName,LastName,Salutation,StreetAddress,City,Province,PostalCode,Phone,Email,WorkStreetAddress,WorkCity,WorkProvince,WorkPostalCode,WorkPhone,WorkEmail,PrefferedEmail,EducationalSummary,IsNCGrad,OccupationalSummary,DateJoined")] Member member, string[] selectedOptions)
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
            return View(member);
        }

        // POST: Members/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,FirstName,MiddleName,LastName,Salutation,StreetAddress,City,Province,PostalCode,Phone,Email,WorkStreetAddress,WorkCity,WorkProvince,WorkPostalCode,WorkPhone,WorkEmail,PrefferedEmail,EducationalSummary,IsNCGrad,OccupationalSummary,DateJoined")] string[] selectedOptions)
        {
            var memberToUpdate = await _context.Members
                .Include(m => m.MemberCommitees).ThenInclude(p => p.Commitee)
                .FirstOrDefaultAsync(m => m.ID == id);

            //Update the plays
            UpdateMemberCommitees(selectedOptions, memberToUpdate);

            //Try updating it with the values posted
            if (await TryUpdateModelAsync<Member>(memberToUpdate, "",
                p => p.FirstName, p => p.MiddleName, p => p.LastName, p => p.DateJoined, p => p.StreetAddress, p => p.WorkStreetAddress, p => p.City, p => p.WorkCity,
                p => p.PostalCode, p => p.WorkPostalCode, p => p.Province, p => p.WorkProvince, p => p.Phone, p => p.WorkPhone, p => p.Email, p => p.WorkEmail, p => p.PrefferedEmail,
                p => p.IsNCGrad, p => p.EducationalSummary, p => p.OccupationalSummary, p => p.Salutation))
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
                if (dex.GetBaseException().Message.Contains("UNIQUE constraint failed"))
                {
                    ModelState.AddModelError("SIN", "Unable to save changes. Remember, you cannot have duplicate SIN numbers.");
                }
                else
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
                }
            }
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

    }
}
