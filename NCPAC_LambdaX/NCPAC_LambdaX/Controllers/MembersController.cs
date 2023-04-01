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
using System.Linq.Expressions;

namespace NCPAC_LambdaX.Controllers
{
    [Authorize]
    public class MembersController : Controller
    {
        private readonly NCPACContext _context;
        private readonly ApplicationDbContext _identityContext;
        private readonly IMyEmailSender _emailSender;
        private readonly UserManager<IdentityUser> _userManager;

        public MembersController(NCPACContext context,
            ApplicationDbContext identityContext, IMyEmailSender emailSender,
            UserManager<IdentityUser> userManager)
        {
            _context = context;
            _identityContext = identityContext;
            _emailSender = emailSender;
            _userManager = userManager;
        }

        // GET: Members
        [Authorize(Roles ="Admin,Supervisor,Staff")]
        public async Task<IActionResult> Index(string SearchString, string SearchString4, string SearchString2, bool IsNcGrad, bool ShowInactive, int? CommiteeIDVal,
            int? page, int? pageSizeID, string actionButton, string sortDirection = "asc", string sortField = "Members")
        {
            CookieHelper.CookieSet(HttpContext, "MembersController" + "URL", "", -1);

            ViewData["Filtering"] = "";

            string[] sortOptions = new[] { "Member" };

            var members = _context.Members
                .Include(m => m.MemberCommitees).ThenInclude(mc => mc.Commitee)
                .Include(d => d.Province)
                .Select(e => new MemberAdminVM
                {
                    DateJoined = e.DateJoined,
                    IsActive = e.IsActive,
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
                }).AsNoTracking();

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

            var pagedData = await PaginatedList<MemberAdminVM>.CreateAsync(members.AsNoTracking(), page ?? 1, pageSize);

            return View(pagedData);
        }

        public async Task<IActionResult> MemberIndex(string SearchString, string SearchString2, bool IsNcGrad, bool ShowInactive, int? CommiteeIDVal,
            int? page, int? pageSizeID, string actionButton, string sortDirection = "asc", string sortField = "Members")
        {
            CookieHelper.CookieSet(HttpContext, "MembersController" + "URL", "", -1);

            ViewData["Filtering"] = "";

            string[] sortOptions = new[] { "Member" };

            var members = _context.Members
                .Include(m => m.MemberCommitees).ThenInclude(mc => mc.Commitee)
                .Include(d => d.Province)
                .Select(e => new MemberAdminVM
                {
                    DateJoined = e.DateJoined,
                    IsActive = e.IsActive,
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
                }).AsNoTracking();

            members = members.Where(m => m.MemberCommitees.Any(m => m.Commitee.MemberCommitees.Any(m => (m.Member.Email == User.Identity.Name) || (m.Member.WorkEmail == User.Identity.Name))));

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
            members = members.Where(p => p.IsActive == true);

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

            var pagedData = await PaginatedList<MemberAdminVM>.CreateAsync(members.AsNoTracking(), page ?? 1, pageSize);

            return View(pagedData);
        }

        // GET: Members/Details/5
        [Authorize(Roles = "Admin,Supervisor,Staff")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Members == null)
            {
                return NotFound();
            }

            var member = await _context.Members
                .Include(m => m.MemberCommitees).ThenInclude(mc => mc.Commitee).ThenInclude(c => c.Meetings)
                .Include(d => d.Province)
                .Include(m => m.ActionItems)
                .Select(e => new MemberAdminVM
                {
                    DateJoined = e.DateJoined,
                    IsActive = e.IsActive,
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
                    MemberCommitees = e.MemberCommitees,
                    ActionItems = e.ActionItems
                }).AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID == id);
            if (member == null)
            {
                return NotFound();
            }

            return View(member);
        }

        // GET: Members/Create
        [Authorize(Roles = "Admin,Supervisor,Staff")]
        public IActionResult Create()
        {
            MemberAdminVM member = new MemberAdminVM();
            member.ProvinceID = "ON";
            member.WorkProvinceID = "ON";
            PopulateAssignedMemberCommiteesData(member);
            PopulateDropDownLists(member);
            return View(member);
        }

        // POST: Members/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin,Supervisor,Staff")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,FirstName,MiddleName,LastName,Salutation,StreetAddress,City,ProvinceID,PostalCode,Phone,Email,WorkStreetAddress,WorkCity,WorkProvinceID,WorkPostalCode,WorkPhone,WorkEmail,MailPrefferenceID,EducationalSummary,IsNCGrad,OccupationalSummary,DateJoined,IsActive")] Member member, string[] selectedOptions)
        {

            //Add the selected memberCommitees
            try
            {
                //Add the selected memberCommitees
                UpdateMemberCommitees(selectedOptions, member);
                member.IsActive = true;
                _context.Add(member);
                await _context.SaveChangesAsync();
                //return RedirectToAction(nameof(Index));
                string EmailAddress = "";
                if ((member.Email != null) || (member.WorkEmail != null))
                {
                    if (member.MailPrefferenceID != "Any")
                    {
                        if (member.MailPrefferenceID == "Work")
                        {
                            EmailAddress = member.WorkEmail;
                        }
                        if (member.MailPrefferenceID == "Personnal")
                        {
                            EmailAddress = member.Email;
                        }
                    }
                    else if (member.Email == null)
                    {
                        EmailAddress = member.WorkEmail;
                    }
                    else if (member.WorkEmail == null)
                    {
                        EmailAddress = member.Email;
                    }
                    else
                    {
                        EmailAddress = member.Email;
                    }

                }

                InsertIdentityUser(EmailAddress);

                //Send Email to new Member - commented out till email configured
            }
            catch (RetryLimitExceededException /* dex */)
            {
                ModelState.AddModelError("", "Unable to save changes after multiple attempts. Try again, and if the problem persists, see your system administrator.");
            }
            catch (DbUpdateException dex)
            {
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }
            MemberAdminVM memberAdminVM = new MemberAdminVM
            {
                DateJoined = member.DateJoined,
                IsActive = member.IsActive,
                ID = member.ID,
                FirstName = member.FirstName,
                MiddleName = member.MiddleName,
                LastName = member.LastName,
                Salutation = member.Salutation,
                StreetAddress = member.StreetAddress,
                City = member.City,
                ProvinceID = member.ProvinceID,
                Province = member.Province,
                PostalCode = member.PostalCode,
                Phone = member.Phone,
                Email = member.Email,
                WorkStreetAddress = member.WorkStreetAddress,
                WorkCity = member.WorkCity,
                WorkProvinceID = member.WorkProvinceID,
                WorkProvince = member.WorkProvince,
                WorkPostalCode = member.WorkPostalCode,
                WorkPhone = member.WorkPhone,
                WorkEmail = member.WorkEmail,
                MailPrefferenceID = member.MailPrefferenceID,
                MailPrefference = member.MailPrefference,
                EducationalSummary = member.EducationalSummary,
                IsNCGrad = member.IsNCGrad,
                OccupationalSummary = member.OccupationalSummary,
                MemberCommitees = member.MemberCommitees
            };

            await InviteUserToResetPassword(memberAdminVM, null);
            PopulateAssignedMemberCommiteesData(memberAdminVM);
            PopulateDropDownLists(memberAdminVM);
            return RedirectToAction("Details", new { memberAdminVM.ID });
        }

        // GET: Members/Edit/5
        [Authorize(Roles = "Admin,Supervisor,Staff")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var member = await _context.Members
                .Include(m => m.MemberCommitees).ThenInclude(p => p.Commitee)
                .Select(m => new MemberAdminVM
                {
                    DateJoined = m.DateJoined,
                    IsActive = m.IsActive,
                    ID = m.ID,
                    FirstName = m.FirstName,
                    MiddleName = m.MiddleName,
                    LastName = m.LastName,
                    Salutation = m.Salutation,
                    StreetAddress = m.StreetAddress,
                    City = m.City,
                    ProvinceID = m.ProvinceID,
                    Province = m.Province,
                    PostalCode = m.PostalCode,
                    Phone = m.Phone,
                    Email = m.Email,
                    WorkStreetAddress = m.WorkStreetAddress,
                    WorkCity = m.WorkCity,
                    WorkProvinceID = m.WorkProvinceID,
                    WorkProvince = m.WorkProvince,
                    WorkPostalCode = m.WorkPostalCode,
                    WorkPhone = m.WorkPhone,
                    WorkEmail = m.WorkEmail,
                    MailPrefferenceID = m.MailPrefferenceID,
                    MailPrefference = m.MailPrefference,
                    EducationalSummary = m.EducationalSummary,
                    IsNCGrad = m.IsNCGrad,
                    OccupationalSummary = m.OccupationalSummary,
                    MemberCommitees = m.MemberCommitees
                }).AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID == id);

            if (member == null)
            {
                return NotFound();
            }

            //Get the user from the Identity system
            var user = await _userManager.FindByEmailAsync(member.Email);
            PopulateAssignedMemberCommiteesData(member);
            PopulateDropDownLists(member);

            return View(member);

            
        }

        // POST: Members/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin,Supervisor,Staff")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, bool IsActive, string MailPrefferenceID, string Email, string WorkEmail, [Bind("ID,FirstName,MiddleName,LastName,Salutation,StreetAddress,City,ProvinceID,PostalCode,Phone,Email,WorkStreetAddress,WorkCity,WorkProvinceID,WorkPostalCode,WorkPhone,WorkEmail,MailPrefferenceID,EducationalSummary,IsNCGrad,OccupationalSummary,DateJoined,IsActive")] string[] selectedOptions)
        {
            var memberToUpdate = await _context.Members
                .FirstOrDefaultAsync(m => m.ID == id);
            if (memberToUpdate == null)
            {
                return NotFound();
            }

            bool mailUpdate = false;

            //Note the current Email and Active Status
            bool ActiveStatus = memberToUpdate.IsActive;
            string databaseEmail = "";
            if ((memberToUpdate.Email != null) || (memberToUpdate.WorkEmail != null))
            {
                if (memberToUpdate.MailPrefferenceID != "Any")
                {
                    if (memberToUpdate.MailPrefferenceID == "Work")
                    {
                        databaseEmail = memberToUpdate.WorkEmail;
                    }
                    if (memberToUpdate.MailPrefferenceID == "Personnal")
                    {
                        databaseEmail = memberToUpdate.Email;
                    }
                }
                else if (memberToUpdate.WorkEmail != null)
                {
                    databaseEmail = memberToUpdate.WorkEmail;
                }
                else if (memberToUpdate.Email != null)
                {
                    databaseEmail = memberToUpdate.Email;
                }
                else
                {
                    databaseEmail = memberToUpdate.Email;
                }

            }

            string EmailAddress = "";
            if ((Email != null) || (WorkEmail != null))
            {
                if (MailPrefferenceID != "Any")
                {
                    if (MailPrefferenceID == "Work")
                    {
                        EmailAddress = WorkEmail;
                    }
                    if (MailPrefferenceID == "Personnal")
                    {
                        EmailAddress = Email;
                    }
                }
                else if (WorkEmail != null)
                {
                    EmailAddress = WorkEmail;
                }
                else if (Email != null)
                {
                    EmailAddress = Email;
                }
                else
                {
                    EmailAddress = Email;
                }

            }


            if (await TryUpdateModelAsync<Member>(memberToUpdate, "",
                p => p.FirstName, p => p.MiddleName, p => p.LastName, p => p.DateJoined, p => p.StreetAddress, p => p.WorkStreetAddress, p => p.City, p => p.WorkCity,
                p => p.PostalCode, p => p.WorkPostalCode, p => p.ProvinceID, p => p.WorkProvinceID, p => p.Phone, p => p.WorkPhone, p => p.Email, p => p.WorkEmail, p => p.MailPrefferenceID,
                p => p.IsNCGrad, p => p.EducationalSummary, p => p.OccupationalSummary, p => p.Salutation, p => p.IsActive))
            {

            }

            UpdateMemberCommitees(selectedOptions, memberToUpdate);

            

            try
            {
                await _context.SaveChangesAsync();
                //Save successful so go on to related changes

                //Check for changes in the Active state
                if (IsActive == false && ActiveStatus == true)
                {
                    //Deactivating them so delete the IdentityUser
                    //This deletes the user's login from the security system
                    await DeleteIdentityUser(EmailAddress);

                }
                else if (IsActive == true && ActiveStatus == false)
                {
                    //You reactivating the user, create them and
                    //give them the selected roles
                    InsertIdentityUser(EmailAddress);
                }
                else if (IsActive == true && ActiveStatus == true)
                {
                    //No change to Active status so check for a change in Email
                    //If you Changed the email, Delete the old login and create a new one
                    //with the selected roles
                    if (EmailAddress != databaseEmail)
                    {                        

                        //This deletes the user's old login from the security system
                        await DeleteIdentityUser(databaseEmail);

                        //Add the new login with the selected roles
                        InsertIdentityUser(EmailAddress);
                        mailUpdate = true;
                    }
                }

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
                    ModelState.AddModelError("Email", "Unable to save changes. Remember, you cannot have duplicate Email addresses.");
                }
                else
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
                }
            }


            //We are here because something went wrong and need to redisplay
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

            if (mailUpdate)
            {
                await InviteUserToResetPassword(memberAdminVM, null);
            }
            PopulateDropDownLists(memberAdminVM);
            PopulateAssignedMemberCommiteesData(memberAdminVM);
            return View(memberAdminVM);

        }

        public async Task<IActionResult> Archive(int? id)
        {
            if (id == null || _context.Members == null)
            {
                return NotFound();
            }

            var member = await _context.Members
                .Include(m => m.MemberCommitees).ThenInclude(mc => mc.Commitee).ThenInclude(c => c.Meetings)
                .Include(d => d.Province)
                .Include(m => m.ActionItems)
                .Select(e => new MemberAdminVM
                {
                    DateJoined = e.DateJoined,
                    IsActive = e.IsActive,
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
                    MemberCommitees = e.MemberCommitees,
                    ActionItems = e.ActionItems
                }).AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID == id);
            if (member == null)
            {
                return NotFound();
            }

            return View(member);
        }

        [HttpPost, ActionName("ArchiveConfirm")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ArchiveConfirm(int id)
        {
            if (_context.Members == null)
            {
                return NotFound();
            }
            var member = await _context.Members.FindAsync(id);

            if (member != null)
            {
                if(member.IsActive == true)
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


        [Authorize(Roles = "Admin,Supervisor,Staff")]
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
                    string EmailAddress = "";
                    if ((a.Email != null) || (a.WorkEmail != null))
                    {
                        if (a.MailPrefferenceID != "Any")
                        {
                            if (a.MailPrefferenceID == "Work")
                            {
                                EmailAddress = a.WorkEmail;
                            }
                            if (a.MailPrefferenceID == "Personnal")
                            {
                                EmailAddress = a.Email;
                            }
                        }
                        else if (a.Email == null)
                        {
                            EmailAddress = a.WorkEmail;
                        }
                        else if (a.WorkEmail == null)
                        {
                            EmailAddress = a.Email;
                        }
                        else
                        {
                            EmailAddress = a.Email;
                        }

                    }
                    InsertIdentityUser(EmailAddress);

                    MemberAdminVM memberAdminVM = new MemberAdminVM
                    {
                        DateJoined = a.DateJoined,
                        IsActive = a.IsActive,
                        ID = a.ID,
                        FirstName = a.FirstName,
                        MiddleName = a.MiddleName,
                        LastName = a.LastName,
                        Salutation = a.Salutation,
                        StreetAddress = a.StreetAddress,
                        City = a.City,
                        ProvinceID = a.ProvinceID,
                        Province = a.Province,
                        PostalCode = a.PostalCode,
                        Phone = a.Phone,
                        Email = a.Email,
                        WorkStreetAddress = a.WorkStreetAddress,
                        WorkCity = a.WorkCity,
                        WorkProvinceID = a.WorkProvinceID,
                        WorkProvince = a.WorkProvince,
                        WorkPostalCode = a.WorkPostalCode,
                        WorkPhone = a.WorkPhone,
                        WorkEmail = a.WorkEmail,
                        MailPrefferenceID = a.MailPrefferenceID,
                        MailPrefference = a.MailPrefference,
                        EducationalSummary = a.EducationalSummary,
                        IsNCGrad = a.IsNCGrad,
                        OccupationalSummary = a.OccupationalSummary,
                        MemberCommitees = a.MemberCommitees
                    };

                    //Send Email to new Employee - commented out till email configured
                    //await InviteUserToResetPassword(memberAdminVM, null);
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


        private void PopulateAssignedMemberCommiteesData(MemberAdminVM member)
        {
            //For this to work, you must have Included the child collection in the parent object
            var allOptions = _context.Commitees;
            var currentOptionsHS = new HashSet<int>(member.MemberCommitees.Select(b => b.CommiteeID));
            //Instead of one list with a boolean, we will make two lists
            var selected = new List<ListOptionVM>();
            var available = new List<ListOptionVM>();
            foreach (var s in allOptions)
            {
                if (currentOptionsHS.Contains(s.ID))
                {
                    selected.Add(new ListOptionVM
                    {
                        ID = s.ID,
                        DisplayText = s.CommiteeName
                    });
                }
                else
                {
                    available.Add(new ListOptionVM
                    {
                        ID = s.ID,
                        DisplayText = s.CommiteeName
                    });
                }
            }

            ViewData["selOpts"] = new MultiSelectList(selected.OrderBy(s => s.DisplayText), "ID", "DisplayText");
            ViewData["availOpts"] = new MultiSelectList(available.OrderBy(s => s.DisplayText), "ID", "DisplayText");
        }


        private void InsertIdentityUser(string Email)
        {
            //Create the IdentityUser in the IdentitySystem
            //Note: this is similar to what we did in ApplicationSeedData
            if (_userManager.FindByEmailAsync(Email).Result == null)
            {
                IdentityUser user = new IdentityUser
                {
                    UserName = Email,
                    Email = Email,
                    EmailConfirmed = true //since we are creating it!
                };
                //Create a random password with a default 8 characters
                string password = MakePassword.Generate();
                IdentityResult result = _userManager.CreateAsync(user, "LambdaX123@").Result;

            }
            else
            {
                TempData["message"] = "The Login Account for " + Email + " was already in the system.";
            }
        }

        private async Task DeleteIdentityUser(string Email)
        {
            var userToDelete = await _identityContext.Users.Where(u => u.Email == Email).FirstOrDefaultAsync();
            if (userToDelete != null)
            {
                _identityContext.Users.Remove(userToDelete);
                await _identityContext.SaveChangesAsync();
            }
        }

        private async Task InviteUserToResetPassword(MemberAdminVM member, string message)
        {
            string EmailAddress = "";
            if ((member.Email != null) || (member.WorkEmail != null))
            {
                if (member.MailPrefferenceID != "Any")
                {
                    if (member.MailPrefferenceID == "Work")
                    {
                        EmailAddress = member.WorkEmail;
                    }
                    if (member.MailPrefferenceID == "Personnal")
                    {
                        EmailAddress = member.Email;
                    }
                }
                else if (member.Email == null)
                {
                    EmailAddress = member.WorkEmail;
                }
                else if (member.WorkEmail == null)
                {
                    EmailAddress = member.Email;
                }
                else
                {
                    EmailAddress = member.Email;
                }

            }
            message ??= "<!DOCTYPE html> " + "<html xmlns:v='urn:schemas-microsoft-com:vml' xmlns:o='urn:schemas-microsoft-com:office:office' lang='en'> " + "<head> " + " <title></title> " + " <meta http-equiv='Content-Type' content='text/html; charset=utf-8'> " + " <meta name='viewport' content='width=device-width, initial-scale=1.0'> " + "  " + "  " + " <link href='https://fonts.googleapis.com/css?family=Roboto+Slab' rel='stylesheet' type='text/css'> " + "  " + " <style> " + " * { " + " box-sizing: border-box; " + " } " + " body { " + " margin: 0; " + " padding: 0; " + " } " + " a[x-apple-data-detectors] { " + " color: inherit !important; " + " text-decoration: inherit !important; " + " } " + " #MessageViewBody a { " + " color: inherit; " + " text-decoration: none; " + " } " + " p { " + " line-height: inherit " + " } " + " .desktop_hide, " + " .desktop_hide table { " + " mso-hide: all; " + " display: none; " + " max-height: 0px; " + " overflow: hidden; " + " } " + " @media (max-width:670px) { " + " .desktop_hide table.icons-inner { " + " display: inline-block !important; " + " } " + " .icons-inner { " + " text-align: center; " + " } " + " .icons-inner td { " + " margin: 0 auto; " + " } " + " .image_block img.big, " + " .row-content { " + " width: 100% !important; " + " } " + " .mobile_hide { " + " display: none; " + " } " + " .stack .column { " + " width: 100%; " + " display: block; " + " } " + " .mobile_hide { " + " min-height: 0; " + " max-height: 0; " + " max-width: 0; " + " overflow: hidden; " + " font-size: 0px; " + " } " + " .desktop_hide, " + " .desktop_hide table { " + " display: table !important; " + " max-height: none !important; " + " } " + " } " + " </style> " + "</head> " + "<body style='background-color: #85a4cd; margin: 0; padding: 0; -webkit-text-size-adjust: none; text-size-adjust: none;'> " + " <table class='nl-container' width='100%' border='0' cellpadding='0' cellspacing='0' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt; background-color: #85a4cd;'> " + " <tbody> " + " <tr> " + " <td> " + " <table class='row row-1' align='center' width='100%' border='0' cellpadding='0' cellspacing='0' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt; background-color: #f3f6fe;'> " + " <tbody> " + " <tr> " + " <td> " + " <table class='row-content stack' align='center' border='0' cellpadding='0' cellspacing='0' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt; color: #000000; width: 650px;' width='650'> " + " <tbody> " + " <tr> " + " <td class='column column-1' width='100%' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt; font-weight: 400; text-align: left; vertical-align: top; padding-top: 15px; padding-bottom: 15px; border-top: 0px; border-right: 0px; border-bottom: 0px; border-left: 0px;'> " + " <table class='image_block block-1' width='100%' border='0' cellpadding='0' cellspacing='0' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt;'> " + " <tr> " + " <td class='pad' style='width:100%;padding-right:0px;padding-left:0px;'> " + " <div class='alignment' align='center' style='line-height:10px'><img src='https://upload.wikimedia.org/wikipedia/commons/thumb/1/12/Niagara-college_vectorized.svg/1200px-Niagara-college_vectorized.svg.png' style='display: block; height: auto; border: 0; width: 163px; max-width: 100%;' width='163' alt='Your Logo' title='Your Logo'></div> " + " </td> " + " </tr> " + " </table> " + " </td> " + " </tr> " + " </tbody> " + " </table> " + " </td> " + " </tr> " + " </tbody> " + " </table> " + " <table class='row row-2' align='center' width='100%' border='0' cellpadding='0' cellspacing='0' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt;'> " + " <tbody> " + " <tr> " + " <td> " + " <table class='row-content stack' align='center' border='0' cellpadding='0' cellspacing='0' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt; color: #000000; width: 650px;' width='650'> " + " <tbody> " + " <tr> " + " <td class='column column-1' width='100%' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt; font-weight: 400; text-align: left; vertical-align: top; padding-top: 5px; padding-bottom: 5px; border-top: 0px; border-right: 0px; border-bottom: 0px; border-left: 0px;'> " + " <table class='heading_block block-2' width='100%' border='0' cellpadding='0' cellspacing='0' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt;'> " + " <tr> " + " <td class='pad' style='padding-bottom:10px;text-align:center;width:100%;padding-top:60px;'> " + " <h1 style='margin: 0; color: #ffffff; direction: ltr; font-family: 'Roboto Slab', Arial, 'Helvetica Neue', Helvetica, sans-serif; font-size: 30px; font-weight: normal; letter-spacing: 2px; line-height: 120%; text-align: center; margin-top: 0; margin-bottom: 0;'><strong>Create a New Password</strong></h1> " + " </td> " + " </tr> " + " </table> " + " <table class='image_block block-3' width='100%' border='0' cellpadding='0' cellspacing='0' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt;'> " + " <tr> " + " <td class='pad' style='width:100%;padding-right:0px;padding-left:0px;'> " + " </td> " + " </tr> " + " </table> " + " <table class='text_block block-5' width='100%' border='0' cellpadding='0' cellspacing='0' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt; word-break: break-word;'> " + " <tr> " + " <td class='pad' style='padding-bottom:5px;padding-left:10px;padding-right:10px;padding-top:25px;'> " + " <div style='font-family: sans-serif'> " + " <div class style='font-size: 14px; font-family: Roboto Slab, Arial, Helvetica Neue, Helvetica, sans-serif; mso-line-height-alt: 16.8px; color: #3f4d75; line-height: 1.2;'> " + " <p style='margin: 0; font-size: 14px; text-align: center; mso-line-height-alt: 16.8px;'><span style='font-size:20px;'>Your PAC Member account for Niagara College Policy Management Committee System was Created!</span></p> " + " </div> " + " </div> " + " </td> " + " </tr> " + " </table> " + " <table class='text_block block-6' width='100%' border='0' cellpadding='0' cellspacing='0' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt; word-break: break-word;'> " + " <tr> " + " <td class='pad' style='padding-bottom:5px;padding-left:10px;padding-right:10px;padding-top:5px;'> " + " <div style='font-family: sans-serif'> " + " <div class style='font-size: 14px; font-family: Roboto Slab, Arial, Helvetica Neue, Helvetica, sans-serif; mso-line-height-alt: 16.8px; color: #3f4d75; line-height: 1.2;'> " + " <p style='margin: 0; font-size: 14px; text-align: center; mso-line-height-alt: 16.8px;'><span style='font-size:22px;'>Use " + EmailAddress + " as your email and reset your password inorder to LogIn.</span></p> " + " </div> " + " </div> " + " </td> " + " </tr> " + " </table> " + " <table class='button_block block-8' width='100%' border='0' cellpadding='0' cellspacing='0' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt;'> " + " <tr> " + " <td class='pad' style='padding-bottom:10px;padding-left:10px;padding-right:10px;padding-top:30px;text-align:center;'> " + " <div class='alignment' align='center'> " + " <a href='https://ncpacproto2.azurewebsites.net/Identity/Account/ForgotPassword' target='_blank' style='text-decoration:none;display:inline-block;color:#3f4d75;background-color:#ffffff;border-radius:10px;width:auto;border-top:2px solid #3F4D75;font-weight:undefined;border-right:2px solid #3F4D75;border-bottom:2px solid #3F4D75;border-left:2px solid #3F4D75;padding-top:10px;padding-bottom:10px;font-family:Roboto Slab, Arial, Helvetica Neue, Helvetica, sans-serif;font-size:18px;text-align:center;mso-border-alt:none;word-break:keep-all;'><span style='padding-left:25px;padding-right:25px;font-size:18px;display:inline-block;letter-spacing:normal;'><span dir='ltr' style='word-break: break-word;'><span style='line-height: 36px;' dir='ltr' data-mce-style>CREATE MY PASSWORD</span></span></span></a> " + "  " + " </div> " + " </td> " + " </tr> " + " </table> " + " <table class='text_block block-10' width='100%' border='0' cellpadding='0' cellspacing='0' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt; word-break: break-word;'> " + " <tr> " + " <td class='pad' style='padding-bottom:40px;padding-left:10px;padding-right:10px;padding-top:30px;'> " + " <div style='font-family: sans-serif'> " + " <div class style='font-size: 14px; font-family: Roboto Slab, Arial, Helvetica Neue, Helvetica, sans-serif; mso-line-height-alt: 16.8px; color: #3f4d75; line-height: 1.2;'> " + " <p style='margin: 0; font-size: 14px; text-align: center; mso-line-height-alt: 16.8px;'><span style='font-size:14px;'>Please click the button above to start resetting your password.</span></p> " + " </div> " + " </div> " + " </td> " + " </tr> " + " </table> " + " </td> " + " </tr> " + " </tbody> " + " </table> " + " </td> " + " </tr> " + " </tbody> " + " </table> " + " <table class='row row-3' align='center' width='100%' border='0' cellpadding='0' cellspacing='0' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt; background-color: #c4d6ec;'> " + " <tbody> " + " <tr> " + " <td> " + " <table class='row-content stack' align='center' border='0' cellpadding='0' cellspacing='0' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt; color: #000000; width: 650px;' width='650'> " + " <tbody> " + " <tr> " + " <td class='column column-1' width='100%' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt; font-weight: 400; text-align: left; vertical-align: top; padding-top: 20px; padding-bottom: 20px; border-top: 0px; border-right: 0px; border-bottom: 0px; border-left: 0px;'> " + " <table class='text_block block-1' width='100%' border='0' cellpadding='10' cellspacing='0' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt; word-break: break-word;'> " + " <tr> " + " <td class='pad'> " + " <div style='font-family: sans-serif'> " + " <div class style='font-size: 14px; font-family: Roboto Slab, Arial, Helvetica Neue, Helvetica, sans-serif; mso-line-height-alt: 16.8px; color: #3f4d75; line-height: 1.2;'> " + " <p style='margin: 0; font-size: 14px; text-align: center; mso-line-height-alt: 16.8px;'><span style='font-size:12px;'>This email was originated from the NCPAC automated mailing system</span></p> " + " </div> " + " </div> " + " </td> " + " </tr> " + " </table> " + " </td> " + " </tr> " + " </tbody> " + " </table> " + " </td> " + " </tr> " + " </tbody> " + " </table> " + " <table class='row row-4' align='center' width='100%' border='0' cellpadding='0' cellspacing='0' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt; background-color: #f3f6fe;'> " + " <tbody> " + " <tr> " + " <td> " + " <table class='row-content stack' align='center' border='0' cellpadding='0' cellspacing='0' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt; color: #000000; width: 650px;' width='650'> " + " <tbody> " + " <tr> " + " <td class='column column-1' width='100%' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt; font-weight: 400; text-align: left; vertical-align: top; padding-top: 0px; padding-bottom: 0px; border-top: 0px; border-right: 0px; border-bottom: 0px; border-left: 0px;'> " + " <div class='spacer_block' style='height:70px;line-height:30px;font-size:1px;'> </div> " + " </td> " + " </tr> " + " </tbody> " + " </table> " + " </td> " + " </tr> " + " </tbody> " + " </table> " + " <table class='row row-5' align='center' width='100%' border='0' cellpadding='0' cellspacing='0' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt;'> " + " <tbody> " + " <tr> " + " <td> " + " <table class='row-content stack' align='center' border='0' cellpadding='0' cellspacing='0' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt; color: #000000; width: 650px;' width='650'> " + " <tbody> " + " <tr> " + " <td class='column column-1' width='100%' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt; font-weight: 400; text-align: left; vertical-align: top; padding-top: 5px; padding-bottom: 5px; border-top: 0px; border-right: 0px; border-bottom: 0px; border-left: 0px;'> " + " <table class='icons_block block-1' width='100%' border='0' cellpadding='0' cellspacing='0' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt;'> " + " <tr> " + " <td class='pad' style='vertical-align: middle; color: #9d9d9d; font-family: inherit; font-size: 15px; padding-bottom: 5px; padding-top: 5px; text-align: center;'> " + " <table width='100%' cellpadding='0' cellspacing='0' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt;'> " + " <tr> " + " <td class='alignment' style='vertical-align: middle; text-align: center;'> " + "  " + "  " + " </td> " + " </tr> " + " </table> " + " </td> " + " </tr> " + " </table> " + " </td> " + " </tr> " + " </tbody> " + " </table> " + " </td> " + " </tr> " + " </tbody> " + " </table> " + " </td> " + " </tr> " + " </tbody> " + " </table> " + "</body> " + "</html>";
            if(EmailAddress.Contains("niagaracollege.ca"))
            {
                message = "<div><div class='R1UVb' style='height: 170px; width: 100%;'><div class='qF8_5'><button type='button' class='ms-Button ms-Button--icon wD8TJ root-573' title='Show original size' aria-label='Show original size' data-is-focusable='true'><span class='ms-Button-flexContainer flexContainer-164' data-automationid='splitbuttonprimary'><i data-icon-name='FullScreen' aria-hidden='true' class='ms-Icon root-90 css-308 ms-Button-icon icon-284' style='font-family: controlIcons;'></i></span></button></div><table border='0' width='100%' height='100%' cellpadding='0' cellspacing='0' bgcolor='rgb(51, 51, 51)' style='border-spacing: 0px; font-family: Helvetica, Arial, sans-serif; background-color: rgb(51, 51, 51) !important; transform: scale(0.425, 0.425); transform-origin: left top;' data-ogsb='' data-ogab='#FFFFFF' min-scale='0.425'><tbody><tr><td align='center' valign='top' bgcolor='rgb(51, 51, 51)' style='background-color: rgb(51, 51, 51) !important; border-collapse: collapse;' data-ogsb='rgb(255, 255, 255)' data-ogab='#FFFFFF'><table width='600' cellpadding='0' cellspacing='0' border='0' class='x_container' bgcolor='rgb(51, 51, 51)' style='border-spacing: 0px; font-family: Helvetica, Arial, sans-serif; color: rgb(182, 182, 182) !important; max-width: 600px; background-color: rgb(51, 51, 51) !important;' data-ogsc='rgb(107, 107, 107)' data-ogsb='' data-ogab='#FFFFFF'><tbody><tr><td class='x_logo' style='border-collapse:collapse; text-align:center; padding-top:20px; padding-bottom:20px; padding-right:0; padding-left:0'><a href='https://www.niagaracollege.ca/' target='_blank' rel='noopener noreferrer' data-auth='NotApplicable' style='text-decoration: none; color: rgb(255, 149, 123) !important;' data-safelink='true' data-ogsc='' data-linkindex='0'><img data-imagetype='External' blockedimagesrc='https://upload.wikimedia.org/wikipedia/commons/thumb/1/12/Niagara-college_vectorized.svg/1200px-Niagara-college_vectorized.svg.png' width='90' height='30' border='0' alt='Niagara College' style='width:100%; max-width:90px; border-width:0'> </a></td></tr><tr><td class='x_container-padding x_content' bgcolor='rgb(51, 51, 51)' style='background-color: rgb(51, 51, 51) !important; padding-top: 20px; padding-bottom: 20px; border-collapse: collapse;' data-ogsb='rgb(255, 255, 255)' data-ogab='#FFFFFF'><h2 data-ogsc='' style='color: rgb(234, 234, 234) !important;'>Hi, Your NCPAC Member Account has been created.</h2><p data-ogsc='' style='color: rgb(182, 182, 182) !important;'>Please click on the button below and go through our reset password option inorder to create your new password. Then you will be able to login to the your PAC account. Do not forget to use " + EmailAddress + " as your email in Login and password reset</p><br aria-hidden='true'><p class='x_purple-cta-btn' style='font-size: 16px; line-height: 1.5; margin: 0px 0px 25px; color: rgb(182, 182, 182) !important;' data-ogsc='rgb(107, 107, 107)'><a href='https://ncpacproto2.azurewebsites.net/Identity/Account/ForgotPassword' target='_blank' rel='noopener noreferrer' data-auth='NotApplicable' class='x_button-purple' data-reset-password-link='https://ncpacproto2.azurewebsites.net/Identity/Account/ForgotPassword' style='background-color: rgb(51, 51, 51) !important; border-radius: 30px; border-width: 1px; border-style: solid; border-color: rgb(68, 31, 75); color: rgb(255, 209, 255) !important; display: inline-block; font-size: 16px; font-weight: bold; line-height: 20px; min-height: 20px; padding: 10px 30px; text-decoration: none !important;' data-safelink='true' data-ogsc='rgb(68, 31, 75)' data-ogsb='rgb(255, 255, 255)' data-linkindex='1'>Create Password </a></p></td></tr></tbody></table></td></tr></tbody></table></div><div class='R1UVb' style='height: 40px; width: 100%;'><div class='qF8_5'><button type='button' class='ms-Button ms-Button--icon wD8TJ root-573' title='Show original size' aria-label='Show original size' data-is-focusable='true'><span class='ms-Button-flexContainer flexContainer-164' data-automationid='splitbuttonprimary'><i data-icon-name='FullScreen' aria-hidden='true' class='ms-Icon root-90 css-308 ms-Button-icon icon-284' style='font-family: controlIcons;'></i></span></button></div><table border='0' width='100%' height='100%' cellpadding='0' cellspacing='0' bgcolor='rgb(51, 51, 51)' style='border-spacing: 0px; font-family: Helvetica, Arial, sans-serif; background-color: rgb(51, 51, 51) !important; transform: scale(0.425, 0.425); transform-origin: left top;' data-ogsb='' data-ogab='#FFFFFF' min-scale='0.425'><tbody><tr><td align='center' valign='top' bgcolor='rgb(51, 51, 51)' style='background-color: rgb(51, 51, 51) !important; border-collapse: collapse;' data-ogsb='rgb(255, 255, 255)' data-ogab='#FFFFFF'><table width='600' cellpadding='0' cellspacing='0' border='0' class='x_container' bgcolor='rgb(51, 51, 51)' style='border-spacing: 0px; font-family: Helvetica, Arial, sans-serif; color: rgb(182, 182, 182) !important; max-width: 600px; background-color: rgb(51, 51, 51) !important;' data-ogsc='rgb(107, 107, 107)' data-ogsb='' data-ogab='#FFFFFF'><tbody><tr><td style='border-collapse:collapse'><table cellspacing='0' border='0' cellpadding='0' align='center' width='600' bgcolor='rgb(51, 51, 51)' class='x_footer' style='border-spacing: 0px; font-family: Helvetica, Arial, sans-serif; width: 100%; background-color: rgb(51, 51, 51) !important;' data-ogsb='rgb(255, 255, 255)' data-ogab='#FFFFFF'><tbody><tr><td class='x_social' align='center' style='border-collapse: collapse; vertical-align: middle; text-align: center; color: rgb(209, 209, 209) !important; font-size: 12px; padding: 20px 20px 10px;' data-ogsc='rgb(67, 67, 67)'><table class='x_intercom-container x_intercom-align-center' align='center' style='border-spacing:0; font-family:Helvetica,Arial,sans-serif'><tbody><tr><td style='border-collapse: collapse; vertical-align: middle; text-align: center; color: rgb(209, 209, 209) !important; font-size: 12px; padding-left: 4px; padding-right: 4px;' data-ogsc='rgb(67, 67, 67)'><a href='' target='_blank' rel='noopener noreferrer' data-auth='NotApplicable' style='color: rgb(255, 149, 123) !important; display: inline-block; text-decoration: none;' data-safelink='true' data-ogsc='rgb(231, 102, 80)' data-linkindex='2'><img data-imagetype='External' blockedimagesrc='' width='21' border='0' style='width:100%; max-width:21px; border-width:0; vertical-align:middle'> </a></td><td style='border-collapse: collapse; vertical-align: middle; text-align: center; color: rgb(209, 209, 209) !important; font-size: 12px; padding-left: 4px; padding-right: 4px;' data-ogsc='rgb(67, 67, 67)'><a href='' target='_blank' rel='noopener noreferrer' data-auth='NotApplicable' style='color: rgb(255, 149, 123) !important; display: inline-block; text-decoration: none;' data-safelink='true' data-ogsc='rgb(231, 102, 80)' data-linkindex='3'><img data-imagetype='External' blockedimagesrc='' width='21' border='0' style='width:100%; max-width:21px; border-width:0; vertical-align:middle'> </a></td><td style='border-collapse: collapse; vertical-align: middle; text-align: center; color: rgb(209, 209, 209) !important; font-size: 12px; padding-left: 4px; padding-right: 4px;' data-ogsc='rgb(67, 67, 67)'><a href='' target='_blank' rel='noopener noreferrer' data-auth='NotApplicable' style='color: rgb(255, 149, 123) !important; display: inline-block; text-decoration: none;' data-safelink='true' data-ogsc='rgb(231, 102, 80)' data-linkindex='4'><img data-imagetype='External' blockedimagesrc='https://getmaple.ca/email/youtube-2x.png' width='21' border='0' style='width:100%; max-width:21px; border-width:0; vertical-align:middle'> </a></td><td style='border-collapse: collapse; vertical-align: middle; text-align: center; color: rgb(209, 209, 209) !important; font-size: 12px; padding-left: 4px; padding-right: 4px;' data-ogsc='rgb(67, 67, 67)'><a href='' target='_blank' rel='noopener noreferrer' data-auth='NotApplicable' style='color: rgb(255, 149, 123) !important; display: inline-block; text-decoration: none;' data-safelink='true' data-ogsc='rgb(231, 102, 80)' data-linkindex='5'><img data-imagetype='External' blockedimagesrc='' width='21' border='0' style='width:100%; max-width:21px; border-width:0; vertical-align:middle'> </a></td><td style='border-collapse: collapse; vertical-align: middle; text-align: center; color: rgb(209, 209, 209) !important; font-size: 12px; padding-left: 4px; padding-right: 4px;' data-ogsc='rgb(67, 67, 67)'><a href='' target='_blank' rel='noopener noreferrer' data-auth='NotApplicable' style='color: rgb(255, 149, 123) !important; display: inline-block; text-decoration: none;' data-safelink='true' data-ogsc='rgb(231, 102, 80)' data-linkindex='6'><img data-imagetype='External' blockedimagesrc='' width='21' border='0' style='width:100%; max-width:21px; border-width:0; vertical-align:middle'> </a></td></tr></tbody></table></td></tr><tr><td class='x_info' style='border-collapse: collapse; vertical-align: middle; text-align: center; color: rgb(182, 182, 182) !important; font-size: 10px; padding: 0px 20px 20px;' data-ogsc='rgb(107, 107, 107)'><br aria-hidden='true'></td></tr></tbody></table></td></tr></tbody></table></td></tr></tbody></table></div><img data-imagetype='External' blockedimagesrc='' alt='' width='1' height='1' border='0' style='height:1px!important; width:1px!important; border-width:0!important; margin-top:0!important; margin-bottom:0!important; margin-right:0!important; margin-left:0!important; padding-top:0!important; padding-bottom:0!important; padding-right:0!important; padding-left:0!important'> </div>";
            }
            try
            {
                await _emailSender.SendOneAsync(member.FirstName + " " + member.LastName, EmailAddress,
                "Account Registration", message);
                TempData["message"] = "Invitation email sent to " + member.FirstName + " " + member.LastName + " at " + EmailAddress;
            }
            catch (Exception)
            {
                TempData["message"] = "Could not send Invitation email to " + member.FirstName + " " + member.LastName + " at " + EmailAddress;
            }


        }

        private void UpdateMemberCommitees(string[] selectedOptions, Member memberToUpdate)
        {
            if (selectedOptions == null)
            {
                memberToUpdate.MemberCommitees = new List<MemberCommitee>();
                return;
            }

            var selectedOptionsHS = new HashSet<string>(selectedOptions);
            var currentOptionsHS = new HashSet<int>(memberToUpdate.MemberCommitees.Select(b => b.CommiteeID));
            foreach (var s in _context.Commitees)
            {
                if (selectedOptionsHS.Contains(s.ID.ToString()))//it is selected
                {
                    if (!currentOptionsHS.Contains(s.ID))//but not currently in the Member's collection - Add it!
                    {
                        memberToUpdate.MemberCommitees.Add(new MemberCommitee
                        {
                            CommiteeID = s.ID,
                            MemberID = memberToUpdate.ID
                        });
                    }
                }
                else //not selected
                {
                    if (currentOptionsHS.Contains(s.ID))//but is currently in the Member's collection - Remove it!
                    {
                        MemberCommitee specToRemove = memberToUpdate.MemberCommitees.FirstOrDefault(d => d.CommiteeID == s.ID);
                        _context.Remove(specToRemove);
                    }
                }
            }
        }

        private SelectList ProvinceSelectList(string selectedId)
        {
            return new SelectList(_context.Provinces
                .OrderByDescending(i => i.ID == "NA").ThenBy(p => p.Name), "ID", "Name", selectedId);
        }

        private SelectList WorkProvinceSelectList(string selectedId)
        {
            return new SelectList(_context.Provinces
                .OrderByDescending(i => i.ID == "NA").ThenBy(p => p.Name), "ID", "Name", selectedId);
        }

        private SelectList MailPrefferenceSelectList(string selectedId)
        {
            return new SelectList(_context.MailPrefferences
                .OrderBy(d => d.Name), "ID", "Name", selectedId);
        }

        private void PopulateDropDownLists(MemberAdminVM member = null)
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
