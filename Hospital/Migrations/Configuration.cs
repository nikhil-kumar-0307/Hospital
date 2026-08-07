namespace Hospital.Migrations
{
    using Hospital.Models;
    using System.Data.Entity.Migrations;

    internal sealed class Configuration : DbMigrationsConfiguration<Hospital.Data.HospitalDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(Hospital.Data.HospitalDbContext context)
        {
            context.Users.AddOrUpdate(
                u => u.EmployeeNumber,
                new User
                {
                    EmployeeNumber = "ADMIN001",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                    Role = "Admin",
                    LastLoginAt = null
                },
                new User
                {
                    EmployeeNumber = "EMP001",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("User@123"),
                    Role = "User",
                    LastLoginAt = null
                }
            );
        }
    }
}