namespace OrganizeMe.Services.Data.Color
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;
    using OrganizeMe.Data.Common.Repositories;
    using OrganizeMe.Data.Models;

    public class ColorService : IColorService
    {
        private readonly IRepository<Color> colorReposository;

        public ColorService(IRepository<Color> colorReposository)
        {
            this.colorReposository = colorReposository;
        }

        public async Task<ICollection<Color>> GetAllColorsAsync()
        {
            var colors = await this.colorReposository.All().ToListAsync();
            return colors;
        }
    }
}
