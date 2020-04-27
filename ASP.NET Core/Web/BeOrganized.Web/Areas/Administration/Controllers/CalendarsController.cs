namespace BeOrganized.Web.Areas.Administration.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using BeOrganized.Services.Data.Calendar;
    using Microsoft.AspNetCore.Mvc;

    public class CalendarsController : AdministrationController
    {
        private readonly ICalendarService calendarService;

        public CalendarsController(ICalendarService calendarService)
        {
            this.calendarService = calendarService;
        }

        public IActionResult Index()
        {
            var calendars = this.calendarService.GetAll();
            return this.View(calendars);
        }

        public IActionResult Create()
        {
            var calendars = this.calendarService.GetAll();
            return this.View(calendars);
        }

        public IActionResult Edit()
        {
            var calendars = this.calendarService.GetAll();
            return this.View(calendars);
        }
    }
}