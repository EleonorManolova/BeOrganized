namespace OrganizeMe.Web.Controllers
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using OrganizeMe.Data.Models.Enums;
    using OrganizeMe.Services;
    using OrganizeMe.Services.Data.Habits;
    using OrganizeMe.Web.ViewModels.Habits;

    [Authorize]
    public class HabitsController : Controller
    {
        private readonly IHabitService habitService;
        private readonly IEnumParseService enumParseService;

        public HabitsController(IHabitService habitService, IEnumParseService enumParseService)
        {
            this.habitService = habitService;
            this.enumParseService = enumParseService;
        }

        public IActionResult Index()
        {
            return this.View();
        }

        public async Task<IActionResult> CreateAsync()
        {
            var model = await this.habitService.GetHabitViewModelAsync(this.User.Identity.Name);
            return this.View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(HabitCreateViewModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }

            if (!this.enumParseService.IsEnumValid<DayTime>(model.Input.DayTime) || !this.enumParseService.IsEnumValid<Frequency>(model.Input.Frequency) || !this.enumParseService.IsEnumValid<Duration>(model.Input.Duration))
            {
                return this.View(model);
            }

            await this.habitService.CreateAsync(model.Input);
            return this.Redirect("/");
        }
    }
}
