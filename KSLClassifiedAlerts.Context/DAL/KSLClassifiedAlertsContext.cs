using KSLClassifiedAlerts.Context.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace KSLClassifiedAlerts.Context.DAL
{
    public class KSLClassifiedAlertsContext : IdentityDbContext<ApplicationUser>
    {
        public KSLClassifiedAlertsContext() : base("KSLClassifiedAlertsContext")
        {
        }
        public KSLClassifiedAlertsContext(string connection) : base(connection)
        {

        }
        //dbsets
        public DbSet<Classified> Classifieds { get;set; }
        public DbSet<Specification> Specifications { get; set; }
        public DbSet<Search> Searches { get; set; }
        public override IDbSet<ApplicationUser> Users { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
        }

        public static KSLClassifiedAlertsContext Create()
        {
            return new KSLClassifiedAlertsContext();
        }
    }

}