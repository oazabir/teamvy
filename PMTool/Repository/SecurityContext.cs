using PMTool.Entities;
using PMTool.Repository.ModelConfiguration;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMTool.Repository
{
    public class SecurityContext : DbContext
    {
        public SecurityContext()
            : base("PMToolContext")
        {
        }

        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<Resource> Resources { get; set; }
        public DbSet<OperationsToRoles> OperationsToRoles { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new ResourceConfiguration());
            modelBuilder.Configurations.Add(new OperationsToRolesConfiguration());
        }
    }
}