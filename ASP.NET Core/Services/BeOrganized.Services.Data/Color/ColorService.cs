namespace BeOrganized.Services.Data.Color
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using BeOrganized.Data.Common.Repositories;
    using BeOrganized.Data.Models;

    public class ColorService : IColorService
    {
        private const string InvalidPropertyErrorMessage = "One or more required properties are null.";

        private readonly IRepository<Color> colorReposository;

        public ColorService(IRepository<Color> colorReposository)
        {
            this.colorReposository = colorReposository;
        }

        public ICollection<Color> GetAllColors()
        {
            var colors = this.colorReposository
                .All()
                .ToList();
            return colors;
        }

        public string GetColorHex(int colorId)
        {
            var colorQuery = this.colorReposository.All().Where(x => x.Id == colorId);

            if (!colorQuery.Any())
            {
                throw new ArgumentException(InvalidPropertyErrorMessage);
            }

            return colorQuery.First().Hex;
        }
    }
}
