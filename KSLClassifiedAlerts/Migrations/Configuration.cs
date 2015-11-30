using KSLClassifiedAlerts.Context.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;

namespace KSLClassifiedAlerts.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<KSLClassifiedAlerts.Context.DAL.KSLClassifiedAlertsContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(KSLClassifiedAlerts.Context.DAL.KSLClassifiedAlertsContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //

            //Uncomment these lines to debug
            //if (System.Diagnostics.Debugger.IsAttached == false)
            //    System.Diagnostics.Debugger.Launch();

            if (context.Users.Count() == 0)
            {
                var appContext = new KSLClassifiedAlerts.Context.DAL.KSLClassifiedAlertsContext();

                var user = new ApplicationUser
                {
                    UserName = "ztrayner@gmail.com",
                    Email = "ztrayner@gmail.com",
                    FirstName = "Zach",
                    LastName = "Trayner"
                };

                var user2 = new ApplicationUser
                {
                    UserName = "tester@test.com",
                    Email = "tester@test.com",
                    FirstName = "test",
                    LastName = "tester"
                };

                ApplicationUserManager userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(context));

                userManager.Create(user, "P4SSW0RD!");
                userManager.Create(user2, "P4SSW0RD!");
            }
        }
    }

}