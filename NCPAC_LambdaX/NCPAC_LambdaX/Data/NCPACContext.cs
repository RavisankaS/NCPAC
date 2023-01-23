﻿using NCPAC_LambdaX.Models;
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
        public DbSet<MemberCommitee> MemberCommitees { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MemberCommitee>()
            .HasKey(p => new { p.MemberID, p.CommiteeID });

            modelBuilder.Entity<MemberCommitee>()
            .HasOne(mc => mc.Member)
            .WithMany(m => m.MemberCommitees)
            .HasForeignKey(mc => mc.MemberID)
            .OnDelete(DeleteBehavior.Restrict);
        }
    }
}