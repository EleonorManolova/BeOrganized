namespace BeOrganized.Services
{
    using System.Collections.Generic;

    public interface IDateTimeService
    {
        List<StartEndDateTime> GenerateDatesForMonthAhead(int duration, int frequency, string dayTime);
    }
}
