namespace OrganizeMe.Services.Data.Habits
{
    using System;
    using System.Collections.Generic;

    using OrganizeMe.Data.Models.Enums;
    using OrganizeMe.Web.ViewModels.Habits;

    public class HabitService : IHabitService
    {
        private readonly IEnumParseService enumParseService;

        public HabitService(IEnumParseService enumParseService)
        {
            this.enumParseService = enumParseService;
        }

        public HabitCreateViewModel GetHabitViewModel()
        {
            var dayTimes = Enum.GetNames(typeof(DayTime));
            var frequencies = Enum.GetNames(typeof(Frequency));
            var durations = Enum.GetNames(typeof(Duration));

            var dayTimesDescriptions = new List<string>();
            var frequenciesDescriptions = new List<string>();
            var durationsDescriptions = new List<string>();
            foreach (var dayTime in dayTimes)
            {
                dayTimesDescriptions.Add(this.enumParseService
                    .GetEnumDescription(dayTime, typeof(DayTime)));
            }

            foreach (var frequency in frequencies)
            {
                frequenciesDescriptions.Add(this.enumParseService
                    .GetEnumDescription(frequency, typeof(Frequency)));
            }

            foreach (var duration in durations)
            {
                durationsDescriptions.Add(this.enumParseService
                    .GetEnumDescription(duration, typeof(Duration)));
            }

            var habitViewModel = new HabitCreateViewModel
            {
                Input = new HabitInputViewModel(),
                DayTimes = dayTimesDescriptions,
                Frequencies = frequenciesDescriptions,
                Durations = durationsDescriptions,
            };

            return habitViewModel;
        }

        public T GetEnum<T>(string description)
        {
            return this.enumParseService.Parse<T>(description);
        }
    }
}
