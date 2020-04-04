namespace OrganizeMe.Services.Data.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;

    using Moq;
    using Nest;
    using OrganizeMe.Data.Common.Repositories;
    using OrganizeMe.Data.Models;
    using OrganizeMe.Services.Data.Calendar;
    using OrganizeMe.Services.Data.Color;
    using OrganizeMe.Services.Data.Events;
    using OrganizeMe.Services.Mapping;
    using OrganizeMe.Web.ViewModels.Calendar;
    using OrganizeMe.Web.ViewModels.Events;
    using Xunit;

    using Calendar = OrganizeMe.Data.Models.Calendar;

    public class EventsServiceTests
    {
        private Mock<IDeletableEntityRepository<Event>> eventsRepository;
        private EventService eventService;

        public EventsServiceTests()
        {
            var elasticClient = new ElasticClient();
            var calendarRepository = new Mock<IDeletableEntityRepository<Calendar>>();
            var colorRepository = new Mock<OrganizeMe.Data.Common.Repositories.IRepository<Color>>();
            var calendarService = new CalendarService(calendarRepository.Object);
            var colorService = new ColorService(colorRepository.Object);
            this.eventsRepository = new Mock<IDeletableEntityRepository<Event>>();
            this.eventService = new EventService(this.eventsRepository.Object, calendarService, elasticClient, colorService);
        }

        [Fact]
        public async Task CreateAsync_WithCorrectData_ShouldReturnCorrectResult()
        {
            var model = new EventViewModel
            {
                Title = "Test",
                StartDate = new DateTime(2020, 02, 02, 0, 0, 0),
                StartTime = new DateTime(0001, 1, 1, 12, 0, 0),
                EndDate = new DateTime(2020, 02, 02, 0, 0, 0),
                EndTime = new DateTime(0001, 1, 1, 12, 30, 0),
                CalendarId = "1",
                Description = "test description",
                Location = "Hotel Test",
                Coordinates = "42.99, 32.99",
                ColorId = 1,
            };

            var result = await this.eventService.CreateAsync(model);
            Assert.True(result);
        }

        [Fact]
        public async Task CreateAsync_WithCorrectData_ShouldCreateCorrectly()
        {
            var model = new EventViewModel
            {
                Title = "Test",
                StartDate = new DateTime(2020, 02, 02, 0, 0, 0),
                StartTime = new DateTime(0001, 1, 1, 12, 0, 0),
                EndDate = new DateTime(2020, 02, 02, 0, 0, 0),
                EndTime = new DateTime(0001, 1, 1, 12, 30, 0),
                CalendarId = "1",
                Description = "test description",
                Location = "Hotel Test",
                Coordinates = "42.99, 32.99",
                ColorId = 1,
            };

            var expectedResult = new Event
            {
                Title = "Test",
                StartDateTime = new DateTime(2020, 02, 02, 12, 0, 0),
                EndDateTime = new DateTime(2020, 02, 02, 12, 30, 0),
                CalendarId = "1",
                Description = "test description",
                Location = "Hotel Test",
                Coordinates = "42.99, 32.99",
                Color = new Color { Id = 1, Name = "First", Hex = "#000000" },
            };

            this.eventsRepository.Setup(x => x.All()).Returns(new List<Event> { expectedResult }.AsQueryable());
            await this.eventService.CreateAsync(model);
            var actualResult = this.eventsRepository.Object.All().FirstOrDefault();

            this.eventsRepository.Verify(x => x.All(), Times.Once);

            Assert.Equal(expectedResult.Title, actualResult.Title);
            Assert.Equal(expectedResult.CalendarId, actualResult.CalendarId);
            Assert.Equal(expectedResult.Location, actualResult.Location);
            Assert.Equal(expectedResult.Coordinates, actualResult.Coordinates);
            Assert.Equal(expectedResult.Description, actualResult.Description);
            Assert.Equal(expectedResult.StartDateTime, actualResult.StartDateTime);
            Assert.Equal(expectedResult.EndDateTime, actualResult.EndDateTime);
            Assert.Equal(expectedResult.ColorId, actualResult.ColorId);
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

            var model = new EventViewModel
            {
                Title = title,
                StartDate = new DateTime(2020, 02, 02, 0, 0, 0),
                StartTime = new DateTime(0001, 1, 1, 12, 0, 0),
                EndDate = new DateTime(2020, 02, 02, 0, 0, 0),
                EndTime = new DateTime(0001, 1, 1, 12, 30, 0),
                CalendarId = calendarId,
                ColorId = 1,
            };

            var exeption = await Assert.ThrowsAsync<ArgumentException>(() =>
                  this.eventService.CreateAsync(model));
            Assert.Equal(exeptionErrorMessage, exeption.Message);
        }

        [Theory]
        [InlineData("2020/02/02 12:00:00", "2020/02/02 12:01:00", "2020/02/02 12:00:00", "2020/02/02 12:00:00")]
        [InlineData("2020/02/02 12:00:00", "2020/02/02 12:00:00", "2020/02/02 12:00:00", "2020/02/02 11:00:00")]
        [InlineData("2020/02/02 23:59:00", "2020/02/02 23:59:00", "2020/02/02 00:30:00", "2020/02/02 00:30:00")]
        public async Task CreateAsync_WithStartDateAfterEndDate_ShouldThrowAnArgumentException(string startDate, string startTime, string endDate, string endTime)
        {
            var exeptionErrorMessage = "The start day and time must be before the end day and time.";

            var model = new EventViewModel
            {
                Title = "Test",
                StartDate = DateTime.ParseExact(startDate, "yyyy/MM/dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture),
                StartTime = DateTime.ParseExact(startTime, "yyyy/MM/dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture),
                EndDate = DateTime.ParseExact(endDate, "yyyy/MM/dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture),
                EndTime = DateTime.ParseExact(endTime, "yyyy/MM/dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture),
                CalendarId = "1",
                ColorId = 1,
            };

            var exeption = await Assert.ThrowsAsync<ArgumentException>(() =>
                  this.eventService.CreateAsync(model));
            Assert.Equal(exeptionErrorMessage, exeption.Message);
        }

        [Fact]
        public async Task GetEditViewModelByIdAsync_WithCorrectData_ShouldReturnCorrectResult()
        {
            InitializeAutomapper<EventViewModel>();
            var user = new ApplicationUser
            {
                Id = "User1",
                UserName = "Username",
            };

            var model = new Event
            {
                Id = "Test1",
                Title = "Test",
                StartDateTime = new DateTime(2020, 02, 02, 12, 0, 0),
                EndDateTime = new DateTime(2020, 02, 02, 12, 30, 0),
                CalendarId = "1",
                Description = "test description",
                Location = "Hotel Test",
                Coordinates = "42.99, 32.99",
                Color = new Color { Id = 1, Name = "First", Hex = "#000000" },
            };

            var eventViewModel = new EventViewModel
            {
                Title = "Test",
                StartDate = new DateTime(2020, 02, 02, 0, 0, 0),
                StartTime = new DateTime(0001, 1, 1, 12, 0, 0),
                EndDate = new DateTime(2020, 02, 02, 0, 0, 0),
                EndTime = new DateTime(0001, 1, 1, 12, 30, 0),
                CalendarId = "1",
                Description = "test description",
                Location = "Hotel Test",
                Coordinates = "42.99, 32.99",
                ColorId = 1,
            };

            this.eventsRepository.Setup(x => x.All()).Returns(new List<Event> { model }.AsQueryable());
            var actualResult = await this.eventService.GetEditViewModelByIdAsync(model.Id, user.UserName);
            var expectedResult = eventViewModel;
            var actualResultOutput = actualResult.Output;

            this.eventsRepository.Verify(x => x.All(), Times.Once);

            Assert.Equal(expectedResult.Title, actualResultOutput.Title);
            Assert.Equal(expectedResult.CalendarId, actualResultOutput.CalendarId);
            Assert.Equal(expectedResult.Location, actualResultOutput.Location);
            Assert.Equal(expectedResult.Coordinates, actualResultOutput.Coordinates);
            Assert.Equal(expectedResult.Description, actualResultOutput.Description);
            Assert.Equal(expectedResult.StartDateTime, actualResultOutput.StartDateTime);
            Assert.Equal(expectedResult.EndDateTime, actualResultOutput.EndDateTime);
            Assert.Equal(expectedResult.ColorId, actualResultOutput.ColorId);
        }

        [Theory]
        [InlineData("", "Test")]
        [InlineData(null, "Test")]
        [InlineData("1", "")]
        [InlineData("1", null)]
        [InlineData("", "")]
        [InlineData(null, null)]
        public async Task GetEditViewModelByIdAsync_WithNullOrEmptyArguments_ShouldThrowAnArgumentException(string eventId, string username)
        {
            var exeptionErrorMessage = "One or more required properties are null.";

            var exeption = await Assert.ThrowsAsync<ArgumentException>(() =>
                  this.eventService.GetEditViewModelByIdAsync(eventId, username));

            Assert.Equal(exeptionErrorMessage, exeption.Message);
        }

        [Fact]
        public async Task GetCreateViewModelAsync_WithCorrectData_ShouldReturnCorrectly()
        {
            InitializeAutomapper<EventViewModel>();
            var user = new ApplicationUser
            {
                Id = "User1",
                UserName = "Username",
            };

            var eventViewModel = new EventViewModel
            {
                StartDate = DateTime.Now,
                StartTime = DateTime.Now,
                EndDate = DateTime.Now,
                EndTime = DateTime.Now.AddMinutes(30),
                ColorId = 1,
            };

            var actualResult = await this.eventService.GetCreateViewModelAsync(user.UserName);
            var expectedResult = eventViewModel;
            var actualResultOutput = actualResult.Input;

            Assert.Equal(expectedResult.Title, actualResultOutput.Title);
            Assert.Equal(expectedResult.CalendarId, actualResultOutput.CalendarId);
            Assert.Equal(expectedResult.Location, actualResultOutput.Location);
            Assert.Equal(expectedResult.Coordinates, actualResultOutput.Coordinates);
            Assert.Equal(expectedResult.Description, actualResultOutput.Description);
            Assert.Equal(expectedResult.StartDateTime, actualResultOutput.StartDateTime);
            Assert.Equal(expectedResult.EndDateTime, actualResultOutput.EndDateTime);
            Assert.Equal(expectedResult.ColorId, actualResultOutput.ColorId);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public async Task GetCreateViewModelAsync_WithNullOrEmptyArgument_ShouldThrowAnArgumentException(string username)
        {
            var exeptionErrorMessage = "One or more required properties are null.";

            var exeption = await Assert.ThrowsAsync<ArgumentException>(() =>
                  this.eventService.GetCreateViewModelAsync(username));

            Assert.Equal(exeptionErrorMessage, exeption.Message);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public async Task GetByIdAsync_WithNullOrEmptyArgument_ShouldThrowAnArgumentException(string id)
        {
            var exeptionErrorMessage = "One or more required properties are null.";

            var exeption = await Assert.ThrowsAsync<ArgumentException>(() =>
                  this.eventService.GetByIdAsync(id));

            Assert.Equal(exeptionErrorMessage, exeption.Message);
        }

        [Fact]
        public async Task GetByIdAsync_WithCorrectData_ShouldReturnCorrectResult()
        {
            InitializeAutomapper<EventViewModel>();

            var model = new Event
            {
                Id = "Test1",
                Title = "Test",
                StartDateTime = new DateTime(2020, 02, 02, 12, 0, 0),
                EndDateTime = new DateTime(2020, 02, 02, 12, 30, 0),
                CalendarId = "1",
                Description = "test description",
                Location = "Hotel Test",
                Coordinates = "42.99, 32.99",
                Color = new Color { Id = 1, Name = "First", Hex = "#000000" },
            };

            this.eventsRepository.Setup(x => x.GetByIdWithDeletedAsync(model.Id)).ReturnsAsync(model);
            var actualResult = await this.eventService.GetByIdAsync(model.Id);
            var expectedResult = model;

            this.eventsRepository.Verify(x => x.GetByIdWithDeletedAsync(model.Id), Times.Once);

            Assert.Equal(expectedResult.Title, actualResult.Title);
            Assert.Equal(expectedResult.CalendarId, actualResult.CalendarId);
            Assert.Equal(expectedResult.Location, actualResult.Location);
            Assert.Equal(expectedResult.Coordinates, actualResult.Coordinates);
            Assert.Equal(expectedResult.Description, actualResult.Description);
            Assert.Equal(expectedResult.StartDateTime, actualResult.StartDateTime);
            Assert.Equal(expectedResult.EndDateTime, actualResult.EndDateTime);
            Assert.Equal(expectedResult.Color, actualResult.Color);
        }

        [Fact]
        public async Task GetByIdAsync_WithIncorrectUsername_ShouldThrowAnArgumentNullException()
        {
            var exeptionErrorMessage = "Event with Id: {0} does not exist.";
            InitializeAutomapper<EventViewModel>();

            var model = new Event
            {
                Id = "Test1",
                Title = "Test",
                StartDateTime = new DateTime(2020, 02, 02, 12, 0, 0),
                EndDateTime = new DateTime(2020, 02, 02, 12, 30, 0),
                CalendarId = "1",
                Description = "test description",
                Location = "Hotel Test",
                Coordinates = "42.99, 32.99",
                Color = new Color { Id = 1, Name = "First", Hex = "#000000" },
            };

            var exeption = await Assert.ThrowsAsync<ArgumentException>(() =>
                 this.eventService.GetByIdAsync(model.Id));
            Assert.Equal(string.Format(exeptionErrorMessage, model.Id), exeption.Message);
        }

        [Fact]
        public async Task GetAllByCalendarIdAsync_WithCorrectData_ShouldReturnCorrectResult()
        {
            InitializeAutomapper<EventViewModel>();
            InitializeAutomapper<EventCalendarViewModel>();

            var calendar = new Calendar
            {
                Id = "Test1",
                Title = "Default",
            };

            var model = new Event
            {
                Id = "Test1",
                Title = "Test",
                StartDateTime = new DateTime(2020, 02, 02, 12, 0, 0),
                EndDateTime = new DateTime(2020, 02, 02, 12, 30, 0),
                Description = "test description",
                Location = "Hotel Test",
                Coordinates = "42.99, 32.99",
                Calendar = calendar,
                CalendarId = calendar.Id,
                Color = new Color { Id = 1, Name = "First", Hex = "#000000" },
            };

            var eventCalendarViewModel = new EventCalendarViewModel
            {
                Title = "Test",
                StartDateTime = new DateTime(2020, 02, 02, 12, 0, 0),
                EndDateTime = new DateTime(2020, 02, 02, 12, 30, 0),
            };

            this.eventsRepository.Setup(x => x.All()).Returns(new List<Event> { model }.AsQueryable());
            var actualResultColleciton = await this.eventService.GetAllByCalendarIdAsync(calendar.Id);
            var expectedResult = eventCalendarViewModel;
            var actualResult = actualResultColleciton.First();

            this.eventsRepository.Verify(x => x.All(), Times.Once);
            Assert.Single(actualResultColleciton);

            Assert.Equal(expectedResult.Title, actualResult.Title);
            Assert.Equal(expectedResult.StartDateTime, actualResult.StartDateTime);
            Assert.Equal(expectedResult.EndDateTime, actualResult.EndDateTime);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public async Task GetAllByCalendarIdAsync_WithNullOrEmptyArgument_ShouldThrowAnArgumentException(string id)
        {
            var exeptionErrorMessage = "One or more required properties are null.";

            var exeption = await Assert.ThrowsAsync<ArgumentException>(() =>
                 this.eventService.GetAllByCalendarIdAsync(id));

            Assert.Equal(exeptionErrorMessage, exeption.Message);
        }

        [Fact]
        public async Task UpdateAsync_WithCorrectData_ShouldReturnCorrectResult()
        {
            InitializeAutomapper<EventViewModel>();

            var model = new Event
            {
                Id = "Test1",
                Title = "Test",
                StartDateTime = new DateTime(2020, 02, 02, 12, 0, 0),
                EndDateTime = new DateTime(2020, 02, 02, 12, 30, 0),
                Description = "test description",
                Location = "Hotel Test",
                Coordinates = "42.99, 32.99",
                CalendarId = "1",
                Color = new Color { Id = 1, Name = "First", Hex = "#000000" },
            };

            var eventViewModel = new EventViewModel
            {
                Title = "Test",
                StartDate = new DateTime(2020, 02, 02, 0, 0, 0),
                StartTime = new DateTime(0001, 1, 1, 12, 0, 0),
                EndDate = new DateTime(2020, 02, 02, 0, 0, 0),
                EndTime = new DateTime(0001, 1, 1, 12, 30, 0),
                CalendarId = "1",
                Description = "test description",
                Location = "Hotel Test",
                Coordinates = "42.99, 32.99",
                ColorId = 1,
            };

            var eventEditViewModel = new EventEditViewModel
            {
                Output = eventViewModel,
            };

            var result = await this.eventService.UpdateAsync(eventEditViewModel, model.Id);

            // this.eventsRepository.Verify(x => x.Update(model), Times.Once);
            Assert.True(result);
        }

        private static void InitializeAutomapper<T>()
        {
            AutoMapperConfig.RegisterMappings(
                            typeof(T).GetTypeInfo().Assembly,
                            typeof(Event).GetTypeInfo().Assembly);
        }
    }
}
