using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StajPortal.Models.Entities;

namespace StajPortal.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSets
        public DbSet<StudentProfile> StudentProfiles { get; set; }
        public DbSet<CompanyProfile> CompanyProfiles { get; set; }
        public DbSet<JobPosting> JobPostings { get; set; }
        public DbSet<Application> Applications { get; set; }
        public DbSet<Message> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ApplicationUser - StudentProfile (1:1)
            modelBuilder.Entity<ApplicationUser>()
                .HasOne(u => u.StudentProfile)
                .WithOne(s => s.User)
                .HasForeignKey<StudentProfile>(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // ApplicationUser - CompanyProfile (1:1)
            modelBuilder.Entity<ApplicationUser>()
                .HasOne(u => u.CompanyProfile)
                .WithOne(c => c.User)
                .HasForeignKey<CompanyProfile>(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // CompanyProfile - JobPosting (1:N)
            modelBuilder.Entity<CompanyProfile>()
                .HasMany(c => c.JobPostings)
                .WithOne(j => j.Company)
                .HasForeignKey(j => j.CompanyId)
                .OnDelete(DeleteBehavior.Cascade);

            // JobPosting - Application (1:N)
            modelBuilder.Entity<JobPosting>()
                .HasMany(j => j.Applications)
                .WithOne(a => a.JobPosting)
                .HasForeignKey(a => a.JobPostingId)
                .OnDelete(DeleteBehavior.Restrict);

            // StudentProfile - Application (1:N)
            modelBuilder.Entity<StudentProfile>()
                .HasMany(s => s.Applications)
                .WithOne(a => a.Student)
                .HasForeignKey(a => a.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            // StudentProfile - GPA precision
            modelBuilder.Entity<StudentProfile>()
                .Property(s => s.GPA)
                .HasPrecision(3, 2); // 0.00 - 4.00 arası

            // ApplicationUser - Message
            modelBuilder.Entity<ApplicationUser>()
                .HasMany(u => u.SentMessages)
                .WithOne(m => m.Sender)
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            // ApplicationUser - Message
            modelBuilder.Entity<ApplicationUser>()
                .HasMany(u => u.ReceivedMessages)
                .WithOne(m => m.Receiver)
                .HasForeignKey(m => m.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);

            // Application - Message
            modelBuilder.Entity<Application>()
                .HasMany(a => a.Messages)
                .WithOne(m => m.RelatedApplication)
                .HasForeignKey(m => m.RelatedApplicationId)
                .OnDelete(DeleteBehavior.SetNull);

            // Indexes
            modelBuilder.Entity<JobPosting>()
                .HasIndex(j => j.IsActive);

            modelBuilder.Entity<JobPosting>()
                .HasIndex(j => j.IsApproved);

            modelBuilder.Entity<Application>()
                .HasIndex(a => a.Status);

            modelBuilder.Entity<Message>()
                .HasIndex(m => m.IsRead);
        }
    }
}

