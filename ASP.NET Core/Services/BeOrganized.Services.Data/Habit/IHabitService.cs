namespace BeOrganized.Services.Data.Habit
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using BeOrganized.Data.Models;
    using BeOrganized.Web.ViewModels.Calendar;
    using BeOrganized.Web.ViewModels.Habits;

    public interface IHabitService
    {
        HabitDetailsViewModel GetDetailsViewModelById(string id);

        Task GenerateMoreHabitsAsync(string calendarId);

        Task<bool> GenerateHabitsAsync(Goal goal, DateTime currentDate);

        ICollection<HabitCalendarViewModel> GetAllByCalendarId(string calendarId);

        Task UpdateHabitsAsync(Goal model, string habitId);

        Task<Habit> GetByIdAsync(string id);

        Task<bool> DeleteCurrentAsync(string id);

        Task<bool> DeleteFollowingAsync(string id);

        Task<bool> SetCompleteAsync(string id);

        Task<bool> SetNotCompleteAsync(string id);

        Task<bool> UpdateAsync(Habit model, string id);
    }
}
