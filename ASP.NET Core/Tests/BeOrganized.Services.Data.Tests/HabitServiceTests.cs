namespace BeOrganized.Services.Data.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;

    using BeOrganized.Data.Common.Repositories;
    using BeOrganized.Data.Models;
    using BeOrganized.Services;
    using BeOrganized.Services.Data.Habit;
    using BeOrganized.Services.Mapping;
    using BeOrganized.Web.ViewModels.Calendar;
    using BeOrganized.Web.ViewModels.Habits;
    using Moq;
    using Xunit;

    public class HabitServiceTests
    {
        private Mock<IDeletableEntityRepository<Habit>> habitsRepository;
        private Mock<IDateTimeService> dateTimeService;
        private HabitService habitService;

        public HabitServiceTests()
        {
            this.InitializeMapper();
            this.habitsRepository = new Mock<IDeletableEntityRepository<Habit>>();

            var enumParseService = new EnumParseService();
            this.dateTimeService = new Mock<IDateTimeService>();
            this.habitService = new HabitService(this.habitsRepository.Object, this.dateTimeService.Object, enumParseService);
        }

        [Fact]
        public void GetAllByCalendarId_WithCorrectData_ShouldReturnCorrectResult()
        {
            var color = new Color
            {
                Id = 1,
                Name = "Test",
                Hex = "TestHex",
            };

            var calendar = new Calendar
            {
                Id = "CalendarId",
                Title = "Default",
                DefaultCalendarColor = color,
                DefaultCalendarColorId = color.Id,
            };

            var goal = new Goal
            {
                Id = "TestId",
                Title = "Test",
                Calendar = calendar,
                CalendarId = calendar.Id,
                IsActive = true,
                Color = color,
                ColorId = color.Id,
            };

            var habit = new Habit
            {
                Id = "TestId",
                Title = "Test",
                StartDateTime = new DateTime(2020, 02, 02, 12, 0, 0),
                EndDateTime = new DateTime(2020, 02, 02, 12, 30, 0),
                GoalId = goal.Id,
                Goal = goal,
            };

            var viewModel = new HabitCalendarViewModel
            {
                Id = "TestId",
                Title = "Test",
                StartDateTime = new DateTime(2020, 02, 02, 12, 0, 0),
                EndDateTime = new DateTime(2020, 02, 02, 12, 30, 0),
                GoalCalendarId = calendar.Id,
                GoalColorHex = color.Hex,
                IsCompleted = false,
            };

            this.habitsRepository.Setup(x => x.All()).Returns(new List<Habit> { habit }.AsQueryable());
            var result = this.habitService.GetAllByCalendarId(calendar.Id);

            this.habitsRepository.Verify(x => x.All(), Times.Once);

            Assert.Equal(1, result.Count);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void GetAllByCalendarId__WithNullOrEmptyArgument_ShouldThrowAnArgumentException(string calendarId)
        {
            var exeptionErrorMessage = $"Calendar with Id: {calendarId} does not exist.";

            var exeption = Assert.Throws<ArgumentException>(() =>
               this.habitService.GetAllByCalendarId(calendarId));

            Assert.Equal(exeptionErrorMessage, exeption.Message);
        }

        [Fact]
        public async Task GetByIdAsync_WithCorrectData_ShouldReturnCorrectResult()
        {
            var color = new Color
            {
                Id = 1,
                Name = "Test",
                Hex = "TestHex",
            };

            var calendar = new Calendar
            {
                Id = "CalendarId",
                Title = "Default",
                DefaultCalendarColor = color,
                DefaultCalendarColorId = color.Id,
            };

            var goal = new Goal
            {
                Id = "TestId",
                Title = "Test",
                Calendar = calendar,
                CalendarId = calendar.Id,
                IsActive = true,
                Color = color,
                ColorId = color.Id,
            };

            var habit = new Habit
            {
                Id = "TestId",
                Title = "Test",
                StartDateTime = new DateTime(2020, 02, 02, 12, 0, 0),
                EndDateTime = new DateTime(2020, 02, 02, 12, 30, 0),
                GoalId = goal.Id,
                Goal = goal,
            };

            this.habitsRepository.Setup(x => x.GetByIdWithDeletedAsync(It.IsAny<string>())).ReturnsAsync(habit);
            var result = await this.habitService.GetByIdAsync(habit.Id);
            var expectedResult = habit;

            this.habitsRepository.Verify(x => x.GetByIdWithDeletedAsync(It.IsAny<string>()), Times.Once);
            Assert.Equal(expectedResult.Title, result.Title);
            Assert.Equal(expectedResult.GoalId, result.GoalId);
            Assert.Equal(expectedResult.Id, result.Id);
            Assert.Equal(expectedResult.StartDateTime, result.StartDateTime);
            Assert.Equal(expectedResult.EndDateTime, result.EndDateTime);
            Assert.Equal(expectedResult.IsCompleted, result.IsCompleted);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public async Task GetByIdAsync__WithNullOrEmptyArgument_ShouldThrowAnArgumentException(string id)
        {
            var exeptionErrorMessage = "One or more required properties are null.";

            var exeption = await Assert.ThrowsAsync<ArgumentException>(() =>
               this.habitService.GetByIdAsync(id));

            Assert.Equal(exeptionErrorMessage, exeption.Message);
        }

        [Fact]
        public async Task GetByIdAsync_WithIncorrectModel_ShouldReturnCorrectResult()
        {
            var nullHabit = new Habit()
            {
                Id = "TestNew",
                Title = "Test",
            };
            nullHabit = null;
            var exeptionErrorMessage = $"Habit with Id: Test1 does not exist.";

            this.habitsRepository.Setup(x => x.GetByIdWithDeletedAsync(It.IsAny<string>())).ReturnsAsync(nullHabit);

            var exeption = await Assert.ThrowsAsync<ArgumentException>(() =>
              this.habitService.GetByIdAsync("Test1"));

            Assert.Equal(exeptionErrorMessage, exeption.Message);
        }

        [Fact]
        public async Task GenerateHabitsAsync_WithCorrectData_ShouldReturnCorrectResult()
        {
            var color = new Color
            {
                Id = 1,
                Name = "Test",
                Hex = "TestHex",
            };

            var calendar = new Calendar
            {
                Id = "CalendarId",
                Title = "Default",
                DefaultCalendarColor = color,
                DefaultCalendarColorId = color.Id,
            };

            var goal = new Goal
            {
                Id = "TestId",
                Title = "Test",
                Calendar = calendar,
                CalendarId = calendar.Id,
                IsActive = true,
                Color = color,
                ColorId = color.Id,
            };

            var currentDate = DateTime.Parse("02/02/2020");
            var startEndDatetime = new StartEndDateTime
            {
                Start = new DateTime(2020, 02, 02, 12, 0, 0),
                End = new DateTime(2020, 02, 02, 12, 30, 0),
            };

            this.dateTimeService
                .Setup(x => x.GenerateDatesForMonthAhead(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DateTime>()))
                .Returns(new List<StartEndDateTime> { startEndDatetime });
            var result = await this.habitService.GenerateHabitsAsync(goal, currentDate);

            this.habitsRepository.Verify(x => x.AddAsync(It.IsAny<Habit>()), Times.Once);
            this.habitsRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task GenerateHabitsAsync__WithNullOrEmptyArgument_ShouldThrowAnArgumentException()
        {
            var exeptionErrorMessage = "Goal does not exist.";
            var goalNull = new Goal();
            goalNull = null;
            var currentDate = DateTime.Parse("02/02/2020");

            var exeption = await Assert.ThrowsAsync<ArgumentException>(() =>
               this.habitService.GenerateHabitsAsync(goalNull, currentDate));

            Assert.Equal(exeptionErrorMessage, exeption.Message);
        }

        [Fact]
        public async Task GenerateMoreHabitsAsync_WithCorrectData_ShouldReturnCorrectResult()
        {
            var color = new Color
            {
                Id = 1,
                Name = "Test",
                Hex = "TestHex",
            };

            var calendar = new Calendar
            {
                Id = "CalendarId",
                Title = "Default",
                DefaultCalendarColor = color,
                DefaultCalendarColorId = color.Id,
            };

            var goal = new Goal
            {
                Id = "TestId",
                Title = "Test",
                Calendar = calendar,
                CalendarId = calendar.Id,
                IsActive = true,
                Color = color,
                ColorId = color.Id,
            };

            var currentDate = DateTime.Parse("02/02/2020");
            var lastGeneratedWeek = DateTime.Parse("02/02/2020");
            var firstDate = DateTime.Parse("03/02/2020");
            var startEndDatetime = new StartEndDateTime
            {
                Start = new DateTime(2020, 02, 02, 12, 0, 0),
                End = new DateTime(2020, 02, 02, 12, 30, 0),
            };

            this.dateTimeService
               .Setup(x => x.FirstDayOfWeekAfhterMonth(It.IsAny<DateTime>()))
               .Returns(lastGeneratedWeek);
            this.dateTimeService
               .Setup(x => x.FirstDayOfWeek(It.IsAny<DateTime>()))
               .Returns(firstDate);
            this.dateTimeService
                .Setup(x => x.GenerateDatesForMonthAhead(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DateTime>()))
                .Returns(new List<StartEndDateTime> { startEndDatetime });

            var result = await this.habitService.GenerateMoreHabitsAsync(goal, currentDate);

            this.habitsRepository.Verify(x => x.AddAsync(It.IsAny<Habit>()), Times.Once);
            this.habitsRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task GenerateMoreHabitsAsync__WithNullOrEmptyArgument_ShouldThrowAnArgumentException()
        {
            var exeptionErrorMessage = "Goal does not exist.";
            var goalNull = new Goal();
            goalNull = null;
            var currentDate = DateTime.Parse("02/02/2020");

            var exeption = await Assert.ThrowsAsync<ArgumentException>(() =>
               this.habitService.GenerateMoreHabitsAsync(goalNull, currentDate));

            Assert.Equal(exeptionErrorMessage, exeption.Message);
        }

        [Fact]
        public async Task GenerateMoreHabitsAsync_WithSameStartAndLastWeek_ShouldReturnCorrectResult()
        {
            var color = new Color
            {
                Id = 1,
                Name = "Test",
                Hex = "TestHex",
            };

            var calendar = new Calendar
            {
                Id = "CalendarId",
                Title = "Default",
                DefaultCalendarColor = color,
                DefaultCalendarColorId = color.Id,
            };

            var goal = new Goal
            {
                Id = "TestId",
                Title = "Test",
                Calendar = calendar,
                CalendarId = calendar.Id,
                IsActive = true,
                Color = color,
                ColorId = color.Id,
            };

            var currentDate = DateTime.Parse("03/02/2020");
            var lastGeneratedWeek = DateTime.Parse("02/02/2020");
            var firstDate = DateTime.Parse("02/02/2020");
            var startEndDatetime = new StartEndDateTime
            {
                Start = new DateTime(2020, 02, 02, 12, 0, 0),
                End = new DateTime(2020, 02, 02, 12, 30, 0),
            };

            this.dateTimeService
               .Setup(x => x.FirstDayOfWeekAfhterMonth(It.IsAny<DateTime>()))
               .Returns(lastGeneratedWeek);
            this.dateTimeService
               .Setup(x => x.FirstDayOfWeek(It.IsAny<DateTime>()))
               .Returns(firstDate);
            this.dateTimeService
                .Setup(x => x.GenerateDatesForMonthAhead(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DateTime>()))
                .Returns(new List<StartEndDateTime> { startEndDatetime });

            var result = await this.habitService.GenerateMoreHabitsAsync(goal, currentDate);

            Assert.False(result);
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
                Id = "CalendarId",
                Title = "Default",
                DefaultCalendarColor = color,
                DefaultCalendarColorId = color.Id,
            };

            var goal = new Goal
            {
                Id = "TestId",
                Title = "Test",
                Calendar = calendar,
                CalendarId = calendar.Id,
                IsActive = true,
                Color = color,
                ColorId = color.Id,
            };

            var habit = new Habit
            {
                Id = "TestId",
                Title = "Test",
                StartDateTime = new DateTime(2020, 02, 02, 12, 0, 0),
                EndDateTime = new DateTime(2020, 02, 02, 12, 30, 0),
                GoalId = goal.Id,
                Goal = goal,
                IsCompleted = false,
            };

            var completedHabitsForWeeks = new Dictionary<string, int> { { "test", 1 }, };

            var detailsModel = new HabitDetailsViewModel
            {
                Id = "TestId",
                Title = "Test",
                StartDateTime = new DateTime(2020, 02, 02, 12, 0, 0),
                EndDateTime = new DateTime(2020, 02, 02, 12, 30, 0),
                GoalId = goal.Id,
                GoalCalendarTitle = calendar.Title,
                GoalColorHex = color.Hex,
                IsCompleted = false,
                GoalFrequencyInt = 1,
                GoalDayTime = "dayTime",
                GoalDuration = "duration",
                GoalFrequency = "frequency",
                CompletedHabitsForWeeks = completedHabitsForWeeks,
            };

            this.habitsRepository.Setup(x => x.All()).Returns(new List<Habit> { habit }.AsQueryable());
            var result = this.habitService.GetDetailsViewModelById(habit.Id);
            var expectedResult = detailsModel;

            this.habitsRepository.Verify(x => x.All(), Times.Exactly(3));
            Assert.Equal(expectedResult.Id, result.Id);
            Assert.Equal(expectedResult.Title, result.Title);
            Assert.Equal(expectedResult.StartDateTime, result.StartDateTime);
            Assert.Equal(expectedResult.EndDateTime, result.EndDateTime);
            Assert.Equal(expectedResult.GoalId, result.GoalId);
            Assert.Equal(expectedResult.GoalColorHex, result.GoalColorHex);
            Assert.Equal(expectedResult.GoalCalendarTitle, result.GoalCalendarTitle);
            Assert.Equal(expectedResult.IsCompleted, result.IsCompleted);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void GetDetailsViewModelById__WithNullOrEmptyArgument_ShouldThrowAnArgumentException(string id)
        {
            var exeptionErrorMessage = $"Habit with Id: {id} does not exist.";

            var exeption = Assert.Throws<ArgumentException>(() =>
              this.habitService.GetDetailsViewModelById(id));

            Assert.Equal(exeptionErrorMessage, exeption.Message);
        }

        [Fact]
        public void GetDetailsViewModelById_WithNoData_ShouldReturnCorrectResult()
        {
            this.habitsRepository.Setup(x => x.All()).Returns(new List<Habit> { }.AsQueryable());
            var result = this.habitService.GetDetailsViewModelById("TestId");
            var expectedResult = new HabitDetailsViewModel();

            this.habitsRepository.Verify(x => x.All(), Times.Once);
            Assert.Equal(expectedResult.Id, result.Id);
            Assert.Equal(expectedResult.Title, result.Title);
            Assert.Equal(expectedResult.StartDateTime, result.StartDateTime);
            Assert.Equal(expectedResult.EndDateTime, result.EndDateTime);
            Assert.Equal(expectedResult.GoalId, result.GoalId);
            Assert.Equal(expectedResult.GoalColorHex, result.GoalColorHex);
            Assert.Equal(expectedResult.GoalCalendarTitle, result.GoalCalendarTitle);
            Assert.Equal(expectedResult.IsCompleted, result.IsCompleted);
        }

        [Fact]
        public void GetDetailsViewModelById_WithSameStartAndLastWeek_ShouldReturnCorrectResult()
        {
            var color = new Color
            {
                Id = 1,
                Name = "Test",
                Hex = "TestHex",
            };

            var calendar = new Calendar
            {
                Id = "CalendarId",
                Title = "Default",
                DefaultCalendarColor = color,
                DefaultCalendarColorId = color.Id,
            };

            var goal = new Goal
            {
                Id = "TestId",
                Title = "Test",
                StartDateTime = new DateTime(2020, 02, 04, 12, 0, 0),
                Calendar = calendar,
                CalendarId = calendar.Id,
                IsActive = true,
                Color = color,
                ColorId = color.Id,
            };

            var habit = new Habit
            {
                Id = "Test1",
                Title = "Test",
                StartDateTime = new DateTime(2020, 02, 02, 12, 0, 0),
                EndDateTime = new DateTime(2020, 02, 02, 12, 30, 0),
                GoalId = goal.Id,
                Goal = goal,
                IsCompleted = false,
            };

            var habitCompleted = new Habit
            {
                Id = "Test2",
                Title = "Test",
                StartDateTime = new DateTime(2020, 02, 02, 12, 0, 0),
                EndDateTime = new DateTime(2020, 02, 02, 12, 30, 0),
                GoalId = goal.Id,
                Goal = goal,
                IsCompleted = true,
            };

            var mondayHabit = DateTime.Parse("03/02/2020");
            var mondayGoal = DateTime.Parse("27/01/2020");
            var firstDate = DateTime.Parse("02/02/2020");
            var startEndDatetime = new StartEndDateTime
            {
                Start = new DateTime(2020, 02, 02, 12, 0, 0),
                End = new DateTime(2020, 02, 02, 12, 30, 0),
            };

            this.habitsRepository
               .Setup(x => x.All())
               .Returns(new List<Habit> { habit, habitCompleted }.AsQueryable());
            this.dateTimeService
               .Setup(x => x.FirstDayOfWeek(habit.StartDateTime))
               .Returns(mondayHabit);
            this.dateTimeService
              .Setup(x => x.FirstDayOfWeek(goal.StartDateTime))
              .Returns(mondayGoal);
            this.dateTimeService
                .Setup(x => x.GenerateDatesForMonthAhead(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DateTime>()))
                .Returns(new List<StartEndDateTime> { startEndDatetime });

            var result = this.habitService.GetDetailsViewModelById(habit.Id);
            Assert.Equal(4, result.CompletedHabitsForWeeks.Count);
        }

        [Fact]
        public async Task UpdateHabitsAsync_WithCorrectData_ShouldReturnCorrectResult()
        {
            var color = new Color
            {
                Id = 1,
                Name = "Test",
                Hex = "TestHex",
            };

            var calendar = new Calendar
            {
                Id = "CalendarId",
                Title = "Default",
                DefaultCalendarColor = color,
                DefaultCalendarColorId = color.Id,
            };

            var goal = new Goal
            {
                Id = "TestId",
                Title = "Test",
                Calendar = calendar,
                CalendarId = calendar.Id,
                IsActive = true,
                Color = color,
                ColorId = color.Id,
            };

            var habit = new Habit
            {
                Id = "TestId",
                Title = "Test",
                StartDateTime = new DateTime(2020, 02, 02, 12, 0, 0),
                EndDateTime = new DateTime(2020, 02, 02, 12, 30, 0),
                GoalId = goal.Id,
                Goal = goal,
                IsCompleted = false,
            };

            var currentDate = DateTime.Parse("02/02/2020");
            var startEndDatetime = new StartEndDateTime
            {
                Start = new DateTime(2020, 02, 02, 12, 0, 0),
                End = new DateTime(2020, 02, 02, 12, 30, 0),
            };

            this.dateTimeService
                .Setup(x => x.GenerateDatesForMonthAhead(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DateTime>()))
                .Returns(new List<StartEndDateTime> { startEndDatetime });

            this.habitsRepository.Setup(x => x.All()).Returns(new List<Habit> { habit }.AsQueryable());
            var result = await this.habitService.UpdateHabitsAsync(goal, habit.Id);

            this.habitsRepository.Verify(x => x.All(), Times.Exactly(2));
            this.habitsRepository.Verify(x => x.SaveChangesAsync(), Times.Exactly(2));
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public async Task UpdateHabitsAsync__WithNullOrEmptyArgument_ShouldThrowAnArgumentException(string id)
        {
            var exeptionErrorMessage = $"Habit with Id: {id} does not exist.";

            var goal = new Goal
            {
                Id = "TestId",
                Title = "Test",
                CalendarId = "1",
                IsActive = true,
                ColorId = 1,
            };

            var exeption = await Assert.ThrowsAsync<ArgumentException>(() =>
              this.habitService.UpdateHabitsAsync(goal, id));

            Assert.Equal(exeptionErrorMessage, exeption.Message);
        }

        [Fact]
        public async Task UpdateHabitsAsync__WithNullOrEmptyArgumentGoal_ShouldThrowAnArgumentException()
        {
            var habitId = "TestId";
            var exeptionErrorMessage = $"Goal does not exist.";

            var goalNull = new Goal();
            goalNull = null;

            var exeption = await Assert.ThrowsAsync<ArgumentException>(() =>
              this.habitService.UpdateHabitsAsync(goalNull, habitId));

            Assert.Equal(exeptionErrorMessage, exeption.Message);
        }

        [Fact]
        public async Task DeleteCurrentAsync_WithCorrectData_ShouldReturnCorrectResult()
        {
            var color = new Color
            {
                Id = 1,
                Name = "Test",
                Hex = "TestHex",
            };

            var calendar = new Calendar
            {
                Id = "CalendarId",
                Title = "Default",
                DefaultCalendarColor = color,
                DefaultCalendarColorId = color.Id,
            };

            var goal = new Goal
            {
                Id = "TestId",
                Title = "Test",
                Calendar = calendar,
                CalendarId = calendar.Id,
                IsActive = true,
                Color = color,
                ColorId = color.Id,
            };

            var habit = new Habit
            {
                Id = "TestId",
                Title = "Test",
                StartDateTime = new DateTime(2020, 02, 02, 12, 0, 0),
                EndDateTime = new DateTime(2020, 02, 02, 12, 30, 0),
                GoalId = goal.Id,
                Goal = goal,
                IsCompleted = false,
            };

            this.habitsRepository.Setup(x => x.GetByIdWithDeletedAsync(It.IsAny<string>())).ReturnsAsync(habit);
            var result = await this.habitService.DeleteCurrentAsync(habit.Id);

            this.habitsRepository.Verify(x => x.Delete(It.IsAny<Habit>()), Times.Once);
            this.habitsRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public async Task DeleteCurrentAsync_WithNullOrEmptyArgument_ShouldThrowAnArgumentException(string id)
        {
            var exeptionErrorMessage = "One or more required properties are null.";

            var exeption = await Assert.ThrowsAsync<ArgumentException>(() =>
                this.habitService.DeleteCurrentAsync(id));

            Assert.Equal(exeptionErrorMessage, exeption.Message);
        }

        [Fact]
        public async Task DeleteCurrentAsync_WithNotExistingModel_ShouldThrowAnArgumentException()
        {
            var exeptionErrorMessage = $"Habit with Id: IncorrectId does not exist.";

            var exeption = await Assert.ThrowsAsync<ArgumentException>(() =>
                this.habitService.DeleteCurrentAsync("IncorrectId"));

            Assert.Equal(exeptionErrorMessage, exeption.Message);
        }

        [Fact]
        public async Task DeleteFollowingAsync_WithCorrectData_ShouldReturnCorrectResult()
        {
            var color = new Color
            {
                Id = 1,
                Name = "Test",
                Hex = "TestHex",
            };

            var calendar = new Calendar
            {
                Id = "CalendarId",
                Title = "Default",
                DefaultCalendarColor = color,
                DefaultCalendarColorId = color.Id,
            };

            var goal = new Goal
            {
                Id = "TestId",
                Title = "Test",
                Calendar = calendar,
                CalendarId = calendar.Id,
                IsActive = true,
                Color = color,
                ColorId = color.Id,
            };

            var habit = new Habit
            {
                Id = "TestId",
                Title = "Test",
                StartDateTime = new DateTime(2020, 02, 02, 12, 0, 0),
                EndDateTime = new DateTime(2020, 02, 02, 12, 30, 0),
                GoalId = goal.Id,
                Goal = goal,
                IsCompleted = false,
            };

            this.habitsRepository.Setup(x => x.GetByIdWithDeletedAsync(It.IsAny<string>())).ReturnsAsync(habit);
            this.habitsRepository.Setup(x => x.All()).Returns(new List<Habit> { habit }.AsQueryable());
            var result = await this.habitService.DeleteFollowingAsync(habit.Id);

            this.habitsRepository.Verify(x => x.Delete(It.IsAny<Habit>()), Times.Once);
            this.habitsRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public async Task DeleteFollowingAsync_WithNullOrEmptyArgument_ShouldThrowAnArgumentException(string id)
        {
            var exeptionErrorMessage = "One or more required properties are null.";

            var exeption = await Assert.ThrowsAsync<ArgumentException>(() =>
                this.habitService.DeleteFollowingAsync(id));

            Assert.Equal(exeptionErrorMessage, exeption.Message);
        }

        [Fact]
        public async Task DeleteFollowingAsync_WithNotExistingModel_ShouldThrowAnArgumentException()
        {
            var exeptionErrorMessage = $"Habit with Id: IncorrectId does not exist.";

            var exeption = await Assert.ThrowsAsync<ArgumentException>(() =>
                this.habitService.DeleteFollowingAsync("IncorrectId"));

            Assert.Equal(exeptionErrorMessage, exeption.Message);
        }

        [Fact]
        public async Task DeleteFollowingAsync_WithNullGoalId_ShouldThrowAnArgumentException()
        {
            var habit = new Habit
            {
                Id = "TestId",
                Title = "Test",
                StartDateTime = new DateTime(2020, 02, 02, 12, 0, 0),
                EndDateTime = new DateTime(2020, 02, 02, 12, 30, 0),
                IsCompleted = false,
            };

            var exeptionErrorMessage = "One or more required properties are null.";
            this.habitsRepository.Setup(x => x.GetByIdWithDeletedAsync(It.IsAny<string>())).ReturnsAsync(habit);
            var exeption = await Assert.ThrowsAsync<ArgumentException>(() =>
                this.habitService.DeleteFollowingAsync(habit.Id));

            Assert.Equal(exeptionErrorMessage, exeption.Message);
        }

        [Fact]
        public async Task SetCompleteAsync_WithCorrectData_ShouldReturnCorrectResult()
        {
            var habit = new Habit
            {
                Id = "TestId",
                Title = "Test",
                StartDateTime = new DateTime(2020, 02, 02, 12, 0, 0),
                EndDateTime = new DateTime(2020, 02, 02, 12, 30, 0),
                GoalId = "GoalId",
                IsCompleted = false,
            };

            this.habitsRepository.Setup(x => x.All()).Returns(new List<Habit> { habit }.AsQueryable());
            var result = await this.habitService.SetCompleteAsync(habit.Id);

            this.habitsRepository.Verify(x => x.Update(It.IsAny<Habit>()), Times.Once);
            this.habitsRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public async Task SetCompleteAsync_WithNullOrEmptyArgument_ShouldThrowAnArgumentException(string id)
        {
            var exeptionErrorMessage = "One or more required properties are null.";

            var exeption = await Assert.ThrowsAsync<ArgumentException>(() =>
                this.habitService.SetCompleteAsync(id));

            Assert.Equal(exeptionErrorMessage, exeption.Message);
        }

        [Fact]
        public async Task SetCompleteAsync_WithCompletedHabit_ShouldReturnCorrectResult()
        {
            var habit = new Habit
            {
                Id = "TestId",
                Title = "Test",
                StartDateTime = new DateTime(2020, 02, 02, 12, 0, 0),
                EndDateTime = new DateTime(2020, 02, 02, 12, 30, 0),
                GoalId = "GoalId",
                IsCompleted = true,
            };

            this.habitsRepository.Setup(x => x.All()).Returns(new List<Habit> { habit }.AsQueryable());
            var result = await this.habitService.SetCompleteAsync(habit.Id);

            Assert.False(result);
        }

        [Fact]
        public async Task SetNotCompleteAsync_WithCorrectData_ShouldReturnCorrectResult()
        {
            var habit = new Habit
            {
                Id = "TestId",
                Title = "Test",
                StartDateTime = new DateTime(2020, 02, 02, 12, 0, 0),
                EndDateTime = new DateTime(2020, 02, 02, 12, 30, 0),
                GoalId = "GoalId",
                IsCompleted = true,
            };

            this.habitsRepository.Setup(x => x.All()).Returns(new List<Habit> { habit }.AsQueryable());
            var result = await this.habitService.SetNotCompleteAsync(habit.Id);

            this.habitsRepository.Verify(x => x.Update(It.IsAny<Habit>()), Times.Once);
            this.habitsRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public async Task SetNotCompleteAsync_WithNullOrEmptyArgument_ShouldThrowAnArgumentException(string id)
        {
            var exeptionErrorMessage = "One or more required properties are null.";

            var exeption = await Assert.ThrowsAsync<ArgumentException>(() =>
                this.habitService.SetNotCompleteAsync(id));

            Assert.Equal(exeptionErrorMessage, exeption.Message);
        }

        [Fact]
        public async Task SetNotCompleteAsync_WithNotCompletedHabit_ShouldReturnCorrectResult()
        {
            var habit = new Habit
            {
                Id = "TestId",
                Title = "Test",
                StartDateTime = new DateTime(2020, 02, 02, 12, 0, 0),
                EndDateTime = new DateTime(2020, 02, 02, 12, 30, 0),
                GoalId = "GoalId",
                IsCompleted = false,
            };

            this.habitsRepository.Setup(x => x.All()).Returns(new List<Habit> { habit }.AsQueryable());
            var result = await this.habitService.SetNotCompleteAsync(habit.Id);

            Assert.False(result);
        }

        [Fact]
        public async Task UpdateAsync_WithCorrectData_ShouldReturnCorrectResult()
        {

            var habit = new Habit
            {
                Id = "TestId",
                Title = "Test",
                StartDateTime = new DateTime(2020, 02, 02, 12, 0, 0),
                EndDateTime = new DateTime(2020, 02, 02, 12, 30, 0),
                GoalId = "GoalId",
                IsCompleted = false,
            };

            var result = await this.habitService.UpdateAsync(habit);

            this.habitsRepository.Verify(x => x.Update(It.IsAny<Habit>()), Times.Once);
            this.habitsRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public async Task UpdateAsync_WithNullOrEmptyArgument_ShouldThrowAnArgumentException(string id)
        {
            var exeptionErrorMessage = "One or more required properties are null.";

            var exeption = await Assert.ThrowsAsync<ArgumentException>(() =>
                this.habitService.UpdateAsync(It.IsAny<Habit>()));

            Assert.Equal(exeptionErrorMessage, exeption.Message);
        }

        private void InitializeMapper() => AutoMapperConfig.
           RegisterMappings(Assembly.Load("BeOrganized.Web.ViewModels"));
    }
}
