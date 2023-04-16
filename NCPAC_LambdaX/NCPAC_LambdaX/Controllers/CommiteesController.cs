using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NCPAC_LambdaX.Data;
using NCPAC_LambdaX.Models;
using NCPAC_LambdaX.Utilities;
using OfficeOpenXml.Style;
using OfficeOpenXml;

namespace NCPAC_LambdaX.Controllers
{
    [Authorize]
    public class CommiteesController : Controller
    {
        private readonly NCPACContext _context;

        public CommiteesController(NCPACContext context)
        {
            _context = context;
        }

        // GET: Commitees
        
        public async Task<IActionResult> Index(string SearchCommitee, string SearchDivision,int? page, int? pageSizeID, string actionButton, string sortDirection = "asc", string sortField = "Commitee")
        {
            ViewData["Filtering"] = "";
            var members = _context.Members
                .ToList();
            ViewData["ToRenew"] = members.Where(m => m.RenewalDate <= DateTime.Now).Count().ToString();

            //List of sort options.
            //NOTE: make sure this array has matching values to the column headings
            string[] sortOptions = new[] { "Commitee","Division" };

            var commitees = _context.Commitees
              .AsNoTracking();

            if (!String.IsNullOrEmpty(SearchCommitee))
            {
                commitees = commitees.Where(p => p.CommiteeName.ToUpper().Contains(SearchCommitee.ToUpper()));
                ViewData["Filtering"] = "show";
            }
            if (!String.IsNullOrEmpty(SearchDivision))
            {
                commitees = commitees.Where(p => p.Division.ToUpper().Contains(SearchDivision.ToUpper()));
                ViewData["Filtering"] = "show";
            }
            //Before we sort, see if we have called for a change of filtering or sorting
            if (!String.IsNullOrEmpty(actionButton)) //Form Submitted!
            {
                page = 1;//Reset page to start

                if (sortOptions.Contains(actionButton))//Change of sort is requested
                {
                    if (actionButton == sortField) //Reverse order on same field
                    {
                        sortDirection = sortDirection == "asc" ? "desc" : "asc";
                    }
                    sortField = actionButton;//Sort by the button clicked
                }
            }
            //Now we know which field and direction to sort by
            if (sortField == "Commitee")
            {
                if (sortDirection == "asc")
                {
                    commitees = commitees
                        .OrderByDescending(p => p.CommiteeName);
                }
                else
                {
                    commitees = commitees
                        .OrderBy(p => p.CommiteeName);
                }
            }
            else if (sortField == "Division")
            {
                if (sortDirection == "asc")
                {
                    commitees = commitees
                        .OrderByDescending(p => p.Division);
                }
                else
                {
                    commitees = commitees
                        .OrderBy(p => p.Division);
                }
            }

            ViewData["sortField"] = sortField;
            ViewData["sortDirection"] = sortDirection;

            ViewBag.sortFieldID = new SelectList(sortOptions, sortField.ToString());

            //Handle Paging
            int pageSize = PageSizeHelper.SetPageSize(HttpContext, pageSizeID, "commitees");
            ViewData["pageSizeID"] = PageSizeHelper.PageSizeList(pageSize);
            var pagedData = await PaginatedList<Commitee>.CreateAsync(commitees.AsNoTracking(), page ?? 1, pageSize);

            return View(pagedData);
        }

        // GET: Commitees/Details/5
        [Authorize(Roles = "Admin,Supervisor")]
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
        [Authorize(Roles = "Admin,Supervisor")]
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
        [Authorize(Roles = "Admin,Supervisor")]
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
        [Authorize(Roles = "Admin,Supervisor")]
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
        [Authorize(Roles = "Admin,Supervisor")]
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
        [Authorize(Roles = "Admin,Supervisor")]
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

        [Authorize(Roles = "Admin,Supervisor")]
        public IActionResult DownloadMembers()
        {
            //Get the appointments
            var membs = from e in _context.Members
                        .Include(a => a.MemberCommitees).ThenInclude(m => m.Commitee)
                        orderby e.LastName 
                        select new
                        {
                            ID = e.ID,
                            Salutation = e.Salutation,
                            FirstName = e.FirstName,
                            MiddleName = e.MiddleName,
                            LastName = e.LastName,
                            DateJoined = e.DateJoined,
                            IsActive = e.IsActive,
                            StreetAddress = e.StreetAddress,
                            City = e.City,
                            Province = e.Province.Name,
                            PostalCode = e.PostalCode,
                            Phone = e.Phone,
                            Email = e.Email,
                            WorkStreetAddress = e.WorkStreetAddress,
                            WorkCity = e.WorkCity,
                            WorkProvince = e.WorkProvince.Name,
                            WorkPostalCode = e.WorkPostalCode,
                            WorkPhone = e.WorkPhone,
                            WorkEmail = e.WorkEmail,
                            MailPrefference = e.MailPrefference.Name,
                            EducationalSummary = e.EducationalSummary,
                            IsNCGrad = e.IsNCGrad,
                            OccupationalSummary = e.OccupationalSummary,
                            MemberCommitees = String.Join(", ", e.MemberCommitees.Select(a => a.Commitee.CommiteeName))
                        };

            //How many rows?
            int numRows = membs.Count();

            if (numRows > 0) //We have data
            {
                //Create a new spreadsheet from scratch.
                using (ExcelPackage excel = new ExcelPackage())
                {

                    //Note: you can also pull a spreadsheet out of the database if you
                    //have saved it in the normal way we do, as a Byte Array in a Model
                    //such as the UploadedFile class.
                    //
                    // Suppose...
                    //
                    // var theSpreadsheet = _context.UploadedFiles.Include(f => f.FileContent).Where(f => f.ID == id).SingleOrDefault();
                    //
                    //    //Pass the Byte[] FileContent to a MemoryStream
                    //
                    // using (MemoryStream memStream = new MemoryStream(theSpreadsheet.FileContent.Content))
                    // {
                    //     ExcelPackage package = new ExcelPackage(memStream);
                    // }

                    var workSheet = excel.Workbook.Worksheets.Add("Members");

                    //Note: Cells[row, column]
                    workSheet.Cells[3, 1].LoadFromCollection(membs, true);


                    //Note: You can define a BLOCK of cells: Cells[startRow, startColumn, endRow, endColumn]
                    //Make Date and Patient Bold
                    workSheet.Cells[4, 1, numRows + 3, 2].Style.Font.Bold = true;

                    //Note: these are fine if you are only 'doing' one thing to the range of cells.
                    //Otherwise you should USE a range object for efficiency
                    workSheet.Cells[numRows + 5, 4].Value = membs.Count().ToString();
                    workSheet.Cells[numRows + 5, 3].Value = "Member Count:";
                    //Set Style and backgound colour of headings
                    using (ExcelRange headings = workSheet.Cells[3, 1, 3, 24])
                    {
                        headings.Style.Font.Bold = true;
                        var fill = headings.Style.Fill;
                        fill.PatternType = ExcelFillStyle.Solid;
                        fill.BackgroundColor.SetColor(Color.LightBlue);
                    }

                    //Boy those notes are BIG!
                    //Lets put them in comments instead.
                    for (int i = 4; i < numRows + 4; i++)
                    {
                        using (ExcelRange Rng = workSheet.Cells[i, 7])
                        {
                            string[] commentWords = Rng.Value.ToString().Split(' ');
                            Rng.Value = commentWords[0] + "...";
                            //This LINQ adds a newline every 7 words
                            string comment = string.Join(Environment.NewLine, commentWords
                                .Select((word, index) => new { word, index })
                                .GroupBy(x => x.index / 7)
                                .Select(grp => string.Join(" ", grp.Select(x => x.word))));
                            ExcelComment cmd = Rng.AddComment(comment, "Member Notes");
                            cmd.AutoFit = true;
                        }
                    }

                    //Autofit columns
                    workSheet.Cells.AutoFitColumns();
                    //Note: You can manually set width of columns as well
                    //workSheet.Column(7).Width = 10;

                    //Add a title and timestamp at the top of the report
                    workSheet.Cells[1, 1].Value = "Members and Commitees Report";
                    using (ExcelRange Rng = workSheet.Cells[1, 1, 1, 24])
                    {
                        Rng.Merge = true; //Merge columns start and end range
                        Rng.Style.Font.Bold = true; //Font should be bold
                        Rng.Style.Font.Size = 18;
                        Rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    }
                    //Since the time zone where the server is running can be different, adjust to 
                    //Local for us.
                    DateTime utcDate = DateTime.UtcNow;
                    TimeZoneInfo esTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
                    DateTime localDate = TimeZoneInfo.ConvertTimeFromUtc(utcDate, esTimeZone);
                    using (ExcelRange Rng = workSheet.Cells[2, 6])
                    {
                        Rng.Value = "Created: " + localDate.ToShortTimeString() + " on " +
                            localDate.ToShortDateString();
                        Rng.Style.Font.Bold = true; //Font should be bold
                        Rng.Style.Font.Size = 12;
                        Rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    }

                    //Ok, time to download the Excel

                    try
                    {
                        Byte[] theData = excel.GetAsByteArray();
                        string filename = "Members.xlsx";
                        string mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        return File(theData, mimeType, filename);
                    }
                    catch (Exception)
                    {
                        return BadRequest("Could not build and download the file.");
                    }
                }
            }
            return NotFound("No data.");
        }

        private bool CommiteeExists(int id)
        {
          return _context.Commitees.Any(e => e.ID == id);
        }
    }
}
