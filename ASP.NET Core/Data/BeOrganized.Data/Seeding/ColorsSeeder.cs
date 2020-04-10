namespace BeOrganized.Data.Seeding
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using BeOrganized.Data.Models;

    public class ColorsSeeder : ISeeder
    {
        public async Task SeedAsync(ApplicationDbContext dbContext, IServiceProvider serviceProvider)
        {
            if (dbContext.Colors.Any())
            {
                return;
            }

            var colors = new List<Color>
            {
                new Color
                {
                    Name = "Terracota",
                    Hex = "#E27D60",
                },
                new Color
                {
                    Name = "Padua",
                    Hex = "#A7E5C6",
                },
                new Color
                {
                    Name = "Tacao",
                    Hex = "#E8A87C",
                },
                new Color
                {
                    Name = "Oriental Pink",
                    Hex = "#C38D9E",
                },
                new Color
                {
                    Name = "Keppel",
                    Hex = "#41B3A3",
                },
            };

            foreach (var color in colors)
            {
                await dbContext.Colors.AddAsync(color);
            }
        }
    }
}
