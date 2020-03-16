namespace OrganizeMe.Data.Seeding
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using OrganizeMe.Common;
    using OrganizeMe.Data.Models;

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
