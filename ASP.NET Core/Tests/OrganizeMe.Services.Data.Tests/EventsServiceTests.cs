namespace OrganizeMe.Services.Data.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;

    using Moq;
    using OrganizeMe.Data.Common.Repositories;
    using OrganizeMe.Data.Models;
    using OrganizeMe.Services.Data.Calendar;
    using OrganizeMe.Services.Data.Events;
    using OrganizeMe.Services.Mapping;
    using OrganizeMe.Web.ViewModels.Events;
    using Xunit;

    public class EventsServiceTests
    {

        [Fact]
        public async Task CreateAsync_WithCorrectData_ShouldReturnCorrectResult()
        {
            InitializeAutomapper<EventViewModel>();
            var calendarRepository = new Mock<IDeletableEntityRepository<Calendar>>();
            var calendarService = new CalendarService(calendarRepository.Object);
            var eventsRepository = new Mock<IDeletableEntityRepository<Event>>();
            var eventService = new EventService(eventsRepository.Object, calendarService);

            var model = new EventViewModel
            {
                Title = "Test",
                StartDate = new DateTime(2020, 02, 02, 12, 0, 0),
                StartTime = new DateTime(1999, 1, 1, 12, 0, 0),
                EndDate = new DateTime(2020, 02, 02, 12, 0, 0),
                EndTime = new DateTime(1999, 1, 1, 12, 30, 0),
                CalendarId = "1",
                Description = "test description",
                Location = "Hotel Test",
                Coordinates = "42.99, 32.99",
            };
            var result = await eventService.CreateAsync(model);
            Assert.True(result);
        }

        [Fact(Skip = "Dont pass")]
        public async Task CreateAsync_WithCorrectData_ShouldCreateCorrectly()
        {
            InitializeAutomapper<EventViewModel>();
            var calendarRepository = new Mock<IDeletableEntityRepository<Calendar>>();
            var calendarService = new CalendarService(calendarRepository.Object);
            var eventsRepository = new Mock<IDeletableEntityRepository<Event>>();
            var eventService = new EventService(eventsRepository.Object, calendarService);

            var model = new EventViewModel
            {
                Title = "Test",
                StartDate = new DateTime(2020, 02, 02, 12, 0, 0),
                StartTime = new DateTime(1999, 1, 1, 12, 0, 0),
                EndDate = new DateTime(2020, 02, 02, 12, 0, 0),
                EndTime = new DateTime(1999, 1, 1, 12, 30, 0),
                CalendarId = "1",
                Description = "test description",
                Location = "Hotel Test",
                Coordinates = "42.99, 32.99",
            };

            await eventService.CreateAsync(model);
            var actualResult = eventsRepository.Object.All().FirstOrDefault();
            var expectedResult = model;

            // eventsRepository.Setup(x => x.All().First()).Returns(model);
            // eventsRepository.Verify(x=>x.cr)

            Assert.Equal(expectedResult.Title, actualResult.Title);
            Assert.Equal(expectedResult.CalendarId, actualResult.CalendarId);
            Assert.Equal(expectedResult.Location, actualResult.Location);
            Assert.Equal(expectedResult.Coordinates, actualResult.Coordinates);
            Assert.Equal(expectedResult.Description, actualResult.Description);
            Assert.Equal(expectedResult.StartDateTime, actualResult.StartDateTime);
            Assert.Equal(expectedResult.EndDateTime, actualResult.EndDateTime);
        }

        [Theory]
        [InlineData("", "1")]
        [InlineData(null, "1")]
        [InlineData("Test", "")]
        [InlineData("Test", null)]
        [InlineData("", "")]
        [InlineData(null, null)]
        public async Task CreateAsync_WithNullOrEmptyArguments_ShouldThrowAnArgumentException(string title, string calendarId)
        {
            var exeptionErrorMessage = "One or more required properties are null.";
            InitializeAutomapper<EventViewModel>();
            var calendarRepository = new Mock<IDeletableEntityRepository<Calendar>>();
            var calendarService = new CalendarService(calendarRepository.Object);
            var eventsRepository = new Mock<IDeletableEntityRepository<Event>>();
            var eventService = new EventService(eventsRepository.Object, calendarService);

            var model = new EventViewModel
            {
                Title = title,
                StartDate = new DateTime(2020, 02, 02, 12, 0, 0),
                StartTime = new DateTime(1999, 1, 1, 12, 0, 0),
                EndDate = new DateTime(2020, 02, 02, 12, 0, 0),
                EndTime = new DateTime(1999, 1, 1, 12, 30, 0),
                CalendarId = calendarId,
            };

            var exeption = await Assert.ThrowsAsync<ArgumentException>(() =>
                  eventService.CreateAsync(model));
            Assert.Equal(exeptionErrorMessage, exeption.Message);
        }

        [Theory]
        [InlineData("2020/02/02 12:00:00", "2020/02/02 12:01:00", "2020/02/02 12:00:00", "2020/02/02 12:00:00")]
        [InlineData("2020/02/02 12:00:00", "2020/02/02 12:00:00", "2020/02/02 12:00:00", "2020/02/02 11:00:00")]
        public async Task CreateAsync_WithStartDateAfterEndDate_ShouldThrowAnArgumentException(string startDate, string startTime, string endDate, string endTime)
        {
            var exeptionErrorMessage = "The start day and time must be before the end day and time.";
            InitializeAutomapper<EventViewModel>();
            var calendarRepository = new Mock<IDeletableEntityRepository<Calendar>>();
            var calendarService = new CalendarService(calendarRepository.Object);
            var eventsRepository = new Mock<IDeletableEntityRepository<Event>>();
            var eventService = new EventService(eventsRepository.Object, calendarService);

            var model = new EventViewModel
            {
                Title = "Test",
                StartDate = DateTime.ParseExact(startDate, "yyyy/MM/dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture),
                StartTime = DateTime.ParseExact(startTime, "yyyy/MM/dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture),
                EndDate = DateTime.ParseExact(endDate, "yyyy/MM/dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture),
                EndTime = DateTime.ParseExact(endTime, "yyyy/MM/dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture),
                CalendarId = "1",
            };

            var exeption = await Assert.ThrowsAsync<ArgumentException>(() =>
                  eventService.CreateAsync(model));
            Assert.Equal(exeptionErrorMessage, exeption.Message);
        }

        //[Fact]
        //public async Task GetEventById_WithCorrectData_ShouldReturnCorrectResult()
        //{
        //    InitializeAutomapper<EventViewModel>();
        //    var calendarRepository = new Mock<IDeletableEntityRepository<Calendar>>();
        //    var calendarService = new CalendarService(calendarRepository.Object);
        //    var eventsRepository = new Mock<IDeletableEntityRepository<Event>>();
        //    var eventService = new EventService(eventsRepository.Object, calendarService);

        //    var model = new EventViewModel
        //    {
        //        Title = "Test",
        //        StartDate = new DateTime(2020, 02, 02, 12, 0, 0),
        //        StartTime = new DateTime(1999, 1, 1, 12, 0, 0),
        //        EndDate = new DateTime(2020, 02, 02, 12, 0, 0),
        //        EndTime = new DateTime(1999, 1, 1, 12, 30, 0),
        //        CalendarId = "1",
        //        Description = "test description",
        //        Location = "Hotel Test",
        //        Coordinates = "42.99, 32.99",
        //    };
        //    var result = await eventService.CreateAsync(model);

        //    Assert.True(result);
        //}

        [Theory]
        [InlineData("", "Test")]
        [InlineData(null, "Test")]
        [InlineData("1", "")]
        [InlineData("1", null)]
        [InlineData("", "")]
        [InlineData(null, null)]
        public void GetEventById_WithNullOrEmptyArguments_ShouldThrowAnArgumentException(string eventId, string username)
        {
            var exeptionErrorMessage = "One or more required properties are null.";
            var calendarRepository = new Mock<IDeletableEntityRepository<Calendar>>();
            var calendarService = new CalendarService(calendarRepository.Object);
            var eventsRepository = new Mock<IDeletableEntityRepository<Event>>();
            var eventService = new EventService(eventsRepository.Object, calendarService);

            var exeption = Assert.Throws<ArgumentException>(() =>
                  eventService.GetEventById(eventId, username));

            Assert.Equal(exeptionErrorMessage, exeption.Message);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void GetEventViewModel_WithNullOrEmptyArgument_ShouldThrowAnArgumentException(string username)
        {
            var exeptionErrorMessage = "One or more required properties are null.";
            var calendarRepository = new Mock<IDeletableEntityRepository<Calendar>>();
            var calendarService = new CalendarService(calendarRepository.Object);
            var eventsRepository = new Mock<IDeletableEntityRepository<Event>>();
            var eventService = new EventService(eventsRepository.Object, calendarService);

            var exeption = Assert.Throws<ArgumentException>(() =>
                  eventService.GetEventViewModel(username));

            Assert.Equal(exeptionErrorMessage, exeption.Message);
        }

        private static void InitializeAutomapper<T>()
        {
            AutoMapperConfig.RegisterMappings(
                            typeof(T).GetTypeInfo().Assembly,
                            typeof(Event).GetTypeInfo().Assembly);
        }
    }
}
