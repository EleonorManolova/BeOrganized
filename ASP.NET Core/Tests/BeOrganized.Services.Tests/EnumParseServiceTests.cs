namespace BeOrganized.Services.Tests
{
    using System;

    using BeOrganized.Data.Models.Enums;
    using BeOrganized.Services;
    using Xunit;

    public class EnumParseServiceTests
    {
        private EnumParseService enumParseService;

        public EnumParseServiceTests()
        {
            this.enumParseService = new EnumParseService();
        }

        [Fact]
        public void GetEnumDescription_WithCorrectData_ShouldReturnCorrectResult()
        {
            var actualResult = this.enumParseService
                .GetEnumDescription(Frequency.EveryDay.ToString(), typeof(Frequency));
            var expectedResult = "Every Day";

            Assert.True(expectedResult == actualResult);
        }

        [Fact]
        public void GetEnumDescription_WithIncorrectData_ShouldReturnCorrectResult()
        {
            string actualResult = this.enumParseService
                .GetEnumDescription("Test", typeof(Frequency));
            string expectedResult = null;

            Assert.True(expectedResult == actualResult);
        }

        [Fact]
        public void Parse_WithCorrectData_ShouldReturnCorrectResult()
        {
            var actualResult = this.enumParseService.Parse<Frequency>("Every Day");
            var expectedResult = Frequency.EveryDay;

            Assert.True(expectedResult == actualResult);
        }

        [Fact]
        public void Parse_WithIncorrectData_ShouldThrowArgumentException()
        {
            Assert.Throws<ArgumentNullException>(() => this.enumParseService.Parse<Frequency>("Test"));
        }
    }
}
