namespace OrganizeMe.Data.Models
{
    using OrganizeMe.Data.Common.Models;

    public class Calendar : BaseDeletableModel<string>
    {
        public string UserId { get; set; }

        public ApplicationUser User { get; set; }
    }
}
