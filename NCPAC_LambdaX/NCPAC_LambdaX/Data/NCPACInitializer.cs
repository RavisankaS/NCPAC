using Microsoft.EntityFrameworkCore;
using NCPAC_LambdaX.Models;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Linq;

namespace NCPAC_LambdaX.Data
{
    public class NCPACInitializer
    {
        public static void Seed(IApplicationBuilder applicationBuilder)
        {
            NCPACContext context = applicationBuilder.ApplicationServices.CreateScope()
                .ServiceProvider.GetRequiredService<NCPACContext>();

            try
            {
                //Delete the database if you need to apply a new Migration
                context.Database.EnsureDeleted(); //Take time to add this to all other Initializer.Seed() code
                //Create the database if it does not exist and apply your Migration
                context.Database.Migrate();

                //This approach to seeding data uses int and string arrays with loops to
                //create the data using random values
                Random random = new Random();

                // Look for any Employees.  Seed ones to match the seeded Identity accounts.
                if (!context.Employees.Any())
                {
                    context.Employees.AddRange(
                     new Employee
                     {
                         FirstName = "Kait",
                         LastName = "Ulisse",
                         Email = "admin@outlook.com",
                         Phone="6478574343"
                     },
                     new Employee
                     {
                         FirstName = "Patricia",
                         LastName = "Beu",
                         Email = "supervisor@outlook.com",
                         Phone = "6478574344"
                     },
                     new Employee
                     {
                         FirstName = "Banuka",
                         LastName = "Ambegoda",
                         Email = "banu@outlook.com",
                         Phone = "6478574345"
                     });

                    context.SaveChanges();
                }

                if (!context.Announcements.Any())
                {
                    context.Announcements.AddRange(
                    new Announcement
                    {
                        Subject = "Two factor authentication",
                        AnnouncementDescription = "All the members who have not yet enrolled with 2 factor authentication please do that now. Follow this link https://its.niagaracollege.ca/studentmfa/ for futher steps in enrolling in mfa. Please follow these instructions and setup 2 factor authentication ASAP to ensure the safety of your account.",
                        TimePosted = DateTime.Now
                    },
                    new Announcement
                    {
                        Subject = "Welcome to the NCPAC management system",
                        AnnouncementDescription = "This is the new application that our organization use to maintain your information as well as meetings, action items and etc. Make your selves comfortable and please do not forget to complete all your information in your profiles.",
                        TimePosted = DateTime.Now
                    }

                    ); 
                    context.SaveChanges();
                }


                // Seed Commitees if there arent any.
                if (!context.Commitees.Any())
                {
                    context.Commitees.AddRange(
                    new Commitee
                    {
                        CommiteeName = "Technology and Industrial Automation",
                        Division = "Trades, Media & Technology"
                    },
                    new Commitee
                    {
                        CommiteeName = "Mechanical Engineering",
                        Division = "Trades, Media & Technology"
                    },
                    new Commitee
                    {
                        CommiteeName = "Renewable Energies",
                        Division = "Trades, Media & Technology"
                    },
                    new Commitee
                    {
                        CommiteeName = "Computer, Electrical & Electronics Engineering",
                        Division = "Trades, Media & Technology"
                    },
                    new Commitee
                    {
                        CommiteeName = "Computer Programming",
                        Division = "Trades, Media & Technology"
                    },
                    new Commitee
                    {
                        CommiteeName = "Game Developement",
                        Division = "Trades, Media & Technology"
                    },
                    new Commitee
                    {
                        CommiteeName = "Graphic Design",
                        Division = "Trades, Media & Technology"
                    },
                    new Commitee
                    {
                        CommiteeName = "Public Relations",
                        Division = "Trades, Media & Technology"
                    },
                    new Commitee
                    {
                        CommiteeName = "Social Media Management",
                        Division = "Trades, Media & Technology"
                    },
                    new Commitee
                    {
                        CommiteeName = "Hair Styling",
                        Division = "Trades, Media & Technology"
                    },
                    new Commitee
                    {
                        CommiteeName = "Motive Power",
                        Division = "Trades, Media & Technology"
                    },
                    new Commitee
                    {
                        CommiteeName = "Carpentry and Rennovation",
                        Division = "Trades, Media & Technology"
                    },
                    new Commitee
                    {
                        CommiteeName = "Mechanical Techniques",
                        Division = "Trades, Media & Technology"
                    },
                    new Commitee
                    {
                        CommiteeName = "Welding Skills",
                        Division = "Trades, Media & Technology"
                    },
                    new Commitee
                    {
                        CommiteeName = "Electrical Technician",
                        Division = "Trades, Media & Technology"
                    }

                    );
                    context.SaveChanges();
                }


                if (!context.Provinces.Any())
                {
                    var provinces = new List<Province>
                    {
                        new Province { ID = "NA", Name = "None"},
                        new Province { ID = "ON", Name = "Ontario"},
                        new Province { ID = "PE", Name = "Prince Edward Island"},
                        new Province { ID = "NB", Name = "New Brunswick"},
                        new Province { ID = "BC", Name = "British Columbia"},
                        new Province { ID = "NL", Name = "Newfoundland and Labrador"},
                        new Province { ID = "SK", Name = "Saskatchewan"},
                        new Province { ID = "NS", Name = "Nova Scotia"},
                        new Province { ID = "MB", Name = "Manitoba"},
                        new Province { ID = "QC", Name = "Quebec"},
                        new Province { ID = "YT", Name = "Yukon"},
                        new Province { ID = "NU", Name = "Nunavut"},
                        new Province { ID = "NT", Name = "Northwest Territories"},
                        new Province { ID = "AB", Name = "Alberta"}
                    };
                    context.Provinces.AddRange(provinces);
                    context.SaveChanges();
                }

                if (!context.MailPrefferences.Any())
                {
                    var mailPrefferences = new List<MailPrefference>
                    {
                        new MailPrefference { ID = "Any", Name = "No Prefference"},
                        new MailPrefference { ID = "Work", Name = "Work Email"},
                        new MailPrefference { ID = "Personnal", Name = "Personnal Email"}
                    };
                    context.MailPrefferences.AddRange(mailPrefferences);
                    context.SaveChanges();
                }

                // If no Members seed some.
                if (!context.Members.Any())
                {
                    context.Members.AddRange(
                    new Member
                    {
                        Salutation = "Mrs.",
                        FirstName = "Ariana",
                        MiddleName = "Medlina",
                        LastName = "Grande",
                        Phone = "2435436547",
                        DateJoined = DateTime.Parse("1991-02-09"),
                        ProvinceID = "AB",
                        Email = "agrande@outlook.com",
                        IsActive = true,
                        StreetAddress = "58 Rose PLace",
                        City = "Welland",
                        PostalCode = "E5E6R6",
                        WorkStreetAddress = "52 Rose PLace",
                        WorkCity = "Toronto",
                        WorkPostalCode = "E5E6R7",
                        WorkProvinceID = "ON",
                        WorkEmail = "agrande@gmail.com",
                        WorkPhone = "1592587536"
                    },
                    new Member
                    {
                        Salutation = "Mrs.",
                        FirstName = "Elton",
                        MiddleName = "Sebestian",
                        LastName = "John",
                        Phone = "243534243",
                        DateJoined = DateTime.Parse("1943-09-12"),
                        ProvinceID = "ON",
                        Email = "ejohn@outlook.com",
                        IsActive = true,
                        StreetAddress = "58 King Street",
                        City = "Welland",
                        PostalCode = "E5E6R7",
                        WorkStreetAddress = "57 King Street",
                        WorkCity = "Toronto",
                        WorkPostalCode = "E4E6R7",
                        WorkProvinceID = "ON",
                        WorkEmail = "ejohn@gmail.com",
                        WorkPhone = "1592587836"
                    },
                    new Member
                    {
                        Salutation = "Mr.",
                        FirstName = "Lil",
                        MiddleName = "Nas",
                        LastName = "X",
                        Phone = "6543455678",
                        DateJoined = DateTime.Parse("1987-08-10"),
                        ProvinceID = "BC",
                        Email = "lx@outlook.com",
                        IsActive = true,
                        StreetAddress = "58 John Street",
                        City = "Niagara",
                        PostalCode = "J5E6R7",
                        WorkStreetAddress = "55 John Street",
                        WorkCity = "Toronto",
                        WorkPostalCode = "L4E6R7",
                        WorkProvinceID = "AB",
                        WorkEmail = "lx@gmail.com",
                        WorkPhone = "1572587836"
                    },
                    new Member
                    {
                        Salutation = "Mr.",
                        FirstName = "Zayn",
                        MiddleName = "A",
                        LastName = "Malik",
                        Phone = "3455466544",
                        DateJoined = DateTime.Parse("1999-01-10"),
                        ProvinceID = "BC",
                        Email = "zmalik@outlook.com",
                        IsActive = true,
                        StreetAddress = "22 Watt Avenue",
                        City = "Torornto",
                        PostalCode = "L3B6E6",
                        WorkStreetAddress = "57 Empress Street",
                        WorkCity = "Toronto",
                        WorkPostalCode = "E4B6R7",
                        WorkProvinceID = "AB",
                        WorkEmail = "zmalik@gmail.com",
                        WorkPhone = "1492557836"
                    },
                    new Member
                    {
                        Salutation = "Mrs.",
                        FirstName = "Kelly",
                        MiddleName = "Lidiya",
                        LastName = "Clearkson",
                        Phone = "2343455436",
                        DateJoined = DateTime.Parse("1984-12-08"),
                        ProvinceID = "ON",
                        Email = "lclearkson@outlook.com",
                        IsActive = true,
                        StreetAddress = "22 Queen Avenue",
                        City = "Welland",
                        PostalCode = "L3G6E6",
                        WorkStreetAddress = "57 Dany Street",
                        WorkCity = "Toronto",
                        WorkPostalCode = "E4A6R7",
                        WorkProvinceID = "ON",
                        WorkEmail = "lclearkson@gmail.com",
                        WorkPhone = "1592557836"
                    },
                    new Member
                    {
                        Salutation = "Princess",
                        FirstName = "Lhea",
                        MiddleName = "",
                        LastName = "Skywalker",
                        Phone = "2435424547",
                        DateJoined = DateTime.Parse("1991-03-09"),
                        ProvinceID = "YT",
                        Email = "lskywalker@outlook.com",
                        IsActive= false
                    },
                    new Member
                    {
                        Salutation = "Mrs.",
                        FirstName = "Christine",
                        MiddleName = "Ela",
                        IsNCGrad = true,
                        LastName = "Catherin",
                        Phone = "243ew34243",
                        DateJoined = DateTime.Parse("1978-09-12"),
                        ProvinceID = "MB",
                        Email = "ccatherin@outlook.com",
                        IsActive = true
                    },
                    new Member
                    {
                        Salutation = "Mr.",
                        FirstName = "Denarys",
                        MiddleName = "Valerian",
                        IsNCGrad = true,
                        LastName = "Targarian",
                        Phone = "6541255678",
                        DateJoined = DateTime.Parse("1957-08-10"),
                        ProvinceID = "QC",
                        Email = "dtargarian@outlook.com",
                        IsActive = false
                    },
                    new Member
                    {
                        Salutation = "Mr.",
                        FirstName = "Jenny",
                        MiddleName = "T",
                        LastName = "Babbenburg",
                        Phone = "3455461244",
                        DateJoined = DateTime.Parse("1999-01-10"),
                        ProvinceID = "AB",
                        Email = "jbabbenberg@outlook.com",
                        IsActive = true
                    },
                    new Member
                    {
                        Salutation = "Mrs.",
                        FirstName = "Samuel",
                        MiddleName = "R",
                        IsNCGrad = true,
                        LastName = "Johnson",
                        Phone = "2342355436",
                        DateJoined = DateTime.Parse("1984-12-08"),
                        ProvinceID = "MB",
                        Email = "sjohnson@outlook.com",
                        IsActive = true
                    },
                    new Member
                    {
                        Salutation = "Mr.",
                        FirstName = "Johnney",
                        MiddleName = "",
                        IsNCGrad = true,
                        LastName = "Khalifa",
                        Phone = "2435836547",
                        DateJoined = DateTime.Parse("1991-02-09"),
                        ProvinceID = "QC",
                        Email = "jkhalifa@outlook.com",
                        IsActive = true
                    },
                    new Member
                    {
                        Salutation = "Mr.",
                        FirstName = "Christopher",
                        MiddleName = "",
                        LastName = "Nolan",
                        Phone = "243534243",
                        DateJoined = DateTime.Parse("1943-09-12"),
                        ProvinceID = "NT",
                        Email = "cnolan@outlook.com",
                        IsActive = true
                    },
                    new Member
                    {
                        Salutation = "Mr.",
                        FirstName = "Randal",
                        MiddleName = "",
                        IsNCGrad = true,
                        LastName = "Tarley",
                        Phone = "6543421678",
                        DateJoined = DateTime.Parse("1987-08-10"),
                        ProvinceID = "PE",
                        Email = "rtarley@outlook.com",
                        IsActive= true
                    },
                    new Member
                    {
                        Salutation = "Mrs.",
                        FirstName = "Anakin",
                        MiddleName = "A",
                        LastName = "Skywalker",
                        Phone = "3455466544",
                        IsNCGrad = true,
                        DateJoined = DateTime.Parse("1999-01-10"),
                        ProvinceID = "NB",
                        Email = "askywalker@outlook.com",
                        IsActive = true
                    },
                    new Member
                    {
                        Salutation = "Mrs.",
                        FirstName = "Bruce",
                        MiddleName = "",
                        LastName = "Waine",
                        Phone = "2343452436",
                        DateJoined = DateTime.Parse("1999-12-08"),
                        ProvinceID = "SK",
                        Email = "bwaine@outlook.com",
                        IsActive = true
                    });
                    context.SaveChanges();
                }

                // If no Members seed some.
                if (!context.MemberCommitees.Any())
                {
                    context.MemberCommitees.AddRange(
                    new MemberCommitee
                    {
                        MemberID = 3,
                        CommiteeID = 2                        
                    },
                    new MemberCommitee
                    {
                        MemberID = 3,
                        CommiteeID = 1
                    },
                    new MemberCommitee
                    {
                        MemberID = 3,
                        CommiteeID = 3
                    },
                    new MemberCommitee
                    {
                        MemberID = 1,
                        CommiteeID = 4
                    },
                    new MemberCommitee
                    {
                        MemberID = 2,
                        CommiteeID = 1
                    },
                    new MemberCommitee
                    {
                        MemberID = 2,
                        CommiteeID = 2
                    },
                    new MemberCommitee
                    {
                        MemberID = 1,
                        CommiteeID = 3
                    },
                    new MemberCommitee
                    {
                        MemberID = 4,
                        CommiteeID = 3
                    },
                    new MemberCommitee
                    {
                        MemberID = 4,
                        CommiteeID = 4
                    },
                    new MemberCommitee
                    {
                        MemberID = 5,
                        CommiteeID = 2
                    },
                    new MemberCommitee
                    {
                        MemberID = 7,
                        CommiteeID = 10
                    },
                    new MemberCommitee
                    {
                        MemberID = 2,
                        CommiteeID = 8
                    },
                    new MemberCommitee
                    {
                        MemberID = 9,
                        CommiteeID = 12
                    },
                    new MemberCommitee
                    {
                        MemberID = 6,
                        CommiteeID = 15
                    },
                    new MemberCommitee
                    {
                        MemberID = 10,
                        CommiteeID = 14
                    },
                    new MemberCommitee
                    {
                        MemberID = 7,
                        CommiteeID = 11
                    },
                    new MemberCommitee
                    {
                        MemberID = 12,
                        CommiteeID = 7
                    },
                    new MemberCommitee
                    {
                        MemberID = 13,
                        CommiteeID = 15
                    },
                    new MemberCommitee
                    {
                        MemberID = 13,
                        CommiteeID = 14
                    },
                    new MemberCommitee
                    {
                        MemberID = 12,
                        CommiteeID = 11
                    });
                    context.SaveChanges();
                }

                if (!context.Meetings.Any())
                {
                    context.Meetings.AddRange(
                     new Meeting
                     {
                         MeetingTitle = "Initial Discussion",
                         Description = "Is a special meeting for everyone",
                         IsCancelled = false,
                         CommiteeID= 1
                     },
                     new Meeting
                     {
                         MeetingTitle = "PAC Discussion",
                         Description = "Is a special meeting",
                         IsCancelled = true,
                         CommiteeID = 3
                     },
                     new Meeting
                     {
                         MeetingTitle = "Commitee Discussion",
                         Description = "Everyone Must Join",
                         IsCancelled = false,
                         CommiteeID = 2
                     });

                    context.SaveChanges();
                }


                if (!context.ActionItems.Any())
                {
                    context.ActionItems.AddRange(
                    new ActionItem
                    {
                        ActionItemTitle = "Organization plans for the annual meetup",
                        Description = "Please follow the instructions of the admin for now.",
                        TimeAppointed = DateTime.Now,
                        TimeUntil = DateTime.Now.AddDays(3),
                        MemberID = 3,
                        MeetingID = 1,
                        IsCompleted = false
                    },
                    new ActionItem
                    {
                        ActionItemTitle = "Complete the commitee report for the year",
                        Description = "Please follow the instructions of the admin for now.",
                        TimeAppointed = DateTime.Now,
                        TimeUntil = DateTime.Now.AddDays(10),
                        MemberID = 2,
                        MeetingID = 1,
                        IsCompleted = false
                    },
                    new ActionItem
                    {
                        ActionItemTitle = "Arrange the pac member survay",
                        Description = "Please follow the instructions of the admin for now.",
                        TimeAppointed = DateTime.Now,
                        TimeUntil = DateTime.Now.AddDays(6),
                        MemberID = 4,
                        MeetingID = 2,
                        IsCompleted = true
                    },
                    new ActionItem
                    {
                        ActionItemTitle = "Setup 2FA to increase your account security",
                        Description = "Please follow the instructions of the admin for now.",
                        TimeAppointed = DateTime.Now,
                        TimeUntil = DateTime.Now.AddDays(15),
                        MemberID = 2,
                        MeetingID = 3,
                        IsCompleted = false
                    },
                    new ActionItem
                    {
                        ActionItemTitle = "Setup a forum to get feedback from members",
                        Description = "Please follow the instructions of the admin for now.",
                        TimeAppointed = DateTime.Now,
                        TimeUntil = DateTime.Now.AddDays(12),
                        MemberID = 2,
                        MeetingID = 3,
                        IsCompleted = false
                    });
                    context.SaveChanges();
                }


            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.GetBaseException().Message);
            }
        }
    }
}
