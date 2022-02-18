namespace BeOrganized.Services.Data.Calendar
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using BeOrganized.Data.Common.Repositories;
    using BeOrganized.Data.Models;
    using BeOrganized.Services.Data.Color;
    using BeOrganized.Services.Mapping;
    using BeOrganized.Web.ViewModels.Administration.Calendars;
    using Microsoft.EntityFrameworkCore;

    public class CalendarService : ICalendarService
    {
        private const string InvalidCalendarIdErrorMessage = "Calendar with Id: {0} does not exist.";
        private const string InvalidPropertyErrorMessage = "One or more required properties are null.";

        private readonly IDeletableEntityRepository<Calendar> calendarRepository;
        private readonly IColorService colorService;

        public CalendarService(IDeletableEntityRepository<Calendar> calendarRepository, IColorService colorService)
        {
            this.calendarRepository = calendarRepository;
            this.colorService = colorService;
        }

        public async Task<bool> CreateFromAdminAsync(CalendarViewModel calendarModel)
        {
            if (string.IsNullOrEmpty(calendarModel.Title) ||
              string.IsNullOrEmpty(calendarModel.UserId) ||
              calendarModel.ColorId <= 0)
            {
                throw new ArgumentException(InvalidPropertyErrorMessage);
            }

            var calendar = new Calendar
            {
                Title = calendarModel.Title,
                UserId = calendarModel.UserId,
                DefaultCalendarColorId = calendarModel.ColorId,
            };

            await this.calendarRepository.AddAsync(calendar);
            var result = await this.calendarRepository.SaveChangesAsync();

            return result > 0;
        }

        public ICollection<Calendar> GetAll()
        {
            return this.calendarRepository.All().ToList();
        }

        public ICollection<CalendarDetailsModel> GetDetailsViewModels()
        {
            return this.calendarRepository
                .All()
                .Select(x => new CalendarDetailsModel
                {
                    Id = x.Id,
                    Title = x.Title,
                    ColorName = x.DefaultCalendarColor.Name,
                    UserUserName = x.User.UserName,
                    CreatedOn = x.CreatedOn,
                })
                .ToList();
        }

        public ICollection<T> GetAllCalendarTitlesByUserName<T>(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                throw new ArgumentException(InvalidPropertyErrorMessage);
            }

            var result = this.calendarRepository
                .All()
                .Include(x => x.User)
                .Where(x => x.User.UserName == username)
                .To<T>()
                .ToList();
            return result;
        }

        public CalendarChangeModel GetCreateViewModel()
        {
            var model = new CalendarChangeModel
            {
                CalendarModel = new CalendarViewModel(),
                Colors = this.colorService.GetAllColors(),
            };

            return model;
        }

        public int GetDefaultCalendarColorId(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                throw new ArgumentException(InvalidPropertyErrorMessage);
            }

            var result = this.calendarRepository
                .All()
                .Select(x => new
                {
                    x.DefaultCalendarColorId,
                    x.User.UserName,
                })
                .Where(x => x.UserName == username)
                .Select(x => x.DefaultCalendarColorId)
                .First();
            return result;
        }

        public string GetCalendarId(string username, string calendarTitle)
        {
            if (string.IsNullOrEmpty(username))
            {
                throw new ArgumentException(InvalidPropertyErrorMessage);
            }

            var result = this.calendarRepository
                .All()
                .Select(x => new
                {
                    x.Id,
                    x.User.UserName,
                    x.Title,
                })
                .Where(x => x.UserName == username && x.Title == calendarTitle)
                .Select(x => x.Id)
                .First();
            return result;
        }

        public string GetUserNameByCalendarId(string calendarId)
        {
            if (string.IsNullOrEmpty(calendarId))
            {
                throw new ArgumentException(InvalidPropertyErrorMessage);
            }

            var username = this.calendarRepository
                .All()
                .Where(x => x.Id == calendarId)
                .Select(x => x.User.UserName)
                .First();

            return username;
        }

        public CalendarChangeModel GetEditChangeViewModelById(string calendarId)
        {
            if (string.IsNullOrEmpty(calendarId))
            {
                throw new ArgumentException(InvalidPropertyErrorMessage);
            }

            var calendar = this.calendarRepository.All().Where(x => x.Id == calendarId).To<CalendarViewModel>().First();
            var calendarResult = new CalendarChangeModel
            {
                CalendarModel = calendar,
                Colors = this.colorService.GetAllColors(),
            };
            return calendarResult;
        }

        public Calendar MapCalendarViewModelToCalendar(CalendarViewModel model, string calendarId)
        {
            if (string.IsNullOrEmpty(calendarId) || model == null)
            {
                throw new ArgumentException(InvalidPropertyErrorMessage);
            }

            var calendar = this.calendarRepository
                .All()
                .Where(x => x.Id == calendarId)
                .First();

            calendar.Id = calendarId;
            calendar.Title = model.Title;
            calendar.UserId = model.UserId;
            calendar.DefaultCalendarColorId = model.ColorId;

            return calendar;
        }

        public async Task<bool> UpdateAsync(Calendar model, string calendarId)
        {
            if (model == null || string.IsNullOrEmpty(calendarId))
            {
                throw new ArgumentException(InvalidPropertyErrorMessage);
            }

            this.calendarRepository.Update(model);
            var result = await this.calendarRepository.SaveChangesAsync();

            return result > 0;
        }

        public async Task<Calendar> GetByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException(InvalidPropertyErrorMessage);
            }

            var calendar = await this.calendarRepository.GetByIdWithDeletedAsync(id);

            if (calendar == null)
            {
                throw new ArgumentException(string.Format(InvalidCalendarIdErrorMessage, id));
            }

            return calendar;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException(InvalidPropertyErrorMessage);
            }

            var calendar = await this.calendarRepository.GetByIdWithDeletedAsync(id);

            if (calendar == null)
            {
                throw new ArgumentException(string.Format(InvalidCalendarIdErrorMessage, id));
            }

            this.calendarRepository.Delete(calendar);
            var result = await this.calendarRepository.SaveChangesAsync();

            return result > 0;
        }
    }
}
