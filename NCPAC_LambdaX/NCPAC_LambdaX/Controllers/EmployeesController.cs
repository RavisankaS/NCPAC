using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NCPAC_LambdaX.Data;
using NCPAC_LambdaX.Models;
using NCPAC_LambdaX.Utilities;
using NCPAC_LambdaX.ViewModels;

namespace NCPAC_LambdaX.Controllers
{
    [Authorize(Roles = "Admin")]
    public class EmployeesController : Controller
    {
        private readonly NCPACContext _context;
        private readonly ApplicationDbContext _identityContext;
        private readonly IMyEmailSender _emailSender;
        private readonly UserManager<IdentityUser> _userManager;

        public EmployeesController(NCPACContext context,
            ApplicationDbContext identityContext, IMyEmailSender emailSender,
            UserManager<IdentityUser> userManager)
        {
            _context = context;
            _identityContext = identityContext;
            _emailSender = emailSender;
            _userManager = userManager;
        }

        // GET: Employees
        public async Task<IActionResult> Index()
        {
            var employees = await _context.Employees
                .Select(e => new EmployeeAdminVM
                {
                    Email = e.Email,
                    Active = e.Active,
                    ID = e.ID,
                    FirstName = e.FirstName,
                    LastName = e.LastName,
                    Phone = e.Phone
                }).ToListAsync();

            foreach (var e in employees)
            {
                var user = await _userManager.FindByEmailAsync(e.Email);
                if (user != null)
                {
                    e.UserRoles = (List<string>)await _userManager.GetRolesAsync(user);
                }
            };

            return View(employees);
        }

        // GET: Employee/Create
        public IActionResult Create()
        {
            EmployeeAdminVM employee = new EmployeeAdminVM();
            PopulateAssignedRoleData(employee);
            return View(employee);
        }

        // POST: Employee/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FirstName,LastName,Phone," +
            "Email")] Employee employee, string[] selectedRoles)
        {

            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(employee);
                    await _context.SaveChangesAsync();

                    InsertIdentityUser(employee.Email, selectedRoles);

                    //Send Email to new Employee - commented out till email configured
                    await InviteUserToResetPassword(employee, null);

                    return RedirectToAction(nameof(Index));
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
            EmployeeAdminVM employeeAdminVM = new EmployeeAdminVM
            {
                Email = employee.Email,
                Active = employee.Active,
                ID = employee.ID,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Phone = employee.Phone
            };
            foreach (var role in selectedRoles)
            {
                employeeAdminVM.UserRoles.Add(role);
            }
            PopulateAssignedRoleData(employeeAdminVM);
            return View(employeeAdminVM);
        }

        // GET: Employees/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .Where(e => e.ID == id)
                .Select(e => new EmployeeAdminVM
                {
                    Email = e.Email,
                    Active = e.Active,
                    ID = e.ID,
                    FirstName = e.FirstName,
                    LastName = e.LastName,
                    Phone = e.Phone
                }).FirstOrDefaultAsync();

            if (employee == null)
            {
                return NotFound();
            }

            //Get the user from the Identity system
            var user = await _userManager.FindByEmailAsync(employee.Email);
            if (user != null)
            {
                //Add the current roles
                var r = await _userManager.GetRolesAsync(user);
                employee.UserRoles = (List<string>)r;
            }
            PopulateAssignedRoleData(employee);

            return View(employee);
        }

        // POST: Employees/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, bool Active, string[] selectedRoles)
        {
            var employeeToUpdate = await _context.Employees
                .FirstOrDefaultAsync(m => m.ID == id);
            if (employeeToUpdate == null)
            {
                return NotFound();
            }

            //Note the current Email and Active Status
            bool ActiveStatus = employeeToUpdate.Active;
            string databaseEmail = employeeToUpdate.Email;


            if (await TryUpdateModelAsync<Employee>(employeeToUpdate, "",
                e => e.FirstName, e => e.LastName, e => e.Phone, e => e.Email, e => e.Active))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    //Save successful so go on to related changes

                    //Check for changes in the Active state
                    if (employeeToUpdate.Active == false && ActiveStatus == true)
                    {
                        //Deactivating them so delete the IdentityUser
                        //This deletes the user's login from the security system
                        await DeleteIdentityUser(employeeToUpdate.Email);

                    }
                    else if (employeeToUpdate.Active == true && ActiveStatus == false)
                    {
                        //You reactivating the user, create them and
                        //give them the selected roles
                        InsertIdentityUser(employeeToUpdate.Email, selectedRoles);
                    }
                    else if (employeeToUpdate.Active == true && ActiveStatus == true)
                    {
                        //No change to Active status so check for a change in Email
                        //If you Changed the email, Delete the old login and create a new one
                        //with the selected roles
                        if (employeeToUpdate.Email != databaseEmail)
                        {
                            //Add the new login with the selected roles
                            InsertIdentityUser(employeeToUpdate.Email, selectedRoles);

                            //This deletes the user's old login from the security system
                            await DeleteIdentityUser(databaseEmail);
                        }
                        else
                        {
                            //Finially, Still Active and no change to Email so just Update
                            await UpdateUserRoles(selectedRoles, employeeToUpdate.Email);
                        }
                    }

                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeExists(employeeToUpdate.ID))
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
            }
            //We are here because something went wrong and need to redisplay
            EmployeeAdminVM employeeAdminVM = new EmployeeAdminVM
            {
                Email = employeeToUpdate.Email,
                Active = employeeToUpdate.Active,
                ID = employeeToUpdate.ID,
                FirstName = employeeToUpdate.FirstName,
                LastName = employeeToUpdate.LastName,
                Phone = employeeToUpdate.Phone
            };
            foreach (var role in selectedRoles)
            {
                employeeAdminVM.UserRoles.Add(role);
            }
            PopulateAssignedRoleData(employeeAdminVM);
            return View(employeeAdminVM);
        }

        private void PopulateAssignedRoleData(EmployeeAdminVM employee)
        {//Prepare checkboxes for all Roles
            var allRoles = _identityContext.Roles;
            var currentRoles = employee.UserRoles;
            var viewModel = new List<RoleVM>();
            foreach (var r in allRoles)
            {
                viewModel.Add(new RoleVM
                {
                    RoleId = r.Id,
                    RoleName = r.Name,
                    Assigned = currentRoles.Contains(r.Name)
                });
            }
            ViewBag.Roles = viewModel;
        }

        private async Task UpdateUserRoles(string[] selectedRoles, string Email)
        {
            var _user = await _userManager.FindByEmailAsync(Email);//IdentityUser
            if (_user != null)
            {
                var UserRoles = (List<string>)await _userManager.GetRolesAsync(_user);//Current roles user is in

                if (selectedRoles == null)
                {
                    //No roles selected so just remove any currently assigned
                    foreach (var r in UserRoles)
                    {
                        await _userManager.RemoveFromRoleAsync(_user, r);
                    }
                }
                else
                {
                    //At least one role checked so loop through all the roles
                    //and add or remove as required

                    //We need to do this next line because foreach loops don't always work well
                    //for data returned by EF when working async.  Pulling it into an IList<>
                    //first means we can safely loop over the colleciton making async calls and avoid
                    //the error 'New transaction is not allowed because there are other threads running in the session'
                    IList<IdentityRole> allRoles = _identityContext.Roles.ToList<IdentityRole>();

                    foreach (var r in allRoles)
                    {
                        if (selectedRoles.Contains(r.Name))
                        {
                            if (!UserRoles.Contains(r.Name))
                            {
                                await _userManager.AddToRoleAsync(_user, r.Name);
                            }
                        }
                        else
                        {
                            if (UserRoles.Contains(r.Name))
                            {
                                await _userManager.RemoveFromRoleAsync(_user, r.Name);
                            }
                        }
                    }
                }
            }
        }

        private void InsertIdentityUser(string Email, string[] selectedRoles)
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
                IdentityResult result = _userManager.CreateAsync(user, password).Result;

                if (result.Succeeded)
                {
                    foreach (string role in selectedRoles)
                    {
                        _userManager.AddToRoleAsync(user, role).Wait();
                    }
                }
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

        private async Task InviteUserToResetPassword(Employee employee, string message)
        {
            message ??= "Hello " + employee.FirstName + "<br /><p>Please navigate to:<br />" +
                        "<a href='https://theapp.azurewebsites.net/' title='https://theapp.azurewebsites.net/' target='_blank' rel='noopener'>" +
                        "https://theapp.azurewebsites.net</a><br />" +
                        " and create a new password for " + employee.Email + " using Forgot Password.</p>";
            try
            {
                await _emailSender.SendOneAsync(employee.FullName, employee.Email,
                "Account Registration", message);
                TempData["message"] = "Invitation email sent to " + employee.FullName + " at " + employee.Email;
            }
            catch (Exception)
            {
                TempData["message"] = "Could not send Invitation email to " + employee.FullName + " at " + employee.Email;
            }


        }

        private bool EmployeeExists(int id)
        {
            return _context.Employees.Any(e => e.ID == id);
        }
    }
}
