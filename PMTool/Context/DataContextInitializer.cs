using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Web.Security;
using PMTool.Repository;


public class PMToolContextInitializer : MigrateDatabaseToLatestVersion<PMToolContext, PMTool.Migrations.Configuration>
{
   
}
    //{
    //    protected override void Seed(DataContext context)
    //    {
    //    WebSecurity.Register("Demo", "123456", "demo@demo.com", true, "Demo", "Demo");
    //    Roles.CreateRole("Admin");
    //    Roles.AddUserToRole("Demo", "Admin");
    //    }
    //}