namespace BeOrganized.Web.ViewModels.Calendar
{
    using System;
    using System.Text.Json.Serialization;

    using BeOrganized.Data.Models;
    using BeOrganized.Services.Mapping;

    public class EventCalendarViewModel : IMapFrom<Event>
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("start")]
        public DateTime StartDateTime { get; set; }

        [JsonPropertyName("end")]
        public DateTime EndDateTime { get; set; }

        public string Location { get; set; }

        public string Coordinates { get; set; }

        public string Description { get; set; }

        public string CalendarId { get; set; }

        [JsonPropertyName("color")]
        public string ColorHex { get; set; }
    }
}
