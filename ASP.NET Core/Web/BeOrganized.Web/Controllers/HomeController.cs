namespace BeOrganized.Web.Controllers
{
    using System.Diagnostics;
    using System.Threading.Tasks;

    using BeOrganized.Common;
    using BeOrganized.Web.ViewModels;
    using BeOrganized.Web.ViewModels.Home;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity.UI.Services;
    using Microsoft.AspNetCore.Mvc;

    public class HomeController : BaseController
    {
        private const string EmailSendedNotification = "Your email has been sent.";
        private readonly IEmailSender emailSender;

        public HomeController(IEmailSender emailSender)
        {
            this.emailSender = emailSender;
        }

        public IActionResult Index()
        {
            if (this.User.Identity.IsAuthenticated && !this.User.IsInRole(GlobalConstants.AdministratorRoleName))
            {
                return this.RedirectToAction(nameof(this.IndexLoggedIn));
            }
            else if (this.User.Identity.IsAuthenticated && this.User.IsInRole(GlobalConstants.AdministratorRoleName))
            {
                return this.Redirect("/Administration/Dashboard");
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

        public IActionResult Contact()
        {
            return this.View();
        }

        [HttpPost]
        public async Task<IActionResult> ContactAsync(ContactViewModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }

            await this.emailSender.SendEmailAsync(GlobalConstants.SupportEmail, $"Email from {model.Name}", model.Message + $"Message send from {model.Email}");
            this.TempData["NotificationSuccess"] = EmailSendedNotification;
            return this.RedirectToAction(nameof(this.Index));
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
