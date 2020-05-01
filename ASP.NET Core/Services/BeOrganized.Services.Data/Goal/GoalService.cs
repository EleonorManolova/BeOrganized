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
    using BeOrganized.Web.ViewModels.Administration.Goals;
    using BeOrganized.Web.ViewModels.Golas;
    using Nest;

    public class GoalService : IGoalService
    {
        private const string InvalidPropertyErrorMessage = "One or more required properties are null.";
        private const string GoalErrorMessage = "Goal with Id: {0} does not exist.";

        private readonly IEnumParseService enumParseService;
        private readonly IDeletableEntityRepository<Goal> goalRepository;
        private readonly ICalendarService calendarService;
        private readonly IColorService colorService;
        private readonly IHabitService habitService;
        private List<string> dayTimesDescriptions;
        private List<string> frequenciesDescriptions;
        private List<string> durationsDescriptions;

        public GoalService(IDeletableEntityRepository<Goal> goalRepository, IEnumParseService enumParseService, ICalendarService calendarService, IColorService colorService, IHabitService habitService)
        {
            this.enumParseService = enumParseService;
            this.goalRepository = goalRepository;
            this.calendarService = calendarService;
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

            await this.goalRepository.AddAsync(goal);
            var result = await this.goalRepository.SaveChangesAsync();

            await this.habitService.GenerateHabitsAsync(goal, goal.StartDateTime);

            return result > 0;
        }

        public GoalChangeViewModel GetGoalChangeViewModel(string username)
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

            var goal = this.goalRepository.All().Where(x => x.Id == goalId).FirstOrDefault();

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

            var habits1 = this.goalRepository.All().Where(x => x.IsActive).Select(x => x.Id).ToList();

            var habits = this.goalRepository
            .All()
            .Where(x => x.IsActive)
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

        public ICollection<GoalDetailsModel> GetDetailsViewModels()
        {
            return this.goalRepository
               .All()
               .Select(x => new GoalDetailsModel
               {
                   Id = x.Id,
                   Title = x.Title,
                   Frequency = x.Frequency,
                   DayTime = x.DayTime,
                   Duration = x.Duration,
                   StartDateTime = x.StartDateTime,
                   IsActive = x.IsActive,
                   ColorName = x.Color.Name,
                   CreatedOn = x.CreatedOn,
               })
               .ToList();
        }

        public GoalChangeModel GetCreateViewModel()
        {
            var model = new GoalChangeModel
            {
                GoalModel = new GoalModel
                {
                    StartDateTime = DateTime.Now,
                },
                DayTimes = this.dayTimesDescriptions,
                Frequencies = this.frequenciesDescriptions,
                Durations = this.durationsDescriptions,
                Calendars = this.calendarService.GetAll(),
                Colors = this.colorService.GetAllColors(),
            };

            return model;
        }

        public async Task<bool> CreateFromAdminAsync(GoalModel goalModel)
        {
            if (string.IsNullOrEmpty(goalModel.Title) ||
              string.IsNullOrEmpty(goalModel.CalendarId) ||
              goalModel.ColorId <= 0)
            {
                throw new ArgumentException(InvalidPropertyErrorMessage);
            }

            var goal = new Goal
            {
                Title = goalModel.Title,
                StartDateTime = goalModel.StartDateTime,
                IsActive = true,
                CalendarId = goalModel.CalendarId,
                ColorId = goalModel.ColorId,
            };

            goal.DayTime = this.enumParseService.Parse<DayTime>(goalModel.DayTime);
            goal.Duration = this.enumParseService.Parse<Duration>(goalModel.Duration);
            goal.Frequency = this.enumParseService.Parse<Frequency>(goalModel.Frequency);

            await this.goalRepository.AddAsync(goal);
            var result = await this.goalRepository.SaveChangesAsync();

            await this.habitService.GenerateHabitsAsync(goal, goal.StartDateTime);

            return result > 0;
        }

        public GoalChangeModel GetEditChangeViewModelById(string goalId)
        {
            if (string.IsNullOrEmpty(goalId))
            {
                throw new ArgumentException(InvalidPropertyErrorMessage);
            }

            var goal = this.goalRepository.All().Where(x => x.Id == goalId).To<GoalModel>().First();
            var goalViewModel = new GoalModel
            {
                Title = goal.Title,
                Frequency = this.enumParseService.GetEnumDescription(goal.Frequency.ToString(), typeof(Frequency)),
                DayTime = this.enumParseService.GetEnumDescription(goal.DayTime.ToString(), typeof(DayTime)),
                Duration = this.enumParseService.GetEnumDescription(goal.Duration.ToString(), typeof(Duration)),
                CalendarId = goal.CalendarId,
                ColorId = goal.ColorId,
            };

            var goalChangeViewModel = new GoalChangeModel
            {
                GoalModel = goalViewModel,
                DayTimes = this.dayTimesDescriptions,
                Frequencies = this.frequenciesDescriptions,
                Durations = this.durationsDescriptions,
                Calendars = this.calendarService.GetAll(),
                Colors = this.colorService.GetAllColors(),
            };

            return goalChangeViewModel;
        }

        public Goal MapGoalModelToGoal(GoalModel model, string goalId)
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

        public async Task<Goal> GetByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException(InvalidPropertyErrorMessage);
            }

            var calendar = await this.goalRepository.GetByIdWithDeletedAsync(id);

            if (calendar == null)
            {
                throw new ArgumentException(string.Format(GoalErrorMessage, id));
            }

            return calendar;
        }

        public async Task<bool> UpdateFromAdminAsync(Goal model)
        {
            if (model == null)
            {
                throw new ArgumentException(InvalidPropertyErrorMessage);
            }

            this.goalRepository.Update(model);
            var result = await this.goalRepository.SaveChangesAsync();
            var habit = this.habitService.GetAllByCalendarId(model.CalendarId)
                .OrderBy(x => x.StartDateTime)
                .First();
            await this.habitService.UpdateHabitsAsync(model, habit.Id);

            return result > 0;
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
