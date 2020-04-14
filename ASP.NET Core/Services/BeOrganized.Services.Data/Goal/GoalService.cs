namespace BeOrganized.Services.Data.Goal
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using BeOrganized.Data.Common.Repositories;
    using BeOrganized.Data.Models;
    using BeOrganized.Data.Models.Enums;
    using BeOrganized.Services;
    using BeOrganized.Services.Data.Calendar;
    using BeOrganized.Services.Data.Color;
    using BeOrganized.Services.Data.Habit;
    using BeOrganized.Services.Mapping;
    using BeOrganized.Web.ViewModels.Calendar;
    using BeOrganized.Web.ViewModels.Golas;
    using Nest;

    public class GoalService : IGoalService
    {
        private const string InvalidPropertyErrorMessage = "One or more required properties are null.";

        private readonly IEnumParseService enumParseService;
        private readonly IDeletableEntityRepository<Goal> goalsRepository;
        private readonly ICalendarService calendarService;
        private readonly ISearchService searchService;
        private readonly IColorService colorService;
        private readonly IHabitService habitService;

        public GoalService(IEnumParseService enumParseService, IDeletableEntityRepository<Goal> goalsRepository, ICalendarService calendarService, ISearchService searchService, IColorService colorService, IHabitService habitService)
        {
            this.enumParseService = enumParseService;
            this.goalsRepository = goalsRepository;
            this.calendarService = calendarService;
            this.searchService = searchService;
            this.colorService = colorService;
            this.habitService = habitService;
        }

        public async Task<bool> CreateAsync(GoalInputViewModel goalViewModel)
        {
            if (string.IsNullOrEmpty(goalViewModel.Title) ||
               string.IsNullOrEmpty(goalViewModel.CalendarId))
            {
                throw new ArgumentException(InvalidPropertyErrorMessage);
            }

            var goal = new Goal
            {
                Title = goalViewModel.Title,
                CalendarId = goalViewModel.CalendarId,
                ColorId = goalViewModel.ColorId,
                StartDateTime = DateTime.Now,
            };
            goal.DayTime = this.enumParseService.Parse<DayTime>(goalViewModel.DayTime);
            goal.Duration = this.enumParseService.Parse<Duration>(goalViewModel.Duration);
            goal.Frequency = this.enumParseService.Parse<Frequency>(goalViewModel.Frequency);

            var response = await this.searchService.CreateIndexAsync(goal);

            await this.goalsRepository.AddAsync(goal);
            var result = await this.goalsRepository.SaveChangesAsync();

            await this.habitService.GenerateHabitsInitialAsync(goal);

            return result > 0 && response == Result.Created;
        }

        public GoalCreateViewModel GetGoalViewModel(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                throw new ArgumentException(InvalidPropertyErrorMessage);
            }

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

            var goalViewModel = new GoalCreateViewModel
            {
                DayTimes = dayTimesDescriptions,
                Frequencies = frequenciesDescriptions,
                Durations = durationsDescriptions,
                Calendars = this.calendarService.GetAllCalendarTitlesByUserId<CalendarHabitViewModel>(username),
                Colors = this.colorService.GetAllColors(),
            };

            return goalViewModel;
        }

        public T GetEnum<T>(string description)
        {
            if (string.IsNullOrEmpty(description))
            {
                throw new ArgumentException(InvalidPropertyErrorMessage);
            }

            return this.enumParseService.Parse<T>(description);
        }
    }
}
