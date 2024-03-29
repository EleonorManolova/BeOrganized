﻿namespace BeOrganized.Services.Tests
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
            var currentDate = new DateTime(2020, 02, 02);
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
            var currentDate = new DateTime(2022, 03, 13);
            var eveningStartDate = 17;
            var eveningEndDate = 21;
            var durationInHour = 2;

            var actualResult = this.datetimeService.GenerateDatesForMonthAhead(120, 11, "Evening", currentDate);
            Assert.Equal(5, actualResult.Count);
            Assert.True(actualResult[0].Start.Hour >= eveningStartDate);
            Assert.True(actualResult[0].Start.Hour <= eveningEndDate);
            Assert.True(actualResult[0].Start.Hour + durationInHour == actualResult[0].End.Hour);
        }

        [Fact]
        public void FirstDayOfWeekAfterMonth_WithCorrectData_ShouldReturnCorrectResults()
        {
            var date = new DateTime(2020, 02, 04);
            var mondayOfWeekAfterMonth = new DateTime(2020, 03, 02);
            var actualResult = this.datetimeService.FirstDayOfWeekAfterMonth(date);
            Assert.Equal(mondayOfWeekAfterMonth, actualResult);
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
