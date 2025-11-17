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

            modelBuilder.Entity<ApplicationUser>()
                .HasOne(u => u.StudentProfile)
                .WithOne(s => s.User)
                .HasForeignKey<StudentProfile>(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            //CompanyProfile
            modelBuilder.Entity<ApplicationUser>()
                .HasOne(u => u.CompanyProfile)
                .WithOne(c => c.User)
                .HasForeignKey<CompanyProfile>(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // CompanyProfile
            modelBuilder.Entity<CompanyProfile>()
                .HasMany(c => c.JobPostings)
                .WithOne(j => j.Company)
                .HasForeignKey(j => j.CompanyId)
                .OnDelete(DeleteBehavior.Cascade);

            // JobPosting 
            modelBuilder.Entity<JobPosting>()
                .HasMany(j => j.Applications)
                .WithOne(a => a.JobPosting)
                .HasForeignKey(a => a.JobPostingId)
                .OnDelete(DeleteBehavior.Restrict);

            //Application
            modelBuilder.Entity<StudentProfile>()
                .HasMany(s => s.Applications)
                .WithOne(a => a.Student)
                .HasForeignKey(a => a.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ApplicationUser>()
                .HasMany(u => u.SentMessages)
                .WithOne(m => m.Sender)
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            // ApplicationUser - Message (Receiver) (1:N)
            modelBuilder.Entity<ApplicationUser>()
                .HasMany(u => u.ReceivedMessages)
                .WithOne(m => m.Receiver)
                .HasForeignKey(m => m.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);

            // Application - Message (1:N) - Optional
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

            // Seed Data - Admin User
            modelBuilder.Entity<ApplicationUser>().HasData(
                new ApplicationUser
                {
                    Id = "admin-seed-id",
                    UserName = "admin@internify.com",
                    NormalizedUserName = "ADMIN@INTERNIFY.COM",
                    Email = "admin@internify.com",
                    NormalizedEmail = "ADMIN@INTERNIFY.COM",
                    EmailConfirmed = true,
                    PasswordHash = "AQAAAAIAAYagAAAAEJ8xQx6TJYqK7X1mGKjHQ4P7dK0pZEwR4FqKz9Vr5YdN7mG3Hw0xLqZ8Yz3pX2Q==", // Password: Admin@123
                    SecurityStamp = Guid.NewGuid().ToString(),
                    FullName = "Admin User",
                    Role = "Admin",
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                }
            );
        }
    }
}

