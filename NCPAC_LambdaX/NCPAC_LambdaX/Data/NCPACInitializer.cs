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
                        FirstName = "Ariana",
                        MiddleName = "Medlina",
                        LastName = "Grande",
                        Phone = "2435436547",
                        DateJoined = DateTime.Parse("1991-02-09")
                    },
                    new Member
                    {
                        FirstName = "Elton",
                        MiddleName = "Sebestian",
                        LastName = "John",
                        Phone = "243534243",
                        DateJoined = DateTime.Parse("1943-09-12")
                    },
                    new Member
                    {
                        FirstName = "Lil",
                        MiddleName = "Nas",
                        LastName = "X",
                        Phone = "6543455678",
                        DateJoined = DateTime.Parse("1987-08-10")
                    },
                    new Member
                    {
                        FirstName = "Zayn",
                        MiddleName = "A",
                        LastName = "Malik",
                        Phone = "3455466544",
                        DateJoined = DateTime.Parse("1999-01-10")
                    },
                    new Member
                    {
                        FirstName = "Kelly",
                        MiddleName = "Lidiya",
                        LastName = "Clearkson",
                        Phone = "2343455436",
                        DateJoined = DateTime.Parse("1984-12-08")
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
