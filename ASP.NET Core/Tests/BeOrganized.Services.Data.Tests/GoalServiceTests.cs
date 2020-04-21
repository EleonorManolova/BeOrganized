namespace BeOrganized.Services.Data.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using BeOrganized.Data.Common.Repositories;
    using BeOrganized.Data.Models;
    using BeOrganized.Data.Models.Enums;
    using BeOrganized.Services;
    using BeOrganized.Services.Data.Calendar;
    using BeOrganized.Services.Data.Color;
    using BeOrganized.Services.Data.Goal;
    using BeOrganized.Services.Data.Habit;
    using BeOrganized.Web.ViewModels.Golas;
    using Moq;
    using Xunit;

    public class GoalServiceTests
    {
        private Mock<IDeletableEntityRepository<Goal>> goalRepository;
        private GoalService goalService;
        private Mock<IEnumParseService> enumParseService;
        private Mock<ICalendarService> calendarService;
        private Mock<IColorService> colorService;
        private Mock<IHabitService> habitService;

        public GoalServiceTests()
        {
            this.enumParseService = new Mock<IEnumParseService>();
            this.calendarService = new Mock<ICalendarService>();
            this.colorService = new Mock<IColorService>();
            this.habitService = new Mock<IHabitService>();
            this.goalRepository = new Mock<IDeletableEntityRepository<Goal>>();
            this.goalService = new GoalService(this.goalRepository.Object, this.enumParseService.Object, this.calendarService.Object, this.colorService.Object, this.habitService.Object);
        }

        [Fact]
        public async Task CreateAsync_WithCorrectData_ShouldReturnCorrectResult()
        {
            var viewModel = new GoalViewModel
            {
                Title = "Test",
                ColorId = 1,
                CalendarId = "1",
                DayTime = DayTime.Afternoon.ToString(),
                Duration = Duration.HourAndHalf.ToString(),
                Frequency = Frequency.EveryDay.ToString(),
            };

            var result = await this.goalService.CreateAsync(viewModel);

            this.goalRepository.Verify(m => m.AddAsync(It.IsAny<Goal>()), Times.Once);
            this.goalRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_WithCorrectData_ShouldCreateCorrectly()
        {
            var viewModel = new GoalViewModel
            {
                Title = "Test",
                ColorId = 1,
                CalendarId = "1",
                DayTime = DayTime.Afternoon.ToString(),
                Duration = Duration.HourAndHalf.ToString(),
                Frequency = Frequency.EveryDay.ToString(),
            };

            var expectedResult = new Goal
            {
                Id = "1",
                Title = "Test",
                ColorId = 1,
                CalendarId = "1",
                DayTime = DayTime.Afternoon,
                Duration = Duration.HourAndHalf,
                Frequency = Frequency.EveryDay,
                IsActive = true,
            };

            this.goalRepository.Setup(x => x.All()).Returns(new List<Goal> { expectedResult }.AsQueryable());
            await this.goalService.CreateAsync(viewModel);
            var actualResult = this.goalRepository.Object.All().FirstOrDefault();

            this.goalRepository.Verify(x => x.All(), Times.Once);

            Assert.Equal(expectedResult.Title, actualResult.Title);
            Assert.Equal(expectedResult.CalendarId, actualResult.CalendarId);
            Assert.Equal(expectedResult.IsActive, actualResult.IsActive);
            Assert.Equal(expectedResult.ColorId, actualResult.ColorId);
            Assert.Equal(expectedResult.StartDateTime, actualResult.StartDateTime);
            Assert.Equal(expectedResult.DayTime, actualResult.DayTime);
            Assert.Equal(expectedResult.Duration, actualResult.Duration);
            Assert.Equal(expectedResult.Frequency, actualResult.Frequency);
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

            var model = new GoalViewModel
            {
                Title = title,
                DayTime = DayTime.Afternoon.ToString(),
                Duration = Duration.HourAndHalf.ToString(),
                Frequency = Frequency.EveryDay.ToString(),
                CalendarId = calendarId,
                ColorId = 1,
            };

            var exeption = await Assert.ThrowsAsync<ArgumentException>(() =>
                  this.goalService.CreateAsync(model));
            Assert.Equal(exeptionErrorMessage, exeption.Message);
        }

        [Fact]
        public void GetGoalChangeViewModel_WithCorrectData_ShouldReturnCorrectResult()
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

            var calendarViewModel = new CalendarHabitViewModel
            {
                Id = calendar.Id,
                Title = calendar.Title,
            };

            var model = new Goal
            {
                Id = "1",
                Title = "Test",
                ColorId = color.Id,
                Color = color,
                CalendarId = calendar.Id,
                Calendar = calendar,
                DayTime = DayTime.Afternoon,
                Duration = Duration.HourAndHalf,
                Frequency = Frequency.EveryDay,
                IsActive = true,
            };

            var viewModel = new GoalViewModel
            {
                Title = "Test",
                ColorId = 1,
                CalendarId = "1",
                DayTime = DayTime.Afternoon.ToString(),
                Duration = Duration.HourAndHalf.ToString(),
                Frequency = Frequency.EveryDay.ToString(),
            };

            this.calendarService
                .Setup(x => x.GetAllCalendarTitlesByUserName<CalendarHabitViewModel>(It.IsAny<string>()))
                .Returns(new List<CalendarHabitViewModel> { calendarViewModel });
            this.colorService.Setup(x => x.GetAllColors()).Returns(new List<Color> { color });
            var actualResult = this.goalService.GetGoalChangeViewModel(user.UserName);
            var expectedResult = viewModel;

            Assert.Equal(1, actualResult.Calendars.Count);
            Assert.Equal(1, actualResult.Colors.Count);
            Assert.Equal(4, actualResult.DayTimes.Count());
            Assert.Equal(9, actualResult.Frequencies.Count());
            Assert.Equal(4, actualResult.Durations.Count());
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void GetGoalChangeViewModel_WithNullOrEmptyArguments_ShouldThrowAnArgumentException(string username)
        {
            var exeptionErrorMessage = "One or more required properties are null.";

            var exeption = Assert.Throws<ArgumentException>(() =>
                 this.goalService.GetGoalChangeViewModel(username));

            Assert.Equal(exeptionErrorMessage, exeption.Message);
        }

        [Fact]
        public async Task UpdateAsync_WithCorrectData_ShouldReturnCorrectResult()
        {
            var model = new Goal
            {
                Id = "1",
                Title = "Test",
                ColorId = 1,
                CalendarId = "1",
                DayTime = DayTime.Afternoon,
                Duration = Duration.HourAndHalf,
                Frequency = Frequency.EveryDay,
                IsActive = true,
            };

            var result = await this.goalService.UpdateAsync(model, model.Id);

            this.goalRepository.Verify(x => x.Update(It.IsAny<Goal>()), Times.Once);
            this.goalRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public async Task UpdateAsync_WithNullOrEmptyArgument_ShouldThrowAnArgumentException(string id)
        {
            var exeptionErrorMessage = "One or more required properties are null.";

            var correctGoal = new Goal
            {
                Id = "1",
                Title = "Test",
                ColorId = 1,
                CalendarId = "1",
                DayTime = DayTime.Afternoon,
                Duration = Duration.HourAndHalf,
                Frequency = Frequency.EveryDay,
                IsActive = true,
            };

            Goal incorrectGoal = null;

            var exeption1 = await Assert.ThrowsAsync<ArgumentException>(() =>
                this.goalService.UpdateAsync(correctGoal, id));
            var exeption2 = await Assert.ThrowsAsync<ArgumentException>(() =>
               this.goalService.UpdateAsync(incorrectGoal, id));
            var exeption3 = await Assert.ThrowsAsync<ArgumentException>(() =>
              this.goalService.UpdateAsync(incorrectGoal, "1"));

            Assert.Equal(exeptionErrorMessage, exeption1.Message);

            Assert.Equal(exeptionErrorMessage, exeption2.Message);

            Assert.Equal(exeptionErrorMessage, exeption3.Message);
        }

        [Fact]
        public void MapGoalViewModelToGoal_WithCorrectData_ShouldReturnCorrectResult()
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

            var model = new Goal
            {
                Id = "1",
                Title = "Test",
                ColorId = color.Id,
                Color = color,
                CalendarId = calendar.Id,
                Calendar = calendar,
                DayTime = DayTime.Afternoon,
                Duration = Duration.HourAndHalf,
                Frequency = Frequency.EveryDay,
                IsActive = true,
            };

            var viewModel = new GoalViewModel
            {
                Title = "Test",
                ColorId = 1,
                CalendarId = "1",
                DayTime = DayTime.Afternoon.ToString(),
                Duration = Duration.HourAndHalf.ToString(),
                Frequency = Frequency.EveryDay.ToString(),
            };

            this.goalRepository.Setup(x => x.All()).Returns(new List<Goal> { model }.AsQueryable());
            var actualResult = this.goalService.MapGoalViewModelToGoal(viewModel, model.Id);
            var expectedResult = model;

            this.goalRepository.Verify(x => x.All(), Times.Once);

            Assert.Equal(expectedResult.Title, actualResult.Title);
            Assert.Equal(expectedResult.CalendarId, actualResult.CalendarId);
            Assert.Equal(expectedResult.ColorId, actualResult.ColorId);
            Assert.Equal(expectedResult.StartDateTime, actualResult.StartDateTime);
            Assert.Equal(expectedResult.DayTime, actualResult.DayTime);
            Assert.Equal(expectedResult.Duration, actualResult.Duration);
            Assert.Equal(expectedResult.Frequency, actualResult.Frequency);
            Assert.Equal(expectedResult.Color, actualResult.Color);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void MapGoalViewModelToGoal_WithWithNullOrEmptyArgument_ShouldThrowAnArgumentException(string id)
        {
            var exeptionErrorMessage = "One or more required properties are null.";

            var correctGoal = new GoalViewModel
            {
                Title = "Test",
                ColorId = 1,
                CalendarId = "1",
                DayTime = DayTime.Afternoon.ToString(),
                Duration = Duration.HourAndHalf.ToString(),
                Frequency = Frequency.EveryDay.ToString(),
            };

            GoalViewModel incorrectGoal = null;

            var exeption1 = Assert.Throws<ArgumentException>(() =>
               this.goalService.MapGoalViewModelToGoal(correctGoal, id));
            var exeption2 = Assert.Throws<ArgumentException>(() =>
              this.goalService.MapGoalViewModelToGoal(incorrectGoal, id));
            var exeption3 = Assert.Throws<ArgumentException>(() =>
             this.goalService.MapGoalViewModelToGoal(incorrectGoal, "1"));

            Assert.Equal(exeptionErrorMessage, exeption1.Message);

            Assert.Equal(exeptionErrorMessage, exeption2.Message);

            Assert.Equal(exeptionErrorMessage, exeption3.Message);
        }

        [Fact]
        public void GetGoalChangeViewModelById_WithCorrectData_ShouldReturnCorrectly()
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

            var calendarViewModel = new CalendarHabitViewModel
            {
                Id = calendar.Id,
                Title = calendar.Title,
            };

            var model = new Goal
            {
                Id = "1",
                Title = "Test",
                ColorId = color.Id,
                Color = color,
                CalendarId = calendar.Id,
                Calendar = calendar,
                DayTime = DayTime.Afternoon,
                Duration = Duration.HourAndHalf,
                Frequency = Frequency.EveryDay,
                IsActive = true,
            };

            var viewModel = new GoalViewModel
            {
                Title = "Test",
                ColorId = 1,
                CalendarId = "1",
                DayTime = DayTime.Afternoon.ToString(),
                Duration = Duration.HourAndHalf.ToString(),
                Frequency = Frequency.EveryDay.ToString(),
            };

            this.calendarService
                .Setup(x => x.GetAllCalendarTitlesByUserName<CalendarHabitViewModel>(It.IsAny<string>()))
                .Returns(new List<CalendarHabitViewModel> { calendarViewModel });
            this.colorService.Setup(x => x.GetAllColors()).Returns(new List<Color> { color });
            this.enumParseService
                .Setup(x => x.GetEnumDescription(It.IsAny<string>(), typeof(DayTime)))
                .Returns(DayTime.Afternoon.ToString());
            this.enumParseService
                .Setup(x => x.GetEnumDescription(It.IsAny<string>(), typeof(Duration)))
                .Returns(Duration.HourAndHalf.ToString());
            this.enumParseService
                .Setup(x => x.GetEnumDescription(It.IsAny<string>(), typeof(Frequency)))
                .Returns(Frequency.EveryDay.ToString());
            this.goalRepository.Setup(x => x.All()).Returns(new List<Goal> { model }.AsQueryable());
            var actualResult = this.goalService.GetGoalChangeViewModelById(model.Id, user.UserName);
            var expectedResult = viewModel;
            var actualResultOutput = actualResult.GoalModel;

            Assert.Equal(expectedResult.Title, actualResultOutput.Title);
            Assert.Equal(expectedResult.CalendarId, actualResultOutput.CalendarId);
            Assert.Equal(expectedResult.ColorId, actualResultOutput.ColorId);
            Assert.Equal(expectedResult.DayTime, actualResultOutput.DayTime);
            Assert.Equal(expectedResult.Duration, actualResultOutput.Duration);
            Assert.Equal(expectedResult.Frequency, actualResultOutput.Frequency);
        }

        [Theory]
        [InlineData("", "1")]
        [InlineData(null, "1")]
        [InlineData("Test", "")]
        [InlineData("Test", null)]
        [InlineData("", "")]
        [InlineData(null, null)]
        public void GetGoalChangeViewModelById_WithNullOrEmptyArgument_ShouldThrowAnArgumentException(string goalId, string username)
        {
            var exeptionErrorMessage = "One or more required properties are null.";

            var exeption = Assert.Throws<ArgumentException>(() =>
                 this.goalService.GetGoalChangeViewModelById(goalId, username));

            Assert.Equal(exeptionErrorMessage, exeption.Message);
        }

        [Fact]
        public void GetGoalChangeViewModelById_WithNoData_ShouldThrowAnArgumentException()
        {
            var exeptionErrorMessage = "One or more required properties are null.";

            this.goalRepository.Setup(x => x.All()).Returns(new List<Goal> { }.AsQueryable());
            var exeption = Assert.Throws<ArgumentException>(() =>
                 this.goalService.GetGoalChangeViewModelById("1", "username"));

            Assert.Equal(exeptionErrorMessage, exeption.Message);
        }

        [Fact]
        public async Task DeleteAsync_WithCorrectData_ShouldReturnCorrectResult()
        {
            var goal = new Goal
            {
                Id = "1",
                Title = "Test",
                ColorId = 1,
                CalendarId = "1",
                DayTime = DayTime.Afternoon,
                Duration = Duration.HourAndHalf,
                Frequency = Frequency.EveryDay,
                IsActive = true,
            };

            this.goalRepository.Setup(x => x.All()).Returns(new List<Goal> { goal }.AsQueryable());
            var result = await this.goalService.DeleteAsync(goal.Id);

            this.goalRepository.Verify(x => x.All(), Times.Once);
            this.goalRepository.Verify(x => x.Update(It.IsAny<Goal>()), Times.Once);
            this.goalRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public async Task DeleteAsync_WithNullOrEmptyArgument_ShouldThrowAnArgumentException(string id)
        {
            var exeptionErrorMessage = "One or more required properties are null.";

            var exeption = await Assert.ThrowsAsync<ArgumentException>(() =>
                this.goalService.DeleteAsync(id));

            Assert.Equal(exeptionErrorMessage, exeption.Message);
        }

        [Fact]
        public async Task DeleteAsync_WithNotExistingModel_ShouldThrowAnArgumentException()
        {
            var model = new Goal
            {
                Id = "1",
                Title = "Test",
                ColorId = 1,
                CalendarId = "1",
                DayTime = DayTime.Afternoon,
                Duration = Duration.HourAndHalf,
                Frequency = Frequency.EveryDay,
                IsActive = true,
            };

            var exeptionErrorMessage = $"Goal with Id: 2 does not exist.";
            this.goalRepository.Setup(x => x.All()).Returns(new List<Goal> { model }.AsQueryable());
            var exeption = await Assert.ThrowsAsync<ArgumentException>(() =>
                this.goalService.DeleteAsync("2"));

            Assert.Equal(exeptionErrorMessage, exeption.Message);
        }

        [Fact]
        public async Task CreateMoreHabitsAsync_WithCorrectData_ShouldReturnCorrectResult()
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

            var calendarViewModel = new CalendarHabitViewModel
            {
                Id = calendar.Id,
                Title = calendar.Title,
            };

            var habits = new List<Habit>
            {
               new Habit
               {
                    Id = "TestId",
                    Title = "Test",
                    StartDateTime = new DateTime(2020, 02, 02, 12, 0, 0),
                    EndDateTime = new DateTime(2020, 02, 02, 12, 30, 0),
                    GoalId = "1",
               },
            };

            var model = new Goal
            {
                Id = "1",
                Title = "Test",
                ColorId = color.Id,
                Color = color,
                CalendarId = calendar.Id,
                Calendar = calendar,
                DayTime = DayTime.Afternoon,
                Duration = Duration.HourAndHalf,
                Frequency = Frequency.EveryDay,
                IsActive = true,
                Habits = habits,
            };

            var viewModel = new GoalViewModel
            {
                Title = "Test",
                ColorId = 1,
                CalendarId = "1",
                DayTime = DayTime.Afternoon.ToString(),
                Duration = Duration.HourAndHalf.ToString(),
                Frequency = Frequency.EveryDay.ToString(),
            };

            this.goalRepository.Setup(x => x.All()).Returns(new List<Goal> { model }.AsQueryable());
            await this.goalService.CreateMoreHabitsAsync(calendar.Id);

            this.goalRepository.Verify(x => x.All(), Times.Exactly(2));
            this.habitService.Verify(x => x.GenerateMoreHabitsAsync(It.IsAny<Goal>(), It.IsAny<DateTime>()));
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public async Task CreateMoreHabitsAsync_WithNullOrEmptyArgument_ShouldThrowAnArgumentException(string id)
        {
            var exeptionErrorMessage = "One or more required properties are null.";

            var exeption = await Assert.ThrowsAsync<ArgumentException>(() =>
                this.goalService.CreateMoreHabitsAsync(id));

            Assert.Equal(exeptionErrorMessage, exeption.Message);
        }
    }
}
