namespace ODCenter.Migrations.User
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Microsoft.AspNet.Identity.Owin;
    using ODCenter.Base;
    using ODCenter.Controllers;
    using ODCenter.Models;
    using System;
    using System.Data.Entity.Migrations;
    using System.Web;

    internal sealed class Configuration : DbMigrationsConfiguration<ODCenter.Models.UserDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
            MigrationsDirectory = @"Migrations\User";
            ContextKey = "ODCenter.Models.ApplicationDbContext";
        }

        protected override void Seed(ODCenter.Models.UserDbContext context)
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
            var userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(context));
            var roleManager = new ApplicationRoleManager(new RoleStore<IdentityRole>(context));

            //var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            //var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

            //var userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            //var roleManager = HttpContext.Current.GetOwinContext().Get<ApplicationRoleManager>();

            //Create Roles
            foreach (String role in UserRoles.All())
            {
                if (roleManager.FindByName(role) == null)
                {
                    roleManager.Create(new IdentityRole(role));
                }
            }

            //Authorize
            ApplicationUser user = userManager.FindByName("admin@oriwater.cn");
            if (user != null)
            {
                if (!userManager.IsInRole(user.Id, UserRoles.Admin))
                {
                    userManager.AddToRole(user.Id, UserRoles.Admin);
                }
            }
        }
    }
}
