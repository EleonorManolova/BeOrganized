namespace BeOrganized.Services
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class DateTimeService : IDateTimeService
    {
        private static Dictionary<string, List<DateTime>> dictionary;
        private DateTime firstMonday;
        private Random random;

        public DateTimeService()
        {
            this.random = new Random();
            GetTimesByDayTime();
            this.firstMonday = FirstDayOfWeek(DateTime.Now);
        }

        public List<StartEndDateTime> GenerateDatesForMonthAhead(int duration, int frequency, string dayTime)
        {
            var daytimeLower = dayTime.ToLower();

            var hoursDaytime = dictionary[daytimeLower];

            var times = new List<StartEndDateTime>();
            if (frequency / 10 < 1)
            {
                // 1 or 2 times a month
                times = this.CreateTime(duration, frequency, hoursDaytime, DateTime.Now, this.firstMonday.AddMonths(1));
            }
            else if (frequency / 10 < 10)
            {
                // 1, 2, 3, 4, 5, 6, 7 times a week
                var frequencyForWeek = frequency % 10;
                times = this.CreateTimeForMonthByWeekFrequency(duration, frequencyForWeek, hoursDaytime);
            }

            return times;
        }

        private static void GetTimesByDayTime()
        {
            dictionary = new Dictionary<string, List<DateTime>>();
            dictionary.Add("morning", GetTimesBetween("09:00", "12:00"));
            dictionary.Add("afternoon", GetTimesBetween("12:00", "17:00"));
            dictionary.Add("evening", GetTimesBetween("17:00", "21:00"));
            dictionary.Add("anytime", GetTimesBetween("17:00", "21:00"));
        }

        private static DateTime FirstDayOfWeek(DateTime dt)
        {
            var culture = System.Threading.Thread.CurrentThread.CurrentCulture;
            var diff = dt.DayOfWeek - culture.DateTimeFormat.FirstDayOfWeek;
            if (diff < 0)
            {
                diff += 7;
            }

            return dt.AddDays(-diff).Date;
        }

        private static List<DateTime> GetTimesBetween(string startTime, string endTime)
        {
            DateTime dt1 = DateTime.ParseExact(startTime, "HH:mm", null);
            DateTime dt2 = DateTime.ParseExact(endTime, "HH:mm", null);
            List<DateTime> listOfTimes = new List<DateTime>();
            while (dt1 <= dt2)
            {
                listOfTimes.Add(dt1);
                dt1 = dt1.AddMinutes(30);
            }

            return listOfTimes;
        }

        private List<StartEndDateTime> CreateTimeForMonthByWeekFrequency(int duration, int frequency, List<DateTime> hoursDaytime)
        {
            // CreateTimeFor4WeeksAhead
            var datesFor4Week = new List<StartEndDateTime>();

            for (int i = 0; i < 4; i++)
            {
                if (i == 0)
                {
                    datesFor4Week.AddRange(this.CreateTime(duration, frequency, hoursDaytime, DateTime.Now.Date, this.firstMonday.AddDays(7)));
                }
                else
                {
                    datesFor4Week.AddRange(this.CreateTime(duration, frequency, hoursDaytime, this.firstMonday.AddDays((7 * i) + 1), this.firstMonday.AddDays(7 * (i + 1))));
                }
            }

            return datesFor4Week;
        }

        private List<StartEndDateTime> CreateTime(int duration, int frequency, List<DateTime> hoursDaytime, DateTime firstDay, DateTime lastDate)
        {
            // CreateTimeAhead
            var datesForWeek = new List<StartEndDateTime>();

            for (int i = 0; i < frequency; i++)
            {
                var startDateTime = this.RandomDay(firstDay, lastDate);
                var time = hoursDaytime[this.random.Next(hoursDaytime.Count)].TimeOfDay.Ticks;
                startDateTime = startDateTime.AddTicks(time);
                datesForWeek.Add(new StartEndDateTime { Start = startDateTime, End = startDateTime.AddMinutes(duration) });
            }

            return datesForWeek;
        }

        private DateTime RandomDay(DateTime startDate, DateTime endDate)
        {
            int range = (endDate - startDate).Days;
            return startDate.AddDays(this.random.Next(range));
        }
    }

    public class StartEndDateTime
    {
        public DateTime Start { get; set; }

        public DateTime End { get; set; }
    }
}
