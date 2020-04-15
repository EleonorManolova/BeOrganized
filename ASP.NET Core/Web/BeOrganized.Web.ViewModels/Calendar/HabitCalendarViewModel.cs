namespace BeOrganized.Web.ViewModels.Calendar
{
    using System;
    using System.Text.Json.Serialization;

    using BeOrganized.Data.Models;
    using BeOrganized.Services.Mapping;

    public class HabitCalendarViewModel : IMapFrom<Habit>
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("start")]
        public DateTime StartDateTime { get; set; }

        [JsonPropertyName("end")]
        public DateTime EndDateTime { get; set; }

        [JsonPropertyName("iscompleted")]
        public bool IsCompleted { get; set; }

        public string GoalCalendarId { get; set; }

        [JsonPropertyName("color")]
        public string GoalColorHex { get; set; }

        [JsonPropertyName("durationEditable")]
        public bool DurationEditable => false;
    }
}
