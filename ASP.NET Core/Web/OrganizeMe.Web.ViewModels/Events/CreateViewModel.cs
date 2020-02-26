namespace OrganizeMe.Web.ViewModels.Events
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using Microsoft.AspNetCore.Mvc;

    public class CreateViewModel
    {
        [BindProperty]
        public Input Input { get; set; }

        public string GoogleApi { get; set; }
    }

    public class Input
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string StartDate { get; set; }

        [Required]
        public string StartTime { get; set; }

        [Required]
        public string EndDate { get; set; }

        [Required]
        public string EndTime { get; set; }

        public string Address { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }
    }
}
