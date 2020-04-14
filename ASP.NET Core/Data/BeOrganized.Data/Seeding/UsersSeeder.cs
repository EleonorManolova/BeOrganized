namespace BeOrganized.Data.Seeding
{
    using System;
    using System.Threading.Tasks;

    using BeOrganized.Common;
    using BeOrganized.Data.Models;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public class UsersSeeder : ISeeder
    {
        public async Task SeedAsync(ApplicationDbContext dbContext, IServiceProvider serviceProvider)
        {
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            var email = configuration["Root:AdminEmail"];
            if (await userManager.FindByEmailAsync(email) == null)
            {
                var user = new ApplicationUser
                {
                    UserName = configuration["Root:AdminName"],
                    Email = configuration["Root:AdminEmail"],
                    EmailConfirmed = true,
                };

                IdentityResult result = await userManager.CreateAsync(user, configuration["Root:AdminPassword"]);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, GlobalConstants.AdministratorRoleName);
                }
            }
        }
    }
}
