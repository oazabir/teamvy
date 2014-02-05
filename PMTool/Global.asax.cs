﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using PMTool.Repository;

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

            Database.SetInitializer(new MigrateDatabaseToLatestVersion<PMToolContext, PMTool.Migrations.Configuration>());
            //Database.SetInitializer(new CreateDatabaseIfNotExists<PMToolContext>());


            //Create some defult user for testing
            MembershipUser user = WebSecurity.FindUsersByEmail("demo@gmail.com", 0, 10).FirstOrDefault();
            if (user == null)
            {
                WebSecurity.Register("demo@gmail.com", "123456", "demo@gmail.com", true, "Demo", "Demo");
                Roles.CreateRole("Admin");
                Roles.AddUserToRole("demo@gmail.com", "Admin");
            }
            WebSecurity.Register("najib@gmail.com", "123456", "najib@gmail.com", true, "najib", "hasan");
            //Roles.CreateRole("Admin");
            Roles.AddUserToRole("najib@gmail.com", "Admin");
            WebSecurity.Register("mahedee.hasan@gmail.com", "123456", "mahedee.hasan@gmail.com", true, "Mahedee", "Hasan");
            //Roles.CreateRole("Admin");
            Roles.AddUserToRole("mahedee.hasan@gmail.com", "Admin");
        }
    }
}