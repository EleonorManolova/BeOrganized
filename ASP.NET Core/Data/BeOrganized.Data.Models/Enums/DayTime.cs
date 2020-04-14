namespace BeOrganized.Data.Models.Enums
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

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
