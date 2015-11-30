using KSLClassifiedAlerts.Context.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KSLClassifiedAlerts.Context.DAL
{
    public class SeedData
    {
        private KSLClassifiedAlertsContext db;
        private UserManager<ApplicationUser> userManager;
        public SeedData(KSLClassifiedAlertsContext context, UserManager<ApplicationUser> UserManager)
        {
            db = context;
            userManager = UserManager;
        }

        public void Initialize()
        {
            CreateUsers().Wait();
            //await db.SaveChangesAsync();
        }



        private async System.Threading.Tasks.Task CreateUsers()
        {
            try
            {
                var user = await userManager.FindByEmailAsync("ztrayner@gmail.com");
                if (user == null)
                {
                    user = new ApplicationUser { UserName = "ztrayner", Email = "ztrayner@gmail.com" };
                    await userManager.CreateAsync(user, "P@SSW0RD!");
                }
            }
            catch (Exception e)
            {
                Console.Write(e.ToString());
            }

        }
    }
}