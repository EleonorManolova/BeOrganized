namespace BeOrganized.Web.ViewModels.Habits
{
    using System.ComponentModel.DataAnnotations;

    public class HabitHubViewModel
    {
        [Required]
        public string Id { get; set; }

        [DataType(DataType.DateTime)]
        public string StartDateTime { get; set; }

        [DataType(DataType.DateTime)]
        public string EndDateTime { get; set; }
    }
}
