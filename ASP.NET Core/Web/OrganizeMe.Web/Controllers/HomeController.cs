namespace OrganizeMe.Web.Controllers
{
    using System.Diagnostics;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using OrganizeMe.Common;
    using OrganizeMe.Web.ViewModels;

    public class HomeController : BaseController
    {
        public IActionResult Index()
        {
            if (this.User.Identity.IsAuthenticated && !this.User.IsInRole(GlobalConstants.AdministratorRoleName))
            {
                return this.RedirectToAction(nameof(this.IndexLoggedIn));
            }

            return this.View();
        }

        [Authorize]
        [Route("/Home/Index")]
        public IActionResult IndexLoggedIn()
        {
            return this.Redirect("/Calendar");
        }

        public IActionResult Privacy()
        {
            return this.View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return this.View(
                new ErrorViewModel { RequestId = Activity.Current?.Id ?? this.HttpContext.TraceIdentifier });
        }

        public IActionResult HttpError(int statusCode)
        {
            if (statusCode == 404)
            {
                return this.View("404", statusCode);
            }

            return this.View("Error");
        }
    }
}
