namespace BeOrganized.Services.Data.Habit
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using BeOrganized.Data.Models;
    using BeOrganized.Web.ViewModels.Calendar;
    using BeOrganized.Web.ViewModels.Habits;

    public interface IHabitService
    {
        HabitDetailsViewModel GetDetailsViewModelById(string id);

        void GenerateMoreHabits(string calendarId);

        Task<bool> GenerateHabitsInitialAsync(Goal goal);
        ICollection<HabitCalendarViewModel> GetAllByCalendarId(string calendarId);
    }
}
