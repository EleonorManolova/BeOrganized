﻿namespace BeOrganized.Services.Data.Habit
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
        private const string InvaliHabitIdErrorMessage = "Habit with Id: {0} does not exist.";
        private const string InvalidGoalModelErrorMessage = "Goal does not exist.";
        private const string InvalidPropertyErrorMessage = "One or more required properties are null.";

        private readonly IDeletableEntityRepository<Habit> habitRepository;
        private readonly IDateTimeService dateTimeService;
        private readonly ISearchService searchService;

        public HabitService(IDeletableEntityRepository<Habit> habitRepository, IDateTimeService dateTimeService, ISearchService searchService)
        {
            this.habitRepository = habitRepository;
            this.dateTimeService = dateTimeService;
            this.searchService = searchService;
        }

        public ICollection<HabitCalendarViewModel> GetAllByCalendarId(string calendarId)
        {
            if (string.IsNullOrEmpty(calendarId))
            {
                throw new ArgumentException(InvalidPropertyErrorMessage);
            }

            var habits = this.habitRepository.All().Where(x => x.Goal.CalendarId == calendarId).To<HabitCalendarViewModel>().ToList();
            return habits;
        }

        public async Task<bool> GenerateHabitsInitialAsync(Goal goal)
        {
            if (goal == null)
            {
                throw new ArgumentException(InvalidGoalModelErrorMessage);
            }

            var startEndDateTimes = this.dateTimeService.GenerateDatesForMonthAhead((int)goal.Duration, (int)goal.Frequency, goal.DayTime.ToString());

            foreach (var time in startEndDateTimes)
            {
                var habit = new Habit()
                {
                    Title = goal.Title,
                    StartDateTime = time.Start,
                    EndDateTime = time.End,
                    Goal = goal,
                };

                await this.searchService.CreateIndexAsync<Habit>(habit);
                await this.habitRepository.AddAsync(habit);
            }

            var result = await this.habitRepository.SaveChangesAsync();

            return result > 0;
        }

        public void GenerateMoreHabits(string calendarId)
        {
            if (string.IsNullOrEmpty(calendarId))
            {
                throw new ArgumentException(InvalidPropertyErrorMessage);
            }

            var goals = this.habitRepository.All().Where(x => x.Goal.Calendar.Id == calendarId);
        }

        public HabitDetailsViewModel GetDetailsViewModelById(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException(InvalidPropertyErrorMessage);
            }

            var habit = this.habitRepository.All().Where(x => x.Id == id).To<HabitDetailsViewModel>().First();

            if (habit == null)
            {
                throw new ArgumentException(string.Format(InvaliHabitIdErrorMessage, id));
            }

            return habit;
        }
    }
}
