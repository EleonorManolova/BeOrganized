namespace BeOrganized.Services.Data.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    using BeOrganized.Data.Common.Repositories;
    using BeOrganized.Data.Models;
    using BeOrganized.Services.Data.Calendar;
    using BeOrganized.Services.Mapping;
    using BeOrganized.Web.ViewModels.Golas;
    using Moq;
    using Xunit;

    public class CalendarServiceTests
    {
        private Mock<IDeletableEntityRepository<Calendar>> calendarRepository;
        private CalendarService calendarService;

        public CalendarServiceTests()
        {
            this.InitializeMapper();
            this.calendarRepository = new Mock<IDeletableEntityRepository<Calendar>>();
            this.calendarService = new CalendarService(this.calendarRepository.Object);
        }

        [Fact]
        public void GetAllColors_WithCorrectData_ShouldReturnCorrectResult()
        {
            var user = new ApplicationUser
            {
                Id = "User1",
                UserName = "Username",
            };

            var color = new Color
            {
                Id = 1,
                Name = "Test",
                Hex = "TestHex",
            };

            var calendar = new Calendar
            {
                Id = "1",
                Title = "Default",
                DefaultCalendarColorId = color.Id,
                DefaultCalendarColor = color,
                User = user,
            };

            this.calendarRepository.Setup(x => x.All()).Returns(new List<Calendar> { calendar }.AsQueryable);
            var resultCollection = this.calendarService.GetAllCalendarTitlesByUserName<CalendarHabitViewModel>(user.UserName);
            var result = resultCollection.First();

            this.calendarRepository.Verify(m => m.All(), Times.Once);
            Assert.Equal(1, resultCollection.Count);
            Assert.Equal(calendar.Id, result.Id);
            Assert.Equal(calendar.Title, result.Title);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void GetAllCalendarTitlesByUserName__WithNullOrEmptyArgument_ShouldThrowAnArgumentException(string username)
        {
            var exeptionErrorMessage = "One or more required properties are null.";

            var exeption = Assert.Throws<ArgumentException>(() =>
              this.calendarService.GetAllCalendarTitlesByUserName<CalendarHabitViewModel>(username));

            Assert.Equal(exeptionErrorMessage, exeption.Message);
        }

        [Fact]
        public void GetDefaultCalendarColorId_WithCorrectData_ShouldReturnCorrectResult()
        {
            var user = new ApplicationUser
            {
                Id = "User1",
                UserName = "Username",
            };

            var color = new Color
            {
                Id = 1,
                Name = "Test",
                Hex = "TestHex",
            };

            var calendar = new Calendar
            {
                Id = "1",
                Title = "Default",
                DefaultCalendarColorId = color.Id,
                DefaultCalendarColor = color,
                User = user,
            };

            this.calendarRepository.Setup(x => x.All()).Returns(new List<Calendar> { calendar }.AsQueryable);
            var result = this.calendarService.GetDefaultCalendarColorId(user.UserName);

            this.calendarRepository.Verify(m => m.All(), Times.Once);
            Assert.Equal(calendar.DefaultCalendarColorId, result);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void GetDefaultCalendarColorId__WithNullOrEmptyArgument_ShouldThrowAnArgumentException(string username)
        {
            var exeptionErrorMessage = "One or more required properties are null.";

            var exeption = Assert.Throws<ArgumentException>(() =>
              this.calendarService.GetDefaultCalendarColorId(username));

            Assert.Equal(exeptionErrorMessage, exeption.Message);
        }

        [Fact]
        public void GetDefaultCalendarIdd_WithCorrectData_ShouldReturnCorrectResult()
        {
            var user = new ApplicationUser
            {
                Id = "User1",
                UserName = "Username",
            };

            var color = new Color
            {
                Id = 1,
                Name = "Test",
                Hex = "TestHex",
            };

            var calendar = new Calendar
            {
                Id = "1",
                Title = "Default",
                DefaultCalendarColorId = color.Id,
                DefaultCalendarColor = color,
                User = user,
            };

            this.calendarRepository.Setup(x => x.All()).Returns(new List<Calendar> { calendar }.AsQueryable);
            var result = this.calendarService.GetDefaultCalendarId(user.UserName);

            this.calendarRepository.Verify(m => m.All(), Times.Once);
            Assert.Equal(calendar.Id, result);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void GetDefaultCalendarId__WithNullOrEmptyArgument_ShouldThrowAnArgumentException(string username)
        {
            var exeptionErrorMessage = "One or more required properties are null.";

            var exeption = Assert.Throws<ArgumentException>(() =>
              this.calendarService.GetDefaultCalendarId(username));

            Assert.Equal(exeptionErrorMessage, exeption.Message);
        }

        [Fact]
        public void GetUserNameByCalendarId_WithCorrectData_ShouldReturnCorrectResult()
        {
            var user = new ApplicationUser
            {
                Id = "User1",
                UserName = "Username",
            };

            var color = new Color
            {
                Id = 1,
                Name = "Test",
                Hex = "TestHex",
            };

            var calendar = new Calendar
            {
                Id = "1",
                Title = "Default",
                DefaultCalendarColorId = color.Id,
                DefaultCalendarColor = color,
                User = user,
            };

            this.calendarRepository.Setup(x => x.All()).Returns(new List<Calendar> { calendar }.AsQueryable);
            var result = this.calendarService.GetUserNameByCalendarId(calendar.Id);

            this.calendarRepository.Verify(m => m.All(), Times.Once);
            Assert.Equal(user.UserName, result);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void GetUserNameByCalendarId__WithNullOrEmptyArgument_ShouldThrowAnArgumentException(string username)
        {
            var exeptionErrorMessage = "One or more required properties are null.";

            var exeption = Assert.Throws<ArgumentException>(() =>
              this.calendarService.GetUserNameByCalendarId(username));

            Assert.Equal(exeptionErrorMessage, exeption.Message);
        }

        private void InitializeMapper() => AutoMapperConfig.
           RegisterMappings(Assembly.Load("BeOrganized.Web.ViewModels"));
    }
}
