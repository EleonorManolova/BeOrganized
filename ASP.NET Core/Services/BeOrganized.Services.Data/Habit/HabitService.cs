namespace BeOrganized.Services.Data.Habit
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using BeOrganized.Data.Common.Repositories;
    using BeOrganized.Data.Models;
    using BeOrganized.Data.Models.Enums;
    using BeOrganized.Services;
    using BeOrganized.Services.Mapping;
    using BeOrganized.Web.ViewModels.Calendar;
    using BeOrganized.Web.ViewModels.Habits;

    public class HabitService : IHabitService
    {
        private const string InvalidHabitIdErrorMessage = "Habit with Id: {0} does not exist.";
        private const string InvaliCalendarIdErrorMessage = "Calendar with Id: {0} does not exist.";
        private const string InvalidGoalModelErrorMessage = "Goal does not exist.";
        private const string InvalidPropertyErrorMessage = "One or more required properties are null.";

        private readonly IDeletableEntityRepository<Habit> habitRepository;
        private readonly IDateTimeService dateTimeService;
        private readonly IEnumParseService enumParseService;

        public HabitService(IDeletableEntityRepository<Habit> habitRepository, IDateTimeService dateTimeService, IEnumParseService enumParseService)
        {
            this.habitRepository = habitRepository;
            this.dateTimeService = dateTimeService;
            this.enumParseService = enumParseService;
        }

        public ICollection<HabitCalendarViewModel> GetAllByCalendarId(string calendarId)
        {
            if (string.IsNullOrEmpty(calendarId))
            {
                throw new ArgumentException(string.Format(InvaliCalendarIdErrorMessage, calendarId));
            }

            var habits = this.habitRepository
                .All()
                .Where(x => x.Goal.CalendarId == calendarId)
                .To<HabitCalendarViewModel>()
                .ToList();

            return habits;
        }

        public async Task<Habit> GetByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException(InvalidPropertyErrorMessage);
            }

            var habit = await this.habitRepository.GetByIdWithDeletedAsync(id);

            if (habit == null)
            {
                throw new ArgumentException(string.Format(InvalidHabitIdErrorMessage, id));
            }

            return habit;
        }

        public async Task<bool> GenerateHabitsAsync(Goal goal, DateTime currentDate)
        {
            if (goal == null)
            {
                throw new ArgumentException(InvalidGoalModelErrorMessage);
            }

            var startEndDateTimes = this.dateTimeService.GenerateDatesForMonthAhead((int)goal.Duration, (int)goal.Frequency, goal.DayTime.ToString(), currentDate);

            foreach (var time in startEndDateTimes)
            {
                var habit = new Habit()
                {
                    Title = goal.Title,
                    StartDateTime = time.Start,
                    EndDateTime = time.End,
                    Goal = goal,
                };

                await this.habitRepository.AddAsync(habit);
            }

            var result = await this.habitRepository.SaveChangesAsync();

            return result > 0;
        }

        public async Task<bool> GenerateMoreHabitsAsync(Goal goal, DateTime currentDate)
        {
            if (goal == null)
            {
                throw new ArgumentException(InvalidGoalModelErrorMessage);
            }

            var lastGeneratedWeek = this.dateTimeService.FirstDayOfWeekAfhterMonth(DateTime.Now);

            // Add days to start from new week, not from given date
            var firstDayOfWeek = this.dateTimeService.FirstDayOfWeek(currentDate);
            if (lastGeneratedWeek == firstDayOfWeek)
            {
                return false;
            }

            var firstDayOfNextWeek = firstDayOfWeek.AddDays(7);
            var result = await this.GenerateHabitsAsync(goal, firstDayOfNextWeek);

            return result;
        }

        public HabitDetailsViewModel GetDetailsViewModelById(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException(string.Format(InvalidHabitIdErrorMessage, id));
            }

            if (this.habitRepository.All().Count() <= 0)
            {
                return new HabitDetailsViewModel();
            }

            var goalAndStartDate = this.habitRepository.All()
                .Where(x => x.Id == id)
                .Select(x => new
                {
                    x.StartDateTime,
                    x.GoalId,
                    GoalTime = x.Goal.StartDateTime,
                })
                .First();
            var currentMonday = this.dateTimeService.FirstDayOfWeek(goalAndStartDate.StartDateTime);

            var completedHabitsForWeeks = new Dictionary<string, int>();
            if (this.dateTimeService.FirstDayOfWeek(goalAndStartDate.GoalTime).Date != currentMonday.Date)
            {
                completedHabitsForWeeks = this.FindCompletedHabitsForWeeks(goalAndStartDate.StartDateTime, goalAndStartDate.GoalId);
            }

            var habit = this.habitRepository
                .All()
                .Where(x => x.Id == id)
                .Select(x => new HabitDetailsViewModel
                {
                    Id = x.Id,
                    Title = x.Title,
                    StartDateTime = x.StartDateTime,
                    EndDateTime = x.EndDateTime,
                    GoalCalendarTitle = x.Goal.Calendar.Title,
                    GoalColorHex = x.Goal.Color.Hex,
                    GoalDayTime = x.Goal.DayTime.ToString(),
                    GoalDuration = x.Goal.Duration.ToString(),
                    GoalFrequency = x.Goal.Frequency.ToString(),
                    GoalFrequencyInt = this.dateTimeService.FindFrequency((int)x.Goal.Frequency),
                    GoalId = x.GoalId,
                    IsCompleted = x.IsCompleted,
                    CompletedHabitsForWeeks = completedHabitsForWeeks,
                })
                .First();

            habit.GoalFrequency = this.enumParseService.GetEnumDescription(habit.GoalFrequency, typeof(Frequency));
            habit.GoalDuration = this.enumParseService.GetEnumDescription(habit.GoalDuration, typeof(Duration));
            habit.GoalDayTime = this.enumParseService.GetEnumDescription(habit.GoalDayTime, typeof(DayTime));

            return habit;
        }

        public async Task<bool> UpdateHabitsAsync(Goal goal, string habitId)
        {
            if (string.IsNullOrEmpty(habitId))
            {
                throw new ArgumentException(string.Format(InvalidHabitIdErrorMessage, habitId));
            }

            if (goal == null)
            {
                throw new ArgumentException(InvalidGoalModelErrorMessage);
            }

            var habit = this.habitRepository
                .All()
                .Where(x => x.Id == habitId)
                .First();

            var isDeletedFuture = await this.DeleteFutureHabitsAsync(goal.Id, habit);

            var isGenerated = await this.GenerateHabitsAsync(goal, habit.StartDateTime);

            return isDeletedFuture == false ? isDeletedFuture : isGenerated;
        }

        public async Task<bool> DeleteCurrentAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException(InvalidPropertyErrorMessage);
            }

            var habit = await this.habitRepository.GetByIdWithDeletedAsync(id);

            if (habit == null)
            {
                throw new ArgumentException(string.Format(InvalidHabitIdErrorMessage, id));
            }

            this.habitRepository.Delete(habit);
            var result = await this.habitRepository.SaveChangesAsync();

            return result > 0;
        }

        public async Task<bool> DeleteFollowingAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException(InvalidPropertyErrorMessage);
            }

            var habit = await this.habitRepository.GetByIdWithDeletedAsync(id);
            if (habit == null)
            {
                throw new ArgumentException(string.Format(InvalidHabitIdErrorMessage, id));
            }

            var result = await this.DeleteFutureHabitsAsync(habit.GoalId, habit);

            return result;
        }

        public async Task<bool> SetCompleteAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException(InvalidPropertyErrorMessage);
            }

            var habit = this.habitRepository.All().Where(x => x.Id == id).First();
            if (habit.IsCompleted)
            {
                return false;
            }

            habit.IsCompleted = true;
            this.habitRepository.Update(habit);
            var result = await this.habitRepository.SaveChangesAsync();

            return result > 0;
        }

        public async Task<bool> SetNotCompleteAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException(InvalidPropertyErrorMessage);
            }

            var habit = this.habitRepository.All().Where(x => x.Id == id).First();
            if (!habit.IsCompleted)
            {
                return false;
            }

            habit.IsCompleted = false;
            this.habitRepository.Update(habit);
            var result = await this.habitRepository.SaveChangesAsync();

            return result > 0;
        }

        public async Task<bool> UpdateAsync(Habit model)
        {
            if (model == null)
            {
                throw new ArgumentException(InvalidPropertyErrorMessage);
            }

            this.habitRepository.Update(model);
            var result = await this.habitRepository.SaveChangesAsync();

            return result > 0;
        }

        private Dictionary<string, int> FindCompletedHabitsForWeeks(DateTime dateTime, string goalId)
        {
            // Current Monday
            var currentMonday = this.dateTimeService.FirstDayOfWeek(dateTime);
            var habits = this.habitRepository
                .All()
                .Where(x => x.GoalId == goalId && x.StartDateTime < currentMonday && x.StartDateTime >= currentMonday.AddDays((-7) * 4) && x.IsCompleted)
                .ToList();
            var goalStartDateTime = this.habitRepository
                .All()
                .Where(x => x.GoalId == goalId)
               .Select(x => x.Goal.StartDateTime)
               .First();

            var result = new Dictionary<string, int>();
            for (int i = 0; i < 4; i++)
            {
                var monday = currentMonday.AddDays((-7) * (1 + i));
                var sunday = currentMonday.AddDays((-1) * (1 + (7 * i)));
                var doneHabitsCount = habits.Where(x => x.StartDateTime.Date >= monday.Date && x.StartDateTime.Date <= sunday.Date).Count();

                var rangeString = $"{monday:dd.MM}-{sunday:dd.MM}";
                result.Add(rangeString, doneHabitsCount);
                if (goalStartDateTime.Date >= monday.Date && goalStartDateTime <= sunday.Date)
                {
                    break;
                }
            }

            return result.Reverse().ToDictionary(x => x.Key, y => y.Value);
        }

        private async Task<bool> DeleteFutureHabitsAsync(string goalId, Habit habit)
        {
            if (string.IsNullOrEmpty(goalId))
            {
                throw new ArgumentException(InvalidPropertyErrorMessage);
            }

            var habits = this.habitRepository
                 .All()
                 .Where(x => x.GoalId == goalId)
                 .Where(x => x.StartDateTime >= habit.StartDateTime)
                 .ToList();

            foreach (var futureHabit in habits)
            {
                this.habitRepository.Delete(futureHabit);
            }

            var result = await this.habitRepository.SaveChangesAsync();

            return result > 0;
        }
    }
}
