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
        [Required(ErrorMessage = "The Title is required.")]
        public string Title { get; set; }

        [Required]
        [Display(Name = "Start Date")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required]
        [Display(Name = "Start Time")]
        [DataType(DataType.Time)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:HH:mm}")]
        public DateTime StartTime { get; set; }

        [Required]
        [Display(Name = "End Date")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        [Required]
        [Display(Name = "End Time")]
        [DataType(DataType.Time)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:HH:mm}")]
        public DateTime EndTime { get; set; }

        public string Address { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }
    }
}
