namespace OrganizeMe.Web.Tests
{
    using System;
    using System.Linq;

    using OrganizeMe.Web.ViewModels.Events;
    using Xunit;

    public class EventViewModelTests
    {
        [Fact]
        public void StartDateShoudBeBeforeEndDate()
        {
            var viewModel = new EventViewModel
            {
                StartDate = new DateTime(2020, 1, 1),
                StartTime = new DateTime(2020, 1, 1, 1, 0, 0),
                EndDate = new DateTime(2020, 1, 1),
                EndTime = new DateTime(2020, 1, 1, 1, 30, 0),
                Title = "Test",
                CalendarId = "",
            };

            var errors = viewModel.Validate(null).Count();

            Assert.Equal(0, errors);
        }

        [Fact]
        public void ErrorShoudBeThrownWhenStartDateIsNotBeforeEndDate()
        {
            var viewModel = new EventViewModel
            {
                StartDate = new DateTime(2020, 1, 1),
                StartTime = new DateTime(2020, 1, 1, 1, 30, 0),
                EndDate = new DateTime(2020, 1, 1),
                EndTime = new DateTime(2020, 1, 1, 1, 0, 0),
                Title = "Test",
                CalendarId = "",
            };

            var errors = viewModel.Validate(null).Count();

            Assert.Equal(1, errors);
        }
    }
}
