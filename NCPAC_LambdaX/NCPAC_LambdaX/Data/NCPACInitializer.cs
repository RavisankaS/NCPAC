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
                        DateJoined = DateTime.Parse("1991-02-09"),
                        CommiteeID = 1
                    },
                    new Member
                    {
                        FirstName = "Elton",
                        MiddleName = "Sebestian",
                        LastName = "John",
                        Phone = "243534243",
                        DateJoined = DateTime.Parse("1943-09-12"),
                        CommiteeID = 2
                    },
                    new Member
                    {
                        FirstName = "Lil",
                        MiddleName = "Nas",
                        LastName = "X",
                        Phone = "6543455678",
                        DateJoined = DateTime.Parse("1987-08-10"),
                        CommiteeID = 2
                    },
                    new Member
                    {
                        FirstName = "Zayn",
                        MiddleName = "A",
                        LastName = "Malik",
                        Phone = "3455466544",
                        DateJoined = DateTime.Parse("1999-01-10"),
                        CommiteeID = 3
                    },
                    new Member
                    {
                        FirstName = "Kelly",
                        MiddleName = "Lidiya",
                        LastName = "Clearkson",
                        Phone = "2343455436",
                        DateJoined = DateTime.Parse("1984-12-08"),
                        CommiteeID = 4
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
