namespace OrganizeMe.Data.Seeding
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using OrganizeMe.Data.Models;

    public class EventsSeeder : ISeeder
    {
        public async Task SeedAsync(ApplicationDbContext dbContext, IServiceProvider serviceProvider)
        {
            if (dbContext.Events.Any())
            {
                return;
            }

            var events = new List<Event> {
            new Event
            {
            Title = "Meeting",
            StartDate = DateTime.Now,
            StartTime = DateTime.Now,
            EndDate = DateTime.Now,
            EndTime = DateTime.Now.AddMinutes(30),
            },
            };

            foreach (var eventCreated in events)
            {
                await dbContext.Events.AddAsync(eventCreated);
            }
        }
    }
}
