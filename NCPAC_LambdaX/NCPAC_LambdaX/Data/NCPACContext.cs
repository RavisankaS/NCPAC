using NCPAC_LambdaX.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Metrics;
using NCPAC_LambdaX.ViewModels;

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
        public DbSet<Meeting> Meetings { get; set; }
        public DbSet<MemberCommitee> MemberCommitees { get; set; }
        public DbSet<MeetingCommitee> MeetingCommitees { get; set; }
        public DbSet<Province> Provinces { get; set; }
        public DbSet<MailPrefference> MailPrefferences { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<ActionItem> ActionItems { get; set; }
        public DbSet<Announcement> Announcements { get; set; }
        public DbSet<MeetingDocument> MeetingDocuments { get; set; }
        public DbSet<ActionItemDocument> ActionItemDocuments { get; set; }
        public DbSet<UploadedFile> UploadedFiles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MemberCommitee>()
            .HasKey(p => new { p.MemberID, p.CommiteeID });

            modelBuilder.Entity<MeetingCommitee>()
            .HasKey(p => new { p.MeetingID, p.CommiteeID });

            modelBuilder.Entity<MemberCommitee>()
            .HasOne(mc => mc.Member)
            .WithMany(m => m.MemberCommitees)
            .HasForeignKey(mc => mc.MemberID)
            .OnDelete(DeleteBehavior.Restrict);
        }

        public DbSet<NCPAC_LambdaX.ViewModels.MemberVM> MemberVM { get; set; }
    }
}
