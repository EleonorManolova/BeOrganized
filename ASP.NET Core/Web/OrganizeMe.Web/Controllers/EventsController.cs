namespace OrganizeMe.Web.Controllers
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using OrganizeMe.Services.Data.Events;
    using OrganizeMe.Web.ViewModels.Events;

    [Authorize]
    public class EventsController : Controller
    {
        private const string DeleteErrorMessage = "Failed to delete the recipe.";
        private const string DeleteSuccessMessage = "You successfully deleted recipe {0} !";

        private readonly IEventService eventService;

        public EventsController(IEventService eventService)
        {
            this.eventService = eventService;
        }

        [HttpGet]
        public async Task<IActionResult> CreateAsync()
        {
            var model = await this.eventService.GetCreateViewModelAsync(this.User.Identity.Name);
            return this.View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(EventCreateViewModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }

            await this.eventService.CreateAsync(model.Input);
            return this.Redirect("/Calendar");
        }

        [HttpGet]
        public async Task<IActionResult> EditAsync(string id)
        {
            if (id == null)
            {
                return this.BadRequest();
            }

            var eventFromDb = await this.eventService.GetEditViewModelByIdAsync(id, this.User.Identity.Name);
            if (eventFromDb == null)
            {
                return this.NotFound();
            }

            this.TempData["EventId"] = id;

            return this.View(eventFromDb);
        }

        [HttpPost]
        public async Task<IActionResult> EditAsync(EventEditViewModel model)
        {
            var id = this.TempData["EventId"].ToString();
            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }

            await this.eventService.UpdateAsync(model, id);
            return this.Redirect("/Calendar");
        }

        [HttpPost]
        [Route("/Events/Delete/{id}")]
        public async Task<IActionResult> DeleteConfirmAsync(string id)
        {
            var recipeTitle = (await this.eventService.GetByIdAsync(id)).Title;

            if (!await this.eventService.DeleteAsync(id))
            {
                this.TempData["Error"] = DeleteErrorMessage;

                return this.Redirect($"/Events/Delete/{id}");
            }

            this.TempData["Success"] = string.Format(DeleteSuccessMessage, recipeTitle);

            return this.Redirect("/Calendar");
        }
    }
}
