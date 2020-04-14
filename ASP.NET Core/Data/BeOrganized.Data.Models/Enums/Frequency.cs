namespace BeOrganized.Data.Models.Enums
{
    using System.ComponentModel;

    public enum Frequency
    {
        [Description("Once a Month")]
        OnceAMonth = 1,

        [Description("Twice a Month")]
        TwiceAMonth = 2,

        [Description("Once a Week")]
        OnceAWeek = 11,

        [Description("Twice a Week")]
        TwiceAWeek = 12,

        [Description("3 Times a Week")]
        ThreeTimesAWeek = 13,

        [Description("4 Times a Week")]
        FourTimesAWeek = 14,

        [Description("5 Times a Week")]
        FiveTimesAWeek = 15,

        [Description("6 Times a Week")]
        SixTimesAWeek = 16,

        [Description("Every Day")]
        EveryDay = 17,
    }
}
