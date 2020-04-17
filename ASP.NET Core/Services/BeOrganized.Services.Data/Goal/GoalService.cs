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
    using BeOrganized.Web.ViewModels.Golas;
    using Nest;

    public class GoalService : IGoalService
    {
        private const string InvalidPropertyErrorMessage = "One or more required properties are null.";
        private const string GoalErrorMessage = "Goal with Id {0} does not exist.";

        private readonly IEnumParseService enumParseService;
        private readonly IDeletableEntityRepository<Goal> goalRepository;
        private readonly ICalendarService calendarService;
        private readonly ISearchService searchService;
        private readonly IColorService colorService;
        private readonly IHabitService habitService;
        private List<string> dayTimesDescriptions;
        private List<string> frequenciesDescriptions;
        private List<string> durationsDescriptions;

        public GoalService(IEnumParseService enumParseService, IDeletableEntityRepository<Goal> goalRepository, ICalendarService calendarService, ISearchService searchService, IColorService colorService, IHabitService habitService)
        {
            this.enumParseService = enumParseService;
            this.goalRepository = goalRepository;
            this.calendarService = calendarService;
            this.searchService = searchService;
            this.colorService = colorService;
            this.habitService = habitService;

            this.FillEnumDescriptions();
        }

        public async Task<bool> CreateAsync(GoalViewModel goalViewModel)
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
                IsActive = true,
            };
            goal.DayTime = this.enumParseService.Parse<DayTime>(goalViewModel.DayTime);
            goal.Duration = this.enumParseService.Parse<Duration>(goalViewModel.Duration);
            goal.Frequency = this.enumParseService.Parse<Frequency>(goalViewModel.Frequency);

            var response = await this.searchService.CreateIndexAsync(goal);

            await this.goalRepository.AddAsync(goal);
            var result = await this.goalRepository.SaveChangesAsync();

            await this.habitService.GenerateHabitsAsync(goal, DateTime.Now);

            return result > 0 && response == Result.Created;
        }

        public GoalChangeViewModel GetGoalViewModel(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                throw new ArgumentException(InvalidPropertyErrorMessage);
            }

            var goalViewModel = new GoalChangeViewModel
            {
                DayTimes = this.dayTimesDescriptions,
                Frequencies = this.frequenciesDescriptions,
                Durations = this.durationsDescriptions,
                Calendars = this.calendarService.GetAllCalendarTitlesByUserName<CalendarHabitViewModel>(username),
                Colors = this.colorService.GetAllColors(),
            };

            return goalViewModel;
        }

        public async Task<bool> UpdateAsync(Goal model, string habitId)
        {
            if (model == null || string.IsNullOrEmpty(habitId))
            {
                throw new ArgumentException(InvalidPropertyErrorMessage);
            }

            this.goalRepository.Update(model);
            var result = await this.goalRepository.SaveChangesAsync();

            await this.habitService.UpdateHabitsAsync(model, habitId);

            return result > 0;
        }

        public Goal MapGoalViewModelToGoal(GoalViewModel model, string goalId)
        {
            if (model == null)
            {
                throw new ArgumentException(InvalidPropertyErrorMessage);
            }

            if (string.IsNullOrEmpty(goalId))
            {
                throw new ArgumentException(InvalidPropertyErrorMessage);
            }

            var goalFromDb = this.goalRepository.All().Where(x => x.Id == goalId).First();

            goalFromDb.Title = model.Title;
            goalFromDb.DayTime = this.enumParseService.Parse<DayTime>(model.DayTime);
            goalFromDb.Frequency = this.enumParseService.Parse<Frequency>(model.Frequency);
            goalFromDb.Duration = this.enumParseService.Parse<Duration>(model.Duration);
            goalFromDb.CalendarId = model.CalendarId;
            goalFromDb.ColorId = model.ColorId;

            return goalFromDb;
        }

        public GoalChangeViewModel GetGoalChangeViewModelById(string goalId, string username)
        {
            if (string.IsNullOrEmpty(goalId) ||
             string.IsNullOrEmpty(username))
            {
                throw new ArgumentException(InvalidPropertyErrorMessage);
            }

            if (this.goalRepository.All().Count() <= 0)
            {
                throw new ArgumentException(InvalidPropertyErrorMessage);
            }

            var goalModel = this.goalRepository.All().Where(x => x.Id == goalId).First();

            var goalViewModel = new GoalViewModel
            {
                Title = goalModel.Title,
                Frequency = this.enumParseService.GetEnumDescription(goalModel.Frequency.ToString(), typeof(Frequency)),
                DayTime = this.enumParseService.GetEnumDescription(goalModel.DayTime.ToString(), typeof(DayTime)),
                Duration = this.enumParseService.GetEnumDescription(goalModel.Duration.ToString(), typeof(Duration)),
                CalendarId = goalModel.CalendarId,
                ColorId = goalModel.ColorId,
            };

            var goalChangeViewModel = new GoalChangeViewModel
            {
                GoalModel = goalViewModel,
                DayTimes = this.dayTimesDescriptions,
                Frequencies = this.frequenciesDescriptions,
                Durations = this.durationsDescriptions,
                Calendars = this.calendarService.GetAllCalendarTitlesByUserName<CalendarHabitViewModel>(username),
                Colors = this.colorService.GetAllColors(),
            };

            return goalChangeViewModel;
        }

        public async Task<bool> DeleteAsync(string goalId)
        {
            if (string.IsNullOrEmpty(goalId))
            {
                throw new ArgumentException(InvalidPropertyErrorMessage);
            }

            var goal = this.goalRepository.All().Where(x => x.Id == goalId).First();

            if (goal == null)
            {
                throw new ArgumentException(string.Format(GoalErrorMessage, goalId));
            }

            goal.IsActive = false;
            this.goalRepository.Update(goal);
            var result = await this.goalRepository.SaveChangesAsync();

            return result > 0;
        }

        public async Task CreateMoreHabitsAsync(string calendarId)
        {
            if (string.IsNullOrEmpty(calendarId))
            {
                throw new ArgumentException(InvalidPropertyErrorMessage);
            }

            var goals = this.goalRepository
                  .All()
                  .Where(x => x.CalendarId == calendarId && x.IsActive)
                  .Distinct()
                  .ToList();

            var habits = this.goalRepository
            .All()
            .Select(x => new
            {
                StartDate = x.Habits.OrderByDescending(y => y.StartDateTime).First().StartDateTime,
                x.Habits.OrderByDescending(y => y.StartDateTime).First().GoalId,
            })
            .ToDictionary(x => x.GoalId, x => x.StartDate);

            foreach (var goal in goals)
            {
                await this.habitService.GenerateMoreHabitsAsync(goal, habits[goal.Id]);
            }
        }

        public T GetEnum<T>(string description)
        {
            if (string.IsNullOrEmpty(description))
            {
                throw new ArgumentException(InvalidPropertyErrorMessage);
            }

            return this.enumParseService.Parse<T>(description);
        }

        private void FillEnumDescriptions()
        {
            this.dayTimesDescriptions = new List<string>();
            this.frequenciesDescriptions = new List<string>();
            this.durationsDescriptions = new List<string>();

            var dayTimes = Enum.GetNames(typeof(DayTime));
            var frequencies = Enum.GetNames(typeof(Frequency));
            var durations = Enum.GetNames(typeof(Duration));

            foreach (var dayTime in dayTimes)
            {
                this.dayTimesDescriptions.Add(this.enumParseService
                    .GetEnumDescription(dayTime, typeof(DayTime)));
            }

            foreach (var frequency in frequencies)
            {
                this.frequenciesDescriptions.Add(this.enumParseService
                    .GetEnumDescription(frequency, typeof(Frequency)));
            }

            foreach (var duration in durations)
            {
                this.durationsDescriptions.Add(this.enumParseService
                    .GetEnumDescription(duration, typeof(Duration)));
            }
        }
    }
}
