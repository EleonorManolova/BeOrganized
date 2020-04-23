namespace BeOrganized.Services.Tests
{
    using System;

    using BeOrganized.Services;
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

        [Fact]
        public void FirstDayOfWeekAfhterMonth_WithCorrectData_ShouldReturnCorrectResults()
        {
            var date = DateTime.Parse("04/02/2020");
            var mondayOfWeek = DateTime.Parse("03/02/2020");
            var actualResult = this.datetimeService.FirstDayOfWeekAfhterMonth(date);
            Assert.Equal(mondayOfWeek.AddDays(7 * 4), actualResult);
        }

        [Fact]
        public void FindFrequencyByWeek_WithCorrectData_ShouldReturnCorrectResults()
        {
            var currentDate = DateTime.Now;
            var freqency = 17;
            var expected = 7;

            var actualResult = this.datetimeService.FindFrequency(freqency);
            Assert.Equal(expected, actualResult);
        }

        [Fact]
        public void FindFrequencyByMonth_WithCorrectData_ShouldReturnCorrectResults()
        {
            var currentDate = DateTime.Now;
            var freqency = 2;
            var expected = 2;

            var actualResult = this.datetimeService.FindFrequency(freqency);
            Assert.Equal(expected, actualResult);
        }

        [Fact]
        public void FindFrequency_WithInvalidEnum_ShouldThrowAnArgumentException()
        {
            var currentDate = DateTime.Now;
            var freqency = -1;
            var exeptionErrorMessage = "Enum does not exist.";

            var exeption = Assert.Throws<ArgumentException>(() =>
              this.datetimeService.FindFrequency(freqency));

            Assert.Equal(exeptionErrorMessage, exeption.Message);
        }

        [Fact]
        public void FindFrequency_WithNotExistingEnum_ShouldThrowAnArgumentException()
        {
            var currentDate = DateTime.Now;
            var freqency = 20;
            var exeptionErrorMessage = "Enum does not exist.";

            var exeption = Assert.Throws<ArgumentException>(() =>
              this.datetimeService.FindFrequency(freqency));

            Assert.Equal(exeptionErrorMessage, exeption.Message);
        }
    }
}
