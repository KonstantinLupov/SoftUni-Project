namespace C_Sharp_Project.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    public sealed class Configuration : DbMigrationsConfiguration<C_Sharp_Project.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
            ContextKey = "C_Sharp_Project.Models.ApplicationDbContext";
        }

        protected override void Seed(C_Sharp_Project.Models.ApplicationDbContext context)
        {
          if(!context.Roles.Any())
            {
            
            }
        }
    }
}
