﻿namespace BeOrganized.Services.Data.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;

    using BeOrganized.Data.Common.Repositories;
    using BeOrganized.Data.Models;
    using BeOrganized.Services.Data.Calendar;
    using BeOrganized.Services.Data.Color;
    using BeOrganized.Services.Data.Events;
    using BeOrganized.Services.Mapping;
    using BeOrganized.Web.ViewModels.Calendar;
    using BeOrganized.Web.ViewModels.Events;
    using Moq;
    using Nest;
    using Xunit;

    using Calendar = BeOrganized.Data.Models.Calendar;

    public class EventsServiceTests
    {
        private Mock<IDeletableEntityRepository<Calendar>> calendarRepository;
        private Mock<BeOrganized.Data.Common.Repositories.IRepository<Color>> colorRepository;
        private Mock<IDeletableEntityRepository<Event>> eventsRepository;
        private EventService eventService;
        private Mock<ISearchService> searchService;

        public EventsServiceTests()
        {
            this.InitializeMapper();
            this.searchService = new Mock<ISearchService>();
            this.colorRepository = new Mock<BeOrganized.Data.Common.Repositories.IRepository<Color>>();
            this.calendarRepository = new Mock<IDeletableEntityRepository<Calendar>>();
            this.eventsRepository = new Mock<IDeletableEntityRepository<Event>>();

            var colorService = new ColorService(this.colorRepository.Object);
            var calendarService = new CalendarService(this.calendarRepository.Object, colorService);
            this.eventService = new EventService(this.eventsRepository.Object, calendarService, this.searchService.Object, colorService);
        }

        [Fact]
        public async Task CreateAsync_WithCorrectData_ShouldReturnCorrectResult()
        {
            var viewModel = new EventViewModel
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

            this.searchService.Setup(x => x.CreateIndexAsync<Event>(It.IsAny<Event>())).ReturnsAsync(Result.Created);
            var result = await this.eventService.CreateAsync(viewModel);

            this.eventsRepository.Verify(m => m.AddAsync(It.IsAny<Event>()), Times.Once);
            this.eventsRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
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

            this.searchService.Setup(x => x.CreateIndexAsync<Event>(It.IsAny<Event>())).ReturnsAsync(Result.Created);
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
        public void GetEditChangeViewModelById_WithCorrectData_ShouldReturnCorrectResult()
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

            var model = new Event
            {
                Id = "Test1",
                Title = "Test",
                StartDateTime = new DateTime(2020, 02, 02, 12, 0, 0),
                EndDateTime = new DateTime(2020, 02, 02, 12, 30, 0),
                CalendarId = calendar.Id,
                Calendar = calendar,
                Description = "test description",
                Location = "Hotel Test",
                Coordinates = "42.99, 32.99",
                ColorId = 1,
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

            this.calendarRepository
                .Setup(x => x.All())
                .Returns(new List<Calendar> { calendar }
                .AsQueryable());
            this.colorRepository.Setup(x => x.All()).Returns(new List<Color> { color }.AsQueryable());
            this.eventsRepository.Setup(x => x.All()).Returns(new List<Event> { model }.AsQueryable());
            var actualResult = this.eventService.GetEditChangeViewModelById(model.Id, user.UserName);
            var expectedResult = eventViewModel;
            var actualResultOutput = actualResult.EventModel;

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
        public void GetEditChangeViewModelById_WithNullOrEmptyArguments_ShouldThrowAnArgumentException(string eventId, string username)
        {
            var exeptionErrorMessage = "One or more required properties are null.";

            var exeption = Assert.Throws<ArgumentException>(() =>
                 this.eventService.GetEditChangeViewModelById(eventId, username));

            Assert.Equal(exeptionErrorMessage, exeption.Message);
        }

        [Fact]
        public void GetCreateChangeViewModel_WithCorrectData_ShouldReturnCorrectly()
        {
            var currentDateTime = DateTime.Now;
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

            var eventViewModel = new EventViewModel
            {
                StartDate = DateTime.Now,
                StartTime = DateTime.Now,
                EndDate = DateTime.Now.AddMinutes(30),
                EndTime = DateTime.Now.AddMinutes(30),
                ColorId = 1,
            };

            this.calendarRepository
                .Setup(x => x.All())
                .Returns(new List<Calendar> { calendar }
                .AsQueryable());
            this.colorRepository.Setup(x => x.All()).Returns(new List<Color> { color }.AsQueryable());
            var actualResult = this.eventService.GetCreateChangeViewModel(user.UserName, currentDateTime);
            var expectedResult = eventViewModel;
            var actualResultOutput = actualResult.EventModel;

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
        public void GetCreateChangeViewModel_WithNullOrEmptyArgument_ShouldThrowAnArgumentException(string username)
        {
            var exeptionErrorMessage = "One or more required properties are null.";

            var exeption = Assert.Throws<ArgumentException>(() =>
                 this.eventService.GetCreateChangeViewModel(username, DateTime.Now));

            Assert.Equal(exeptionErrorMessage, exeption.Message);
        }

        [Fact]
        public void GetDetailsViewModelById_WithCorrectData_ShouldReturnCorrectResult()
        {
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
                UserId = "UserId",
            };

            var model = new Event
            {
                Id = "Test1",
                Title = "Test",
                StartDateTime = DateTime.Today,
                EndDateTime = DateTime.Today.AddMinutes(30),
                CalendarId = calendar.Id,
                Calendar = calendar,
                Description = "test description",
                Location = "Hotel Test",
                Coordinates = "42.99, 32.99",
                ColorId = color.Id,
                Color = color,
            };

            var eventViewModel = new EventDetailsViewModel
            {
                Id = "Test1",
                Title = "Test",
                StartDateTime = DateTime.Today,
                EndDateTime = DateTime.Today.AddMinutes(30),
                CalendarTitle = "Default",
                Description = "test description",
                Location = "Hotel Test",
                ColorHex = color.Hex,
                CalendarId = calendar.Id,
            };

            this.eventsRepository.Setup(x => x.AllAsNoTracking()).Returns(new List<Event> { model }.AsQueryable());
            var actualResult = this.eventService.GetDetailsViewModelById(model.Id);
            var expectedResult = eventViewModel;

            this.eventsRepository.Verify(x => x.AllAsNoTracking(), Times.Once);

            Assert.Equal(expectedResult.Id, actualResult.Id);
            Assert.Equal(expectedResult.Title, actualResult.Title);
            Assert.Equal(expectedResult.CalendarTitle, actualResult.CalendarTitle);
            Assert.Equal(expectedResult.Location, actualResult.Location);
            Assert.Equal(expectedResult.Description, actualResult.Description);
            Assert.Equal(expectedResult.StartDateTime, actualResult.StartDateTime);
            Assert.Equal(expectedResult.EndDateTime, actualResult.EndDateTime);
            Assert.Equal(expectedResult.ColorHex, actualResult.ColorHex);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void GetDetailsViewModelById_WithNullOrEmptyArguments_ShouldThrowAnArgumentException(string eventId)
        {
            var exeptionErrorMessage = "One or more required properties are null.";

            var exeption = Assert.Throws<ArgumentException>(() =>
                 this.eventService.GetDetailsViewModelById(eventId));

            Assert.Equal(exeptionErrorMessage, exeption.Message);
        }

        [Fact]
        public async Task GetByIdAsync_WithCorrectData_ShouldReturnCorrectResult()
        {
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

            this.eventsRepository.Setup(x => x.GetByIdWithDeletedAsync(It.IsAny<string>())).ReturnsAsync(model);
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
        public async Task GetByIdAsync_WithIncorrectUsername_ShouldThrowAnArgumentNullException()
        {
            var exeptionErrorMessage = "Event with Id: {0} does not exist.";

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
        public void GetAllByCalendarId_WithCorrectData_ShouldReturnCorrectResult()
        {
            var calendar = new Calendar
            {
                Id = "Test1",
                Title = "Default",
            };

            var color = new Color
            {
                Id = 1,
                Name = "Test",
                Hex = "TestHex",
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
                Color = color,
            };

            var eventCalendarViewModel = new EventCalendarViewModel
            {
                Id = "Test1",
                Title = "Test",
                StartDateTime = new DateTime(2020, 02, 02, 12, 0, 0),
                EndDateTime = new DateTime(2020, 02, 02, 12, 30, 0),
                CalendarId = "Test1",
                Description = "test description",
                Location = "Hotel Test",
                Coordinates = "42.99, 32.99",
                ColorHex = color.Hex,
            };

            this.eventsRepository.Setup(x => x.All()).Returns(new List<Event> { model }.AsQueryable());
            var actualResultColleciton = this.eventService.GetAllByCalendarId(calendar.Id);
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
        public void GetAllByCalendarId_WithNullOrEmptyArgument_ShouldThrowAnArgumentException(string id)
        {
            var exeptionErrorMessage = "One or more required properties are null.";

            var exeption = Assert.Throws<ArgumentException>(() =>
                this.eventService.GetAllByCalendarId(id));

            Assert.Equal(exeptionErrorMessage, exeption.Message);
        }

        [Fact]
        public async Task UpdateAsync_WithCorrectData_ShouldReturnCorrectResult()
        {
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
                ColorId = 1,
            };

            this.searchService.Setup(x => x.UpdateIndexAsync<Event>(It.IsAny<Event>())).ReturnsAsync(Result.Updated);
            var result = await this.eventService.UpdateAsync(model, model.Id);

            this.eventsRepository.Verify(x => x.Update(It.IsAny<Event>()), Times.Once);
            this.eventsRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public async Task UpdateAsync_WithNullOrEmptyArgument_ShouldThrowAnArgumentException(string id)
        {
            var exeptionErrorMessage = "One or more required properties are null.";

            var correctEvent = new Event
            {
                Id = "Test1",
                Title = "Test",
                StartDateTime = new DateTime(2020, 02, 02, 12, 0, 0),
                EndDateTime = new DateTime(2020, 02, 02, 12, 30, 0),
                Description = "test description",
                Location = "Hotel Test",
                Coordinates = "42.99, 32.99",
                CalendarId = "1",
                ColorId = 1,
            };

            Event incorrectEventEditViewModel = null;

            var exeption1 = await Assert.ThrowsAsync<ArgumentException>(() =>
                this.eventService.UpdateAsync(correctEvent, id));
            var exeption2 = await Assert.ThrowsAsync<ArgumentException>(() =>
               this.eventService.UpdateAsync(incorrectEventEditViewModel, id));
            var exeption3 = await Assert.ThrowsAsync<ArgumentException>(() =>
              this.eventService.UpdateAsync(incorrectEventEditViewModel, "1"));

            Assert.Equal(exeptionErrorMessage, exeption1.Message);

            Assert.Equal(exeptionErrorMessage, exeption2.Message);

            Assert.Equal(exeptionErrorMessage, exeption3.Message);
        }

        [Fact]
        public async Task DeleteAsync_WithCorrectData_ShouldReturnCorrectResult()
        {
            var model = new Event
            {
                Id = "Test1",
                Title = "Test1",
                StartDateTime = new DateTime(2020, 02, 02, 12, 0, 0),
                EndDateTime = new DateTime(2020, 02, 02, 12, 30, 0),
                Description = "test description",
                Location = "Hotel Test",
                Coordinates = "42.99, 32.99",
                CalendarId = "1",
                ColorId = 1,
            };

            var eventViewModel = new EventViewModel
            {
                Title = "Test1",
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

            var eventEditViewModel = new EventChangeViewModel
            {
                EventModel = eventViewModel,
            };

            this.searchService.Setup(x => x.DeleteIndexAsync<Event>(It.IsAny<Event>())).ReturnsAsync(Result.Deleted);
            this.eventsRepository.Setup(x => x.GetByIdWithDeletedAsync(It.IsAny<string>())).ReturnsAsync(model);
            var result = await this.eventService.DeleteAsync(model.Id);

            this.eventsRepository.Verify(x => x.Delete(It.IsAny<Event>()), Times.Once);
            this.eventsRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public async Task DeleteAsync_WithNullOrEmptyArgument_ShouldThrowAnArgumentException(string id)
        {
            var exeptionErrorMessage = "One or more required properties are null.";

            var exeption = await Assert.ThrowsAsync<ArgumentException>(() =>
                this.eventService.DeleteAsync(id));

            Assert.Equal(exeptionErrorMessage, exeption.Message);
        }

        [Fact]
        public async Task DeleteAsync_WithNotExistingModel_ShouldThrowAnArgumentException()
        {
            var model = new Event
            {
                Id = "Test1",
                Title = "Test1",
                StartDateTime = new DateTime(2020, 02, 02, 12, 0, 0),
                EndDateTime = new DateTime(2020, 02, 02, 12, 30, 0),
                Description = "test description",
                Location = "Hotel Test",
                Coordinates = "42.99, 32.99",
                CalendarId = "1",
                ColorId = 1,
            };

            var exeptionErrorMessage = $"Event with Id: {model.Id} does not exist.";
            var exeption = await Assert.ThrowsAsync<ArgumentException>(() =>
                this.eventService.DeleteAsync(model.Id));

            Assert.Equal(exeptionErrorMessage, exeption.Message);
        }

        [Fact]
        public void GetAll_WithCorrectData_ShouldReturnCorrectResult()
        {
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
                ColorId = 1,
                Calendar = new Calendar
                {
                    Id = "1",
                    UserId = "1",
                },
            };

            this.eventsRepository.Setup(x => x.All()).Returns(new List<Event> { model }.AsQueryable());
            var actualResult = this.eventService.GetAll();

            this.eventsRepository.Verify(x => x.All(), Times.Once);

            Assert.Equal(1, actualResult.Count);
        }

        [Fact]
        public void GetAllByUsername_WithCorrectData_ShouldReturnCorrectResult()
        {
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
                ColorId = 1,
                Calendar = new Calendar
                {
                    Id = "1",
                    User = user,
                },
            };

            this.eventsRepository.Setup(x => x.All()).Returns(new List<Event> { model }.AsQueryable());
            var actualResult = this.eventService.GetAllByUsername(user.UserName);

            this.eventsRepository.Verify(x => x.All(), Times.Once);

            Assert.Equal(1, actualResult.Count);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void GetAllByUsername_WithNullOrEmptyArgument_ShouldThrowAnArgumentException(string username)
        {
            var exeptionErrorMessage = "One or more required properties are null.";

            var exeption = Assert.Throws<ArgumentException>(() =>
               this.eventService.GetAllByUsername(username));

            Assert.Equal(exeptionErrorMessage, exeption.Message);
        }

        [Fact]
        public void MapEventViewModelToEvent_WithCorrectData_ShouldReturnCorrectResult()
        {
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
                ColorId = 1,
                Calendar = new Calendar
                {
                    Id = "1",
                    User = user,
                },
            };

            var eventViewModel = new EventViewModel
            {
                Title = "Test1",
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
            var actualResult = this.eventService.MapEventViewModelToEvent(eventViewModel, model.Id);
            var expectedResult = model;

            this.eventsRepository.Verify(x => x.All(), Times.Once);

            Assert.Equal(expectedResult.Title, actualResult.Title);
            Assert.Equal(expectedResult.CalendarId, actualResult.CalendarId);
            Assert.Equal(expectedResult.Location, actualResult.Location);
            Assert.Equal(expectedResult.Coordinates, actualResult.Coordinates);
            Assert.Equal(expectedResult.Description, actualResult.Description);
            Assert.Equal(expectedResult.StartDateTime, actualResult.StartDateTime);
            Assert.Equal(expectedResult.EndDateTime, actualResult.EndDateTime);
            Assert.Equal(expectedResult.Color, actualResult.Color);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void MapEventViewModelToEvent_WithWithNullOrEmptyArgument_ShouldThrowAnArgumentException(string username)
        {
            var exeptionErrorMessage = "One or more required properties are null.";

            var exeption = Assert.Throws<ArgumentException>(() =>
               this.eventService.GetAllByUsername(username));

            Assert.Equal(exeptionErrorMessage, exeption.Message);
        }

        private void InitializeMapper() => AutoMapperConfig.
           RegisterMappings(Assembly.Load("BeOrganized.Web.ViewModels"));
    }
}
