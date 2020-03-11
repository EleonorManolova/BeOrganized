namespace OrganizeMe.Web.ViewModels.Events
{
    using Microsoft.AspNetCore.Mvc;

    public class EventCreateViewModel
    {
        [BindProperty]
        public EventInputViewModel Input { get; set; }

        public string GoogleApi { get; set; }
    }
}
