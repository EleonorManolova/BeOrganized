namespace BeOrganized.Web.Areas.Administration.Controllers
{
    using System.Linq;
    using System.Threading.Tasks;

    using BeOrganized.Common;
    using BeOrganized.Data.Models;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;

    public class UsersController : AdministrationController
    {
        private const string InvalidUserIdErrorMessage = "User with Id = {0} cannot be found.";

        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<ApplicationRole> roleManager;

        public UsersController(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        public IActionResult All()
        {
            return this.View(this.userManager.Users);
        }

        public async Task<IActionResult> ManageAdministration()
        {
            var role = await this.roleManager.Roles.SingleAsync(r => r.Name == GlobalConstants.AdministratorRoleName);
            var admins = this.userManager.Users
                .Where(x => x.Roles.Any(r => r.RoleId == role.Id))
                .OrderBy(x => x.CreatedOn)
                .ToList();
            var users = this.userManager.Users
                .Where(x => !admins.Contains(x))
                .ToList();
            admins.AddRange(users);

            return this.View(admins);
        }

        public async Task<IActionResult> Edit(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return this.BadRequest();
            }

            var user = await this.userManager.FindByIdAsync(userId);
            this.TempData["UserId"] = user.Id;
            return this.View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ApplicationUser user, string userId)
        {
            try
            {
                var model = await this.userManager.FindByIdAsync(userId);
                model.FullName = user.FullName;
                model.UserName = user.UserName;
                model.Email = user.Email;
                model.IsDeleted = user.IsDeleted;
                model.EmailConfirmed = user.EmailConfirmed;
                model.PhoneNumber = user.PhoneNumber;
                model.TwoFactorEnabled = user.TwoFactorEnabled;
                await this.userManager.UpdateAsync(model);
                return this.RedirectToAction(nameof(this.All));
            }
            catch
            {
                return this.View(user);
            }
        }

        [Route("/Administration/Users/Delete/{userId}")]
        public async Task<IActionResult> Delete(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return this.BadRequest();
            }

            var user = await this.userManager.FindByIdAsync(userId);
            return this.View(user);
        }

        [HttpPost]
        [Route("/Administration/Users/Delete/{userId}")]
        public async Task<IActionResult> DeleteConfirm(string userId)
        {
            try
            {
                var user = await this.userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    this.ViewBag.ErrorMessage = string.Format(InvalidUserIdErrorMessage, userId);
                }

                user.IsDeleted = true;
                await this.userManager.UpdateAsync(user);
                return this.RedirectToAction(nameof(this.All));
            }
            catch
            {
                return this.RedirectToAction(nameof(this.All));
            }
        }

        [Route("/Administration/Users/MakeAdmin/{userId}")]
        public async Task<IActionResult> MakeAdmin(string userId)
        {
            var user = await this.userManager.FindByIdAsync(userId);
            return this.View(user);
        }

        [HttpPost]
        [Route("/Administration/Users/MakeAdmin/{userId}")]
        public async Task<IActionResult> MakeAdminConfirm(string userId)
        {
            var user = await this.userManager.FindByIdAsync(userId);
            var adminRole = await this.roleManager.Roles.SingleAsync(r => r.Name == GlobalConstants.AdministratorRoleName);
            if (user.IsDeleted || user.Roles.Any(x => x.RoleId == adminRole.Id))
            {
                return this.RedirectToAction(nameof(this.ManageAdministration));
            }

            var identityRole = new IdentityUserRole<string>()
            {
                RoleId = adminRole.Id,
                UserId = user.Id,
            };
            user.Roles.Add(identityRole);
            var result = await this.userManager.UpdateAsync(user);

            return this.RedirectToAction(nameof(this.ManageAdministration));
        }

        [Route("/Administration/Users/RemoveAdmin/{userId}")]
        public async Task<IActionResult> RemoveAdmin(string userId)
        {
            var user = await this.userManager.FindByIdAsync(userId);
            return this.View(user);
        }

        [HttpPost]
        [Route("/Administration/Users/RemoveAdmin/{userId}")]
        public async Task<IActionResult> RemoveAdminConfirm(string userId)
        {
            var user = await this.userManager.FindByIdAsync(userId);
            var adminRole = await this.roleManager.Roles.SingleAsync(r => r.Name == GlobalConstants.AdministratorRoleName);
            if (user.IsDeleted || user.Roles.Any(x => x.RoleId == adminRole.Id))
            {
                return this.RedirectToAction(nameof(this.ManageAdministration));
            }

            await this.userManager.RemoveFromRoleAsync(user, GlobalConstants.AdministratorRoleName);
            await this.userManager.UpdateAsync(user);

            return this.RedirectToAction(nameof(this.ManageAdministration));
        }
    }
}
