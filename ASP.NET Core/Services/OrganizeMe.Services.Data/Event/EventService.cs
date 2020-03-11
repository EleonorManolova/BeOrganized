namespace OrganizeMe.Services.Data.Events
{
    using System;

    using OrganizeMe.Web.ViewModels.Events;

    public class EventService : IEventService
    {
        public EventCreateViewModel GetEventViewModel(string googleApi)
        {
            var model = new EventCreateViewModel
            {
                Input = new EventInputViewModel
                {
                    StartDate = DateTime.Now,
                    StartTime = DateTime.Now,
                    EndDate = DateTime.Now,
                    EndTime = DateTime.Now.AddMinutes(30),
                },
                GoogleApi = googleApi,
            };

            return model;
        }
    }
}
