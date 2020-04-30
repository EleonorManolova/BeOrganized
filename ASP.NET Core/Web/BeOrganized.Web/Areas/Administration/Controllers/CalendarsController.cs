// <copyright file="CalendarsController.cs" company="BeOrganized">
// Copyright (c) BeOrganized. All Rights Reserved.
// </copyright>

namespace BeOrganized.Web.Areas.Administration.Controllers
{
    using System.Linq;
    using System.Threading.Tasks;

    using BeOrganized.Data.Models;
    using BeOrganized.Services.Data.Calendar;
    using BeOrganized.Web.ViewModels.Administration.Calendars;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    public class CalendarsController : AdministrationController
    {
        private const string DeleteErrorMessage = "Failed to delete the calendar.";
        private const string DeleteSuccessMessage = "You successfully deleted calendar {0}!";

        private readonly ICalendarService calendarService;
        private readonly UserManager<ApplicationUser> userManager;

        public CalendarsController(ICalendarService calendarService, UserManager<ApplicationUser> userManager)
        {
            this.calendarService = calendarService;
            this.userManager = userManager;
        }

        public IActionResult Index()
        {
            var calendars = this.calendarService.GetDetailsViewModels().OrderByDescending(x => x.CreatedOn).ToList();
            return this.View(calendars);
        }

        public IActionResult Create()
        {
            var model = this.calendarService.GetCreateViewModel();
            model.Users = this.userManager.Users.ToList();
            return this.View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(CalendarChangeModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }

            await this.calendarService.CreateFromAdminAsync(model.CalendarModel);
            return this.RedirectToAction(nameof(this.Index));
        }

        public IActionResult Edit(string calendarId)
        {
            if (string.IsNullOrEmpty(calendarId))
            {
                return this.BadRequest();
            }

            var calendar = this.calendarService.GetEditChangeViewModelById(calendarId);
            if (calendar == null)
            {
                return this.NotFound();
            }
            calendar.Users = this.userManager.Users.ToList();
            this.TempData["CalendarId"] = calendarId;

            return this.View(calendar);
        }

        [HttpPost]
        public async Task<IActionResult> EditAsync(CalendarChangeModel model, string id)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }

            var calendarModel = this.calendarService.MapCalendarViewModelToCalendar(model.CalendarModel, id);
            await this.calendarService.UpdateAsync(calendarModel, id);
            return this.RedirectToAction(nameof(this.Index));
        }

        [Route("/Administration/Calendars/Delete/{calendarId}")]
        public async Task<IActionResult> Delete(string calendarId)
        {
            if (!this.ModelState.IsValid)
            {
                return this.RedirectToAction(nameof(this.Index));
            }

            var calendar = await this.calendarService.GetByIdAsync(calendarId);
            return this.View(calendar);
        }

        [HttpPost]
        [Route("/Administration/Calendars/Delete/{calendarId}")]
        public async Task<IActionResult> DeleteAsync(string calendarId)
        {
            var eventTitle = (await this.calendarService.GetByIdAsync(calendarId)).Title;

            if (!await this.calendarService.DeleteAsync(calendarId))
            {
                this.TempData["NotificationError"] = DeleteErrorMessage;
                this.View();
            }

            this.TempData["NotificationSuccess"] = string.Format(DeleteSuccessMessage, eventTitle);

            return this.RedirectToAction(nameof(this.Index));
        }
    }
}