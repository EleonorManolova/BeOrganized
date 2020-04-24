namespace BeOrganized.Web.Areas.Administration.Controllers
{
    using BeOrganized.Services.Data;
    using BeOrganized.Web.ViewModels.Administration.Dashboard;
    using Microsoft.AspNetCore.Mvc;

    public class DashboardController : AdministrationController
    {
        private readonly ISettingsService settingsService;

        public DashboardController(ISettingsService settingsService)
        {
            this.settingsService = settingsService;
        }

        public IActionResult Index()
        {
            var viewModel = new IndexViewModel
            {
                SettingsCount = this.settingsService.GetCount(),
                UserName = this.User.Identity.Name,
                UniqueVisitorsCount = 5,
            };
            return this.View();
        }
    }
}
