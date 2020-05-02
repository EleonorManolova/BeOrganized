namespace BeOrganized.Web.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using BeOrganized.Common;
    using BeOrganized.Data.Models;
    using BeOrganized.Services.Data.Calendar;
    using BeOrganized.Services.Data.Color;
    using BeOrganized.Services.Data.Events;
    using BeOrganized.Web.ViewModels.Search;
    using Microsoft.AspNetCore.Mvc;
    using Nest;

    public class SearchController : BaseController
    {
        private readonly IEventService eventService;
        private readonly IElasticClient elasticClient;
        private readonly IColorService colorService;
        private readonly ICalendarService calendarService;

        public SearchController(IEventService eventService, IElasticClient elasticClient, IColorService colorService, ICalendarService calendarService)
        {
            this.eventService = eventService;
            this.elasticClient = elasticClient;
            this.colorService = colorService;
            this.calendarService = calendarService;
        }

        [Route("/Search")]
        public async Task<IActionResult> Find(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                var model = new List<EventSearchViewModel>();
                return this.View("Results", model);
            }

            var calendarId = this.calendarService.GetCalendarId(this.User.Identity.Name, GlobalConstants.DefaultCalendarTitle);

            var response = await this.elasticClient.SearchAsync<Event>(s => s
          .Query(q => q
               .Bool(x => x
                   .Must(sh => sh
                   .Match(t => t.Field(f => f.Title.ToLower()).Query(query.ToLower())))
                   .Filter(sh => sh
                   .Match(t => t.Field(f => f.CalendarId).Query(calendarId))))));


            if (!response.IsValid)
            {
                return this.View("Results", new EventSearchViewModel[] { });
            }

            var sortedResult = response.Documents
                .Where(x => x.IsDeleted != true)
                .Select(x => new EventSearchViewModel
                {
                    Id = x.Id,
                    StartDateTime = x.StartDateTime,
                    EndDateTime = x.EndDateTime,
                    ColorHex = this.colorService.GetColorHex(x.ColorId),
                    Title = x.Title,
                })
                .OrderBy(x => x.StartDateTime)
                .ToList();
            return this.View("Results", sortedResult);
        }
    }
}
