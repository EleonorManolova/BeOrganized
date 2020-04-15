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

        public void GenerateMoreHabits(string calendarId)
        {
            if (string.IsNullOrEmpty(calendarId))
            {
                throw new ArgumentException(string.Format(InvaliCalendarIdErrorMessage, calendarId));
            }

            var goals = this.habitRepository.All().Where(x => x.Goal.Calendar.Id == calendarId);
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
                    GoalId = x.GoalId,
                    IsCompleted = x.IsCompleted,
                })
                .First();

            habit.GoalFrequency = this.enumParseService.GetEnumDescription(habit.GoalFrequency, typeof(Frequency));
            habit.GoalDuration = this.enumParseService.GetEnumDescription(habit.GoalDuration, typeof(Duration));
            habit.GoalDayTime = this.enumParseService.GetEnumDescription(habit.GoalDayTime, typeof(DayTime));

            return habit;
        }

        public async Task UpdateHabitsAsync(Goal goal, string habitId)
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

            await this.DeleteFutureHabitsAsync(goal.Id, habit);

            await this.GenerateHabitsAsync(goal, habit.StartDateTime);
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

        public async Task<bool> SetComplete(string id)
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

        public async Task<bool> SetNotComplete(string id)
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

        public async Task<bool> UpdateAsync(Habit model, string habitId)
        {
            if (model == null || string.IsNullOrEmpty(habitId))
            {
                throw new ArgumentException(InvalidPropertyErrorMessage);
            }

            this.habitRepository.Update(model);
            var result = await this.habitRepository.SaveChangesAsync();

            return result > 0;
        }

        private async Task<bool> DeleteFutureHabitsAsync(string goalId, Habit habit)
        {
            if (string.IsNullOrEmpty(goalId))
            {
                throw new ArgumentException(InvalidPropertyErrorMessage);
            }

            if (habit == null)
            {
                throw new ArgumentException(InvalidHabitIdErrorMessage, habit.Id);
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
