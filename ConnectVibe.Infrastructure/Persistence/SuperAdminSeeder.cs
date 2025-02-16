using Fliq.Application.Common.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;


namespace Fliq.Infrastructure.Persistence
{
    public class SuperAdminSeeder
    {
        private readonly FliqDbContext _dbContext;
        private readonly IConfiguration _configuration;

        public SuperAdminSeeder(FliqDbContext context, IConfiguration configuration)
        {
            _dbContext = context;
            _configuration = configuration;
        }

        public async Task SeedSuperAdmin()
        {
            var superAdminRole = await _dbContext.Roles.FirstOrDefaultAsync(r => r.Name == "SuperAdmin");
            if (superAdminRole == null)
            {
                return;
            }

            var existingSuperAdmin = await _dbContext.Users
                .FirstOrDefaultAsync(u => u.RoleId == superAdminRole.Id);

            if (existingSuperAdmin == null)
            {
                var firstName = _configuration["SuperAdmin:FirstName"];
                var lastName = _configuration["SuperAdmin:LastName"];
                var email = _configuration["SuperAdmin:Email"];
                var password = _configuration["SuperAdmin:Password"];

                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                {
                    throw new InvalidOperationException("SuperAdmin email and password must be configured in environment variable.");
                }
               
                var superAdmin = new Domain.Entities.User();

                superAdmin.FirstName = firstName;
                superAdmin.LastName = lastName;
                superAdmin.Email = email;
                superAdmin.PasswordSalt = PasswordSalt.Create();
                superAdmin.PasswordHash = PasswordHash.Create(password, superAdmin.PasswordSalt);
                superAdmin.RoleId = superAdminRole.Id;
                superAdmin.IsActive = true;
                superAdmin.IsEmailValidated = true;
                
                _dbContext.Users.Add(superAdmin);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
