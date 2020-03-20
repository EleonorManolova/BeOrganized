namespace OrganizeMe.Web.ViewModels.Calendar
{
    using System;
    using AutoMapper;
    using OrganizeMe.Data.Models;
    using OrganizeMe.Services.Mapping;

    public class EventCalendarViewModel : IMapFrom<Event>, IHaveCustomMappings
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
        }
    }
}
