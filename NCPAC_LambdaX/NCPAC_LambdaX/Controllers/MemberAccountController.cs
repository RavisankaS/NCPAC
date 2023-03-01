using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using NCPAC_LambdaX.Data;
using NCPAC_LambdaX.Models;
using NCPAC_LambdaX.Utilities;
using NCPAC_LambdaX.ViewModels;

namespace NCPAC_LambdaX.Controllers
{
    public class MemberAccountController : Controller
    {
        private readonly NCPACContext _context;

        public MemberAccountController(NCPACContext context)
        {
            _context = context;
        }

        // GET: MemberAccount
        public async Task<IActionResult> Index()
        {
            return RedirectToAction(nameof(Details));
        }

        // GET: MemberAccount/Details/5
        public async Task<IActionResult> Details()
        {
            var member = await _context.Members
                .Include(m => m.MailPrefference)
                .Include(m => m.Province)
                .Include(m => m.WorkProvince)
                .Where(e => e.Email == User.Identity.Name || e.WorkEmail == User.Identity.Name)
                .Select(e => new MemberVM
                {
                    ID = e.ID,
                    FirstName = e.FirstName,
                    MiddleName = e.MiddleName,
                    LastName = e.LastName,
                    Salutation = e.Salutation,
                    StreetAddress = e.StreetAddress,
                    City = e.City,
                    ProvinceID = e.ProvinceID,
                    Province = e.Province,
                    PostalCode = e.PostalCode,
                    Phone = e.Phone,
                    Email = e.Email,
                    WorkStreetAddress = e.WorkStreetAddress,
                    WorkCity = e.WorkCity,
                    WorkProvinceID = e.WorkProvinceID,
                    WorkProvince = e.WorkProvince,
                    WorkPostalCode = e.WorkPostalCode,
                    WorkPhone = e.WorkPhone,
                    WorkEmail = e.WorkEmail,
                    MailPrefferenceID = e.MailPrefferenceID,
                    MailPrefference = e.MailPrefference,
                    EducationalSummary = e.EducationalSummary,
                    IsNCGrad = e.IsNCGrad,
                    OccupationalSummary = e.OccupationalSummary,
                    MemberCommitees = e.MemberCommitees
                }).AsNoTracking()
                .FirstOrDefaultAsync();
            if (member == null)
            {
                return NotFound();
            }

            return View(member);
        }

        // GET: MemberAccount/Edit/5
        public async Task<IActionResult> Edit()
        {
            var member = await _context.Members
                .Include(m => m.MailPrefference)
                .Include(m => m.Province)
                .Include(m => m.WorkProvince)
                .Where(e => e.Email == User.Identity.Name || e.WorkEmail == User.Identity.Name)
                .Select(e => new MemberVM
                {
                    ID = e.ID,
                    FirstName = e.FirstName,
                    MiddleName = e.MiddleName,
                    LastName = e.LastName,
                    Salutation = e.Salutation,
                    StreetAddress = e.StreetAddress,
                    City = e.City,
                    ProvinceID = e.ProvinceID,
                    Province = e.Province,
                    PostalCode = e.PostalCode,
                    Phone = e.Phone,
                    Email = e.Email,
                    WorkStreetAddress = e.WorkStreetAddress,
                    WorkCity = e.WorkCity,
                    WorkProvinceID = e.WorkProvinceID,
                    WorkProvince = e.WorkProvince,
                    WorkPostalCode = e.WorkPostalCode,
                    WorkPhone = e.WorkPhone,
                    WorkEmail = e.WorkEmail,
                    MailPrefferenceID = e.MailPrefferenceID,
                    MailPrefference = e.MailPrefference,
                    EducationalSummary = e.EducationalSummary,
                    IsNCGrad = e.IsNCGrad,
                    OccupationalSummary = e.OccupationalSummary,
                    MemberCommitees = e.MemberCommitees
                }).AsNoTracking()
                .FirstOrDefaultAsync();
            if (member == null)
            {
                return NotFound();
            }
            PopulateAssignedMemberCommiteesData(member);
            PopulateDropDownLists(member);

            return View(member);
        }

        // POST: MemberAccount/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string[] selectedOptions)
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
                p => p.IsNCGrad, p => p.EducationalSummary, p => p.OccupationalSummary, p => p.Salutation, p => p.IsActive))
            {

            }
            try
            {
                _context.Update(memberToUpdate);
                await _context.SaveChangesAsync();
                UpdateUserNameCookie(memberToUpdate.FirstName + " " + memberToUpdate.LastName);
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

            MemberAdminVM memberAdminVM = new MemberAdminVM
            {
                DateJoined = memberToUpdate.DateJoined,
                IsActive = memberToUpdate.IsActive,
                ID = memberToUpdate.ID,
                FirstName = memberToUpdate.FirstName,
                MiddleName = memberToUpdate.MiddleName,
                LastName = memberToUpdate.LastName,
                Salutation = memberToUpdate.Salutation,
                StreetAddress = memberToUpdate.StreetAddress,
                City = memberToUpdate.City,
                ProvinceID = memberToUpdate.ProvinceID,
                Province = memberToUpdate.Province,
                PostalCode = memberToUpdate.PostalCode,
                Phone = memberToUpdate.Phone,
                Email = memberToUpdate.Email,
                WorkStreetAddress = memberToUpdate.WorkStreetAddress,
                WorkCity = memberToUpdate.WorkCity,
                WorkProvinceID = memberToUpdate.WorkProvinceID,
                WorkProvince = memberToUpdate.WorkProvince,
                WorkPostalCode = memberToUpdate.WorkPostalCode,
                WorkPhone = memberToUpdate.WorkPhone,
                WorkEmail = memberToUpdate.WorkEmail,
                MailPrefferenceID = memberToUpdate.MailPrefferenceID,
                MailPrefference = memberToUpdate.MailPrefference,
                EducationalSummary = memberToUpdate.EducationalSummary,
                IsNCGrad = memberToUpdate.IsNCGrad,
                OccupationalSummary = memberToUpdate.OccupationalSummary,
                MemberCommitees = memberToUpdate.MemberCommitees
            };

            PopulateDropDownLists(memberAdminVM);
            PopulateAssignedMemberCommiteesData(memberAdminVM);
            return View(memberToUpdate);
        }

        private bool MemberExists(int id)
        {
            return _context.Members.Any(e => e.ID == id);
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
        private void UpdateUserNameCookie(string userName)
        {
            CookieHelper.CookieSet(HttpContext, "userName", userName, 960);
        }

        private bool MemberVMExists(int id)
        {
          return _context.MemberVM.Any(e => e.ID == id);
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

        private void PopulateDropDownLists(MemberVM member = null)
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


        private void PopulateAssignedMemberCommiteesData(MemberVM member)
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
    }
}
