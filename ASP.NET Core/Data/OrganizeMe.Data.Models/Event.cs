namespace OrganizeMe.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using OrganizeMe.Data.Common.Models;

    public class Event : BaseDeletableModel<string>
    {
        public int MyProperty { get; set; }

        public string CalendarId { get; set; }

        public Calendar Calendar { get; set; }
    }
}
