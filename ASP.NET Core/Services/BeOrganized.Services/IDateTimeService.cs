namespace BeOrganized.Services
{
    using System;
    using System.Collections.Generic;

    public interface IDateTimeService
    {
        int FindFrequency(int frequency);

        DateTime FirstDayOfWeek(DateTime dt);

        DateTime FirstDayOfWeekAfhterMonth(DateTime dt);

        List<StartEndDateTime> GenerateDatesForMonthAhead(int duration, int frequency, string dayTime, DateTime currentDate);
    }
}
