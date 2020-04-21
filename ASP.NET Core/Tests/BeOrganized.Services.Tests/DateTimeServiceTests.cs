namespace BeOrganized.Services.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;
    using BeOrganized.Data.Models;
    using BeOrganized.Services;
    using Moq;
    using Nest;
    using Xunit;

    public class DateTimeServiceTests
    {
        private DateTimeService datetimeService;

        public DateTimeServiceTests()
        {
            this.datetimeService = new DateTimeService();
        }

        [Fact]
        public void GenerateDatesForMonthAheadByMonth_WithCorrectData_ShouldReturnCorrectResults()
        {
            var currentDate = DateTime.Parse("02/02/2020");
            var afternoonStartDate = 12;
            var afternoonEndDate = 17;
            var durationInHour = 1;

            var actualResult = this.datetimeService.GenerateDatesForMonthAhead(60, 1, "Afternoon", currentDate);
            Assert.Single(actualResult);
            Assert.True(actualResult[0].Start.Hour >= afternoonStartDate);
            Assert.True(actualResult[0].Start.Hour <= afternoonEndDate);
            Assert.True(actualResult[0].Start.Hour + durationInHour == actualResult[0].End.Hour);
        }

        [Fact]
        public void GenerateDatesForMonthAheadByWeek_WithCorrectData_ShouldReturnCorrectResults()
        {
            var currentDate = DateTime.Now;
            var eveningStartDate = 17;
            var eveningEndDate = 21;
            var durationInHour = 2;

            var actualResult = this.datetimeService.GenerateDatesForMonthAhead(120, 11, "Evening", currentDate);
            Assert.Equal(4, actualResult.Count);
            Assert.True(actualResult[0].Start.Hour >= eveningStartDate);
            Assert.True(actualResult[0].Start.Hour <= eveningEndDate);
            Assert.True(actualResult[0].Start.Hour + durationInHour == actualResult[0].End.Hour);
        }
    }
}
