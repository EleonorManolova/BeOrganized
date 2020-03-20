namespace OrganizeMe.Services.Data.Habits
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using OrganizeMe.Data.Common.Repositories;
    using OrganizeMe.Data.Models;
    using OrganizeMe.Data.Models.Enums;
    using OrganizeMe.Services.Data.Calendar;
    using OrganizeMe.Web.ViewModels.Habits;

    public class HabitService : IHabitService
    {
        private readonly IEnumParseService enumParseService;
        private readonly IDeletableEntityRepository<Habit> habitsRepository;
        private readonly ICalendarService calendarService;

        public HabitService(IEnumParseService enumParseService, IDeletableEntityRepository<Habit> habitsRepository, ICalendarService calendarService)
        {
            this.enumParseService = enumParseService;
            this.habitsRepository = habitsRepository;
            this.calendarService = calendarService;
        }

        public async Task<bool> CreateAsync(HabitInputViewModel habitViewModel)
        {
            var habit = new Habit
            {
                Title = habitViewModel.Title,
                CalendarId = habitViewModel.CalendarId,
                IsCompleted = false,
            };
            habit.DayTime = this.enumParseService.Parse<DayTime>(habitViewModel.DayTime);
            habit.Duration = this.enumParseService.Parse<Duration>(habitViewModel.Duration);
            habit.Frequency = this.enumParseService.Parse<Frequency>(habitViewModel.Frequency);

            await this.habitsRepository.AddAsync(habit);
            await this.habitsRepository.SaveChangesAsync();
            return true;
        }

        public HabitCreateViewModel GetHabitViewModel(string username)
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
                DayTimes = dayTimesDescriptions,
                Frequencies = frequenciesDescriptions,
                Durations = durationsDescriptions,
                Calendars = this.calendarService.GetAllCalendarTitlesByUserId<CalendarHabitViewModel>(username),
            };

            return habitViewModel;
        }

        public T GetEnum<T>(string description)
        {
            return this.enumParseService.Parse<T>(description);
        }
    }
}
