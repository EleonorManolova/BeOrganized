namespace BeOrganized.Web.ViewModels.Search
{
    using System;

    using BeOrganized.Data.Models;
    using BeOrganized.Services.Mapping;

    public class EventSearchViewModel : IMapFrom<Event>
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public DateTime StartDateTime { get; set; }

        public DateTime EndDateTime { get; set; }

        public string ColorHex { get; set; }
    }
}
