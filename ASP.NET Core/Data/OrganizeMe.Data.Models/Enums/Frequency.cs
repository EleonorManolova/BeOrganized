namespace OrganizeMe.Data.Models.Enums
{
    using System.ComponentModel;

    public enum Frequency
    {
        [Description("Once a Month")]
        OnceAMonth = 1,

        [Description("Twice a Month")]
        TwiceAMonth = 2,

        [Description("Once a Week")]
        OnceAWeek = 3,

        [Description("Twice a Week")]
        TwiceAWeek = 4,

        [Description("3 Times a Week")]
        ThreeTimesAWeek = 5,

        [Description("4 Times a Week")]
        FourTimesAWeek = 6,

        [Description("5 Times a Week")]
        FiveTimesAWeek = 7,

        [Description("6 Times a Week")]
        SixTimesAWeek = 8,

        [Description("Every Day")]
        EveryDay = 9,
    }
}
