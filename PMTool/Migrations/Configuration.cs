namespace PMTool.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using System.Web.Security;
    using PMTool.Models;   

    public sealed class Configuration : DbMigrationsConfiguration<PMTool.Repository.PMToolContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            ContextKey = "PMTool.Repository.PMToolContext"; 
        }

        protected override void Seed(PMTool.Repository.PMToolContext context)
        {
            context.Priorities.AddOrUpdate(c => c.Name,
            new Priority { Name = "High", Description = "High" },
            new Priority { Name = "Medium", Description = "Medium" },
            new Priority { Name = "Low", Description = "Low" }
            );
            //context.Labels.AddOrUpdate(c => c.Name,
            //new Label { Name = "Label1", Description = "Label1", ActionDate = DateTime.Now, CreateDate = DateTime.Now, ModificationDate = DateTime.Now, ModifieddBy = (int)Membership.GetUser("Demo").ProviderUserKey, CreatedBy = (int)Membership.GetUser("Demo").ProviderUserKey, IsActive = true },
            //new Label { Name = "Label2", Description = "Label2", ActionDate = DateTime.Now, CreateDate = DateTime.Now, ModificationDate = DateTime.Now, ModifieddBy = (int)Membership.GetUser("Demo").ProviderUserKey, CreatedBy = (int)Membership.GetUser("Demo").ProviderUserKey, IsActive = true },
            //new Label { Name = "Label3", Description = "Label3", ActionDate = DateTime.Now, CreateDate = DateTime.Now, ModificationDate = DateTime.Now, ModifieddBy = (int)Membership.GetUser("Demo").ProviderUserKey, CreatedBy = (int)Membership.GetUser("Demo").ProviderUserKey, IsActive = true }
            //);
        }
    }
}
