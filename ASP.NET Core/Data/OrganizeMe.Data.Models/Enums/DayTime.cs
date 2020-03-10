namespace OrganizeMe.Data.Models.Enums
{
    using System.ComponentModel;

    public enum DayTime
    {
        [Description("Any Time")]
        AnyTime = 0,

        [Description("Morning")]
        Morning = 1,

        [Description("Afternoon")]
        Afternoon = 2,

        [Description("Evening")]
        Evening = 3,
    }
}
