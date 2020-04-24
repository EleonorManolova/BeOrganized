namespace BeOrganized.Web.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

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

        // [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
        [HttpPost]
        [Route("/Search/ReIndex")]
        public async Task<IActionResult> ReIndex()
        {
            await this.elasticClient.DeleteByQueryAsync<Event>(q => q.MatchAll());

            var allEvents = this.eventService.GetAllByUsername(this.User.Identity.Name).ToArray();

            foreach (var eventFromDb in allEvents)
            {
                await this.elasticClient.IndexDocumentAsync(eventFromDb);
            }

            return this.Ok($"{allEvents.Length} post(s) reindexed");
        }

        [Route("/Search")]
        public async Task<IActionResult> Find(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                var model = new List<EventSearchViewModel>();
                return this.View("Results", model);
            }

            //var response = await this.elasticClient.SearchAsync<Event>(s => s
            //    .Query(qx => qx
            //        .MultiMatch(m => m
            //                .Query(query.ToLower())
            //                .Fields(ff => ff
            //                    .Field(f => f.Title, boost: 15)
            //                    .Field(f => f.Location, boost: 10)))));
            var calendarId = this.calendarService.GetDefaultCalendarId(this.User.Identity.Name);

            var response = await this.elasticClient.SearchAsync<Event>(s => s
          .Query(q => q
               .Bool(x => x
                   .Should(sh => sh

                        .Match(t => t.Field(f => f.Title.ToLower()).Query(query.ToLower())))
                   .Should(sh => sh
                   .Match(t => t.Field(f => f.CalendarId).Query(calendarId))))));

           // var response = await this.elasticClient.SearchAsync<Event>(s => s
           //.Query(q => q
           //     .Bool(x => x
           //         .Should(sh => new QueryContainer
           //         {
           //             sh.Match(t => t.Field(f => f.Title.ToLower()).Query(query.ToLower())),
           //             sh.Match(t => t.Field(f => f.CalendarId).Query(calendarId)),
           //         }))));
            if (!response.IsValid)
            {
                // We could handle errors here by checking response.OriginalException or response.ServerError properties
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
