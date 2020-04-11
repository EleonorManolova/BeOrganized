namespace BeOrganized.Web.Controllers
{
    using BeOrganized.Common;
    using BeOrganized.Data.Models;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    [Route("Identity/[controller]/[action]")]
    public class UsersController : BaseController
    {
        private readonly UserManager<ApplicationUser> userManager;

        public UsersController(UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
        }

        [AcceptVerbs("GET", "POST")]
        public IActionResult VerifyEmail([FromQuery(Name = "Input.Email")] string email)
        {
            var user = this.userManager.FindByEmailAsync(email);
            if (user.Result != null)
            {
                return this.Json(string.Format(ErrorMessages.InvalidEmailErrorMessage, email));
            }

            return this.Json(true);
        }

        [AcceptVerbs("GET", "POST")]
        public IActionResult VerifyUsername([FromQuery(Name = "Input.UserName")] string username)
        {
            var user = this.userManager.FindByNameAsync(username);
            if (user.Result != null)
            {
                return this.Json(string.Format(ErrorMessages.InvalidUsernameErrorMessage, username));
            }

            return this.Json(true);
        }
    }
}
