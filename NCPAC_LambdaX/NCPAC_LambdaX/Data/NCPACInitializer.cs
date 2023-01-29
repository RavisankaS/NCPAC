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

                // Seed Commitees if there arent any.
                if (!context.Commitees.Any())
                {
                    context.Commitees.AddRange(
                    new Commitee
                    {
                        CommiteeName = "Technology and Industrial Automation",
                        Division = "Technology"
                    },
                    new Commitee
                    {
                        CommiteeName = "Mechanical Engineering",
                        Division = "Technology"
                    },
                    new Commitee
                    {
                        CommiteeName = "Renewable Energies",
                        Division = "Technology"
                    },
                    new Commitee
                    {
                        CommiteeName = "Computer, Electrical & Electronics Engineering",
                        Division = "Technology"
                    },
                    new Commitee
                    {
                        CommiteeName = "Computer Programming",
                        Division = "Media"
                    },
                    new Commitee
                    {
                        CommiteeName = "Game Developement",
                        Division = "Media"
                    },
                    new Commitee
                    {
                        CommiteeName = "Graphic Design",
                        Division = "Media"
                    },
                    new Commitee
                    {
                        CommiteeName = "Public Relations",
                        Division = "Media"
                    },
                    new Commitee
                    {
                        CommiteeName = "Social Media Management",
                        Division = "Media"
                    },
                    new Commitee
                    {
                        CommiteeName = "Hair Styling",
                        Division = "Trades"
                    },
                    new Commitee
                    {
                        CommiteeName = "Motive Power",
                        Division = "Trades"
                    },
                    new Commitee
                    {
                        CommiteeName = "Carpentry and Rennovation",
                        Division = "Trades"
                    },
                    new Commitee
                    {
                        CommiteeName = "Mechanical Techniques",
                        Division = "Trades"
                    },
                    new Commitee
                    {
                        CommiteeName = "Welding Skills",
                        Division = "Trades"
                    },
                    new Commitee
                    {
                        CommiteeName = "Electrical Technician",
                        Division = "Trades"
                    }

                    );
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
                        Province = "AB",
                        Email = "agrande@outlook.com"
                    },
                    new Member
                    {
                        Salutation = "Mrs.",
                        FirstName = "Elton",
                        MiddleName = "Sebestian",
                        LastName = "John",
                        Phone = "243534243",
                        DateJoined = DateTime.Parse("1943-09-12"),
                        Province = "AB",
                        Email = "ejohn@outlook.com"
                    },
                    new Member
                    {
                        Salutation = "Mr.",
                        FirstName = "Lil",
                        MiddleName = "Nas",
                        LastName = "X",
                        Phone = "6543455678",
                        DateJoined = DateTime.Parse("1987-08-10"),
                        Province = "AB",
                        Email = "lx@outlook.com"
                    },
                    new Member
                    {
                        FirstName = "Zayn",
                        MiddleName = "A",
                        LastName = "Malik",
                        Phone = "3455466544",
                        DateJoined = DateTime.Parse("1999-01-10"),
                        Province = "AB",
                        Email = "zmalik@outlook.com"
                    },
                    new Member
                    {
                        FirstName = "Kelly",
                        MiddleName = "Lidiya",
                        LastName = "Clearkson",
                        Phone = "2343455436",
                        DateJoined = DateTime.Parse("1984-12-08"),
                        Province = "AB",
                        Email = "lclearkson@outlook.com"
                    },
                    new Member
                    {
                        Salutation = "Princess",
                        FirstName = "Lhea",
                        MiddleName = "",
                        LastName = "Skywalker",
                        Phone = "2435424547",
                        DateJoined = DateTime.Parse("1991-03-09"),
                        Province = "AB",
                        Email = "lskywalker@outlook.com"
                    },
                    new Member
                    {
                        FirstName = "Christine",
                        MiddleName = "Ela",
                        LastName = "Catherin",
                        Phone = "243ew34243",
                        DateJoined = DateTime.Parse("1978-09-12"),
                        Province = "AB",
                        Email = "ccatherin@outlook.com"
                    },
                    new Member
                    {
                        Salutation = "Mr.",
                        FirstName = "Denarys",
                        MiddleName = "Valerian",
                        LastName = "Targarian",
                        Phone = "6541255678",
                        DateJoined = DateTime.Parse("1957-08-10"),
                        Province = "AB",
                        Email = "dtargarian@outlook.com"
                    },
                    new Member
                    {
                        Salutation = "Mr.",
                        FirstName = "Jenny",
                        MiddleName = "T",
                        LastName = "Babbenburg",
                        Phone = "3455461244",
                        DateJoined = DateTime.Parse("1999-01-10"),
                        Province = "AB",
                        Email = "jbabbenberg@outlook.com"
                    },
                    new Member
                    {
                        Salutation = "Mrs.",
                        FirstName = "Samuel",
                        MiddleName = "R",
                        LastName = "Johnson",
                        Phone = "2342355436",
                        DateJoined = DateTime.Parse("1984-12-08"),
                        Province = "AB",
                        Email = "sjohnson@outlook.com"
                    },
                    new Member
                    {
                        FirstName = "Johnney",
                        MiddleName = "",
                        LastName = "Khalifa",
                        Phone = "2435836547",
                        DateJoined = DateTime.Parse("1991-02-09"),
                        Province = "AB",
                        Email = "jkhalifa@outlook.com"
                    },
                    new Member
                    {
                        Salutation = "Mrs.",
                        FirstName = "Christopher",
                        MiddleName = "",
                        LastName = "Nolan",
                        Phone = "243534243",
                        DateJoined = DateTime.Parse("1943-09-12"),
                        Province = "ON",
                        Email = "cnolan@outlook.com"
                    },
                    new Member
                    {
                        FirstName = "Randal",
                        MiddleName = "",
                        LastName = "Tarley",
                        Phone = "6543421678",
                        DateJoined = DateTime.Parse("1987-08-10"),
                        Province = "ON",
                        Email = "rtarley@outlook.com"
                    },
                    new Member
                    {
                        Salutation = "Mrs.",
                        FirstName = "Anakin",
                        MiddleName = "A",
                        LastName = "Skywalker",
                        Phone = "3455466544",
                        DateJoined = DateTime.Parse("1999-01-10"),
                        Province = "ON",
                        Email = "askywalker@outlook.com"
                    },
                    new Member
                    {
                        Salutation = "Mrs.",
                        FirstName = "Bruce",
                        MiddleName = "",
                        LastName = "Waine",
                        Phone = "2343452436",
                        DateJoined = DateTime.Parse("1999-12-08"),
                        Province = "ON",
                        Email = "bwaine@outlook.com"
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



            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.GetBaseException().Message);
            }
        }
    }
}
