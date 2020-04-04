namespace OrganizeMe.Web.ViewModels.Calendar
{
    using System;

    using AutoMapper;
    using OrganizeMe.Data.Models;
    using OrganizeMe.Services.Mapping;

    public class EventCalendarViewModel : IHaveCustomMappings
    {
        public string Title { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndDate { get; set; }

        public DateTime EndTime { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<Event, EventCalendarViewModel>().ForMember(
                m => m.Title,
                opt => opt.MapFrom(x => x.Title));

            configuration.CreateMap<Event, EventCalendarViewModel>()
                .ForMember(x => x.StartDate, y => y.MapFrom(x => x.StartDateTime.Date))
                .ForMember(x => x.StartTime, y => y.MapFrom(x => default(DateTime).Add(x.StartDateTime.TimeOfDay)))
                .ForMember(x => x.EndDate, y => y.MapFrom(x => x.EndDateTime.Date))
                .ForMember(x => x.EndTime, y => y.MapFrom(x => default(DateTime).Add(x.EndDateTime.TimeOfDay)));
        }
    }
}
