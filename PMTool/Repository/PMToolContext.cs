using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using PMTool.Models;

namespace PMTool.Repository
{
    public class PMToolContext : DbContext
    {

        //public DbSet<PMTool.Models.Test> Tests { get; set; }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }

        public DbSet<PMTool.Models.Priority> Priorities { get; set; }

        public DbSet<PMTool.Models.Label> Labels { get; set; }

        public DbSet<PMTool.Models.Project> Projects { get; set; }

        public DbSet<PMTool.Models.Task> Tasks { get; set; }

        public DbSet<PMTool.Models.Notification> Notifications { get; set; }

        public DbSet<PMTool.Models.ProjectStatus> ProjectStatuses { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();

            modelBuilder.Entity<Project>().
                HasMany(i => i.Users).
                WithMany(c => c.Projects).
                Map(mc =>
                {
                    mc.MapLeftKey("ProjectId");
                    mc.MapRightKey("UserId");
                    mc.ToTable("ProjectUsers");
                });

            modelBuilder.Entity<Task>().
                HasMany(i => i.Users).
                WithMany(c => c.Tasks).
                Map(mc =>
                {
                    mc.MapLeftKey("TaskId");
                    mc.MapRightKey("UserId");
                    mc.ToTable("TaskUsers");
                });

            modelBuilder.Entity<Task>().
               HasMany(i => i.Followers).
               WithMany(c => c.FollowerTasks).
               Map(mc =>
               {
                   mc.MapRightKey("TaskId");
                   mc.MapLeftKey("UserId");
                   mc.ToTable("TaskFollowers");
               });

            modelBuilder.Entity<Task>().
              HasMany(i => i.Labels).
              WithMany(c => c.Tasks).
              Map(mc =>
              {
                  mc.MapRightKey("TaskId");
                  mc.MapLeftKey("LabelId");
                  mc.ToTable("TaskLabels");
              });

            modelBuilder.Entity<Project>()
                .HasMany(i => i.ProjectOwners)
                .WithMany(c => c.OwnerProjects)
                .Map(mc =>
                    {
                        mc.MapRightKey("UserId");
                        mc.MapLeftKey("ProjectID");
                        mc.ToTable("ProjectOwners");
                    });

        }

        public DbSet<PMTool.Models.Sprint> Sprints { get; set; }

        public DbSet<PMTool.Models.TaskActivityLog> TaskActivityLogs { get; set; }

        public DbSet<PMTool.Models.ProjectStatusRule> ProjectStatusRules { get; set; }

        public DbSet<PMTool.Models.TimeLog> TimeLogs { get; set; }

        public DbSet<PMTool.Models.TaskMessage> TaskMessages { get; set; }

    }
}