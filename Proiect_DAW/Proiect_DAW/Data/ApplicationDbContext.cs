using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Proiect_DAW.Models;
using Task = Proiect_DAW.Models.Task;

namespace Proiect_DAW.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Task> Tasks { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Note> Note { get; set; }
        public DbSet<Project> Projects { get; set; }


        //relatiile many to many dintre User si Project
        public DbSet<UserProject> UserProjects { get; set; }

        public DbSet<CollabProject> CollabProjects { get; set; }

        //relatiile many to many dintre Task si User
        public DbSet<TaskAsignee> TaskAsignees { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Organizatori
            modelBuilder.Entity<UserProject>()
            .HasKey(up => new
            {
                up.Id,
                up.ProjectId,
                up.UserId
            });

            modelBuilder.Entity<UserProject>()
            .HasOne(up => up.Project)
            .WithMany(up => up.Organizers)
            .HasForeignKey(up => up.ProjectId);

            modelBuilder.Entity<UserProject>()
            .HasOne(up => up.User)
            .WithMany(up => up.Projects)
            .HasForeignKey(up => up.UserId);

            // Colaboratori

            modelBuilder.Entity<CollabProject>()
            .HasKey(cp => new
            {
                cp.Id,
                cp.ProjectId,
                cp.UserId
            });

            modelBuilder.Entity<CollabProject>()
            .HasOne(cp => cp.Project)
            .WithMany(cp => cp.Collaborators)
            .HasForeignKey(cp => cp.ProjectId);

            modelBuilder.Entity<CollabProject>()
            .HasOne(cp => cp.User)
            .WithMany(cp => cp.CollabProjects)
            .HasForeignKey(cp => cp.UserId);

            // TaskAsignees

            modelBuilder.Entity<TaskAsignee>()
           .HasKey(ta => new
           {
               ta.Id,
               ta.TaskId,
               ta.UserId
           });

            modelBuilder.Entity<TaskAsignee>()
            .HasOne(ta => ta.Task)
            .WithMany(ta => ta.Assignees)
            .HasForeignKey(ta => ta.TaskId);

            modelBuilder.Entity<TaskAsignee>()
            .HasOne(ta => ta.User)
            .WithMany(ta => ta.Tasks)
            .HasForeignKey(ta => ta.UserId);

        }
    }
}