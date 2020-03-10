namespace OrganizeMe.Web.ViewModels.Events
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using Microsoft.AspNetCore.Mvc;

    public class EventCreateViewModel
    {
        [BindProperty]
        public EventInputViewModel Input { get; set; }

        public string GoogleApi { get; set; }
    }
}
