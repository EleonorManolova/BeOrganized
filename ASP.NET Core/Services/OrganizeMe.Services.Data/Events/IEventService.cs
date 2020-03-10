namespace OrganizeMe.Services.Data.Events
{
    using OrganizeMe.Web.ViewModels.Events;

    public interface IEventService
    {
        EventCreateViewModel GetEventViewModel(string googleApi);
    }
}
