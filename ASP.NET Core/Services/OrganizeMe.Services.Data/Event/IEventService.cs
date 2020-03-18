namespace OrganizeMe.Services.Data.Events
{
    using System.Threading.Tasks;

    using OrganizeMe.Web.ViewModels.Events;

    public interface IEventService
    {
        Task<bool> CreateAsync(EventInputViewModel eventViewModel);

        EventCreateViewModel GetEventViewModel(string googleApi);
    }
}
