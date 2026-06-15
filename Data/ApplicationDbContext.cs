using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using AdvancedVotingSystem.Models;

namespace AdvancedVotingSystem.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Election> Elections { get; set; }
        public DbSet<ElectionCategory> ElectionCategories { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<Candidate> Candidates { get; set; }
        public DbSet<Committee> Committees { get; set; }
        public DbSet<Voter> Voters { get; set; }
        public DbSet<VoteRecord> VoteRecords { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // Required for Identity

            // Configure Delete Behaviors to avoid multiple cascade paths

            // Election -> Committees
            modelBuilder.Entity<Committee>()
                .HasOne(c => c.Election)
                .WithMany(e => e.Committees)
                .HasForeignKey(c => c.ElectionId)
                .OnDelete(DeleteBehavior.Restrict);

            // Election -> Categories
            modelBuilder.Entity<ElectionCategory>()
                .HasOne(ec => ec.Election)
                .WithMany(e => e.Categories)
                .HasForeignKey(ec => ec.ElectionId)
                .OnDelete(DeleteBehavior.Cascade);

            // Election -> Positions
            modelBuilder.Entity<Position>()
                .HasOne(p => p.Election)
                .WithMany(e => e.Positions)
                .HasForeignKey(p => p.ElectionId)
                .OnDelete(DeleteBehavior.Cascade);

            // Position -> Candidates
            modelBuilder.Entity<Candidate>()
                .HasOne(c => c.Position)
                .WithMany(p => p.Candidates)
                .HasForeignKey(c => c.PositionId)
                .OnDelete(DeleteBehavior.Cascade);

            // Committee -> Voters
            modelBuilder.Entity<Voter>()
                .HasOne(v => v.Committee)
                .WithMany(c => c.Voters)
                .HasForeignKey(v => v.CommitteeId)
                .OnDelete(DeleteBehavior.Cascade);

            // Voter -> VoteRecords
            modelBuilder.Entity<VoteRecord>()
                .HasOne(vr => vr.Voter)
                .WithMany(v => v.VoteRecords)
                .HasForeignKey(vr => vr.VoterId)
                .OnDelete(DeleteBehavior.Restrict);

            // Candidate -> VoteRecords
            modelBuilder.Entity<VoteRecord>()
                .HasOne(vr => vr.Candidate)
                .WithMany(c => c.VoteRecords)
                .HasForeignKey(vr => vr.CandidateId)
                .OnDelete(DeleteBehavior.Cascade);

            // Committee -> Head (ApplicationUser)
            modelBuilder.Entity<Committee>()
                .HasOne(c => c.Head)
                .WithMany() // A user can be head of many committees? usually 1, but we don't need a collection in ApplicationUser for it
                .HasForeignKey(c => c.HeadId)
                .OnDelete(DeleteBehavior.SetNull);

            // Committee -> Supervisors (ApplicationUser)
            modelBuilder.Entity<Committee>()
                .HasMany(c => c.Supervisors)
                .WithOne(u => u.Committee)
                .HasForeignKey(u => u.CommitteeId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
