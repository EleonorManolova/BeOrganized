namespace OrganizeMe.Data.Models.Enums
{
    using System.ComponentModel;

    public enum Duration
    {
        [Description("15 Minutes")]
        FifteenMinutes = 15,

        [Description("30 Minutes")]
        ThirtyMinutes = 30,

        [Description("1 Hour")]
        OneHour = 60,

        [Description("2 Hour")]
        TwoHours = 120,
    }
}
