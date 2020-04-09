namespace OrganizeMe.Web.ViewModels.Events
{
    using System.ComponentModel.DataAnnotations;

    public class EventHubViewModel
    {
        [Required]
        public string Id { get; set; }

        [DataType(DataType.DateTime)]
        public string StartDateTime { get; set; }

        [DataType(DataType.DateTime)]
        public string EndDateTime { get; set; }
    }
}
