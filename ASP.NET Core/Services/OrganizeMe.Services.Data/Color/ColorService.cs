namespace OrganizeMe.Services.Data.Color
{
    using System.Collections.Generic;
    using System.Linq;

    using OrganizeMe.Data.Common.Repositories;
    using OrganizeMe.Data.Models;

    public class ColorService : IColorService
    {
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
    }
}
