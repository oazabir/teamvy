using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using PMTool.Repository;
using WebMatrix.WebData;

namespace PMTool
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {


        protected void Application_Start()
        {

            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();
            WebSecurity.Register();


            //Database.SetInitializer(new MigrateDatabaseToLatestVersion<PMToolContext, PMTool.Migrations.Configuration>());

            //if (!WebMatrix.WebData.WebSecurity.Initialized)
            //    WebMatrix.WebData.WebSecurity.InitializeDatabaseConnection("PMToolContext",
            //        "UserProfile", "UserId", "UserName", autoCreateTables: true);

            //WebSecurity.InitializeDatabaseConnection("DefaultConnection", "UserProfile", "UserId", "UserName", autoCreateTables: true);
            //Database.SetInitializer(new CreateDatabaseIfNotExists<PMToolContext>());


            //Create some defult user for testing
            //MembershipUser user = WebSecurity.FindUsersByEmail("demo@gmail.com", 0, 10).FirstOrDefault();
            //if (user == null)
            //{
            //    WebSecurity.Register("demo@gmail.com", "bs@123", "demo@gmail.com", true, "Demo", "User");
            //    Roles.CreateRole("Admin");
            //    Roles.AddUserToRole("demo@gmail.com", "Admin");
            //}
            //WebSecurity.Register("najib@bs-23.com", "bs@123", "najib@bs-23.com", true, "Najib", "Hasan");
            ////Roles.CreateRole("Admin");
            //Roles.AddUserToRole("najib@bs-23.com", "Admin");
            //WebSecurity.Register("mahedee.hasan@bs-23.com", "bs@123", "mahedee.hasan@bs-23.com", true, "Mahedee", "Hasan");
            //Roles.CreateRole("Admin");
            //Roles.AddUserToRole("mahedee.hasan@bs-23.com", "Admin");
   

        }


    }
}