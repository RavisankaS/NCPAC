using NCPAC_LambdaX.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Metrics;

namespace NCPAC_LambdaX.Data
{
    public class NCPACContext : DbContext
    {
        public NCPACContext(DbContextOptions<NCPACContext> options)
            : base(options)
        {

        }

        public DbSet<Commitee> Commitees { get; set; }
        public DbSet<Member> Members { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {


            modelBuilder.Entity<Commitee>()
                .HasMany<Member>(d => d.Members)
                .WithOne(p => p.Commitee)
                .HasForeignKey(p => p.CommiteeID)
                .OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(modelBuilder);
        }
    }
}
