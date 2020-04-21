namespace BeOrganized.Services.Data.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using BeOrganized.Data.Models;
    using BeOrganized.Services.Data.Color;
    using Moq;
    using Xunit;

    public class ColorServiceTests
    {
        private Mock<BeOrganized.Data.Common.Repositories.IRepository<Color>> colorRepository;
        private ColorService colorService;

        public ColorServiceTests()
        {
            this.colorRepository = new Mock<BeOrganized.Data.Common.Repositories.IRepository<Color>>();
            this.colorService = new ColorService(this.colorRepository.Object);
        }

        [Fact]
        public void GetAllCalendarTitlesByUserName_WithCorrectData_ShouldReturnCorrectResult()
        {
            var color = new Color
            {
                Id = 1,
                Name = "Test",
                Hex = "TestHex",
            };

            this.colorRepository.Setup(x => x.All()).Returns(new List<Color> { color }.AsQueryable);
            var resultCollection = this.colorService.GetAllColors();
            var result = resultCollection.First();

            this.colorRepository.Verify(m => m.All(), Times.Once);
            Assert.Equal(1, resultCollection.Count);
            Assert.Equal(color.Id, result.Id);
            Assert.Equal(color.Name, result.Name);
            Assert.Equal(color.Hex, result.Hex);
        }

        [Fact]
        public void GetColorHex_WithCorrectData_ShouldReturnCorrectResult()
        {
            var color = new Color
            {
                Id = 1,
                Name = "Test",
                Hex = "TestHex",
            };

            this.colorRepository.Setup(x => x.All()).Returns(new List<Color> { color }.AsQueryable);
            var result = this.colorService.GetColorHex(color.Id);

            this.colorRepository.Verify(m => m.All(), Times.Once);
            Assert.Equal(color.Hex, result);
        }

        [Fact]
        public void GetColorHex_WithNoData_ShouldReturnCorrectResult()
        {
            var color = new Color
            {
                Id = 1,
                Name = "Test",
                Hex = "TestHex",
            };

            this.colorRepository.Setup(x => x.All()).Returns(new List<Color> { }.AsQueryable);
            var exeptionErrorMessage = "One or more required properties are null.";

            var exeption = Assert.Throws<ArgumentException>(() =>
              this.colorService.GetColorHex(2));

            Assert.Equal(exeptionErrorMessage, exeption.Message);
        }
    }
}
