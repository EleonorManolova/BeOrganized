namespace OrganizeMe.Services.Data.Habits
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using OrganizeMe.Data.Common.Repositories;
    using OrganizeMe.Data.Models;
    using OrganizeMe.Data.Models.Enums;
    using OrganizeMe.Services.Mapping;
    using OrganizeMe.Web.ViewModels.Habits;

    public class HabitService : IHabitService
    {
        private readonly IEnumParseService enumParseService;
        private readonly IDeletableEntityRepository<Habit> habitsRepository;

        public HabitService(IEnumParseService enumParseService, IDeletableEntityRepository<Habit> habitsRepository)
        {
            this.enumParseService = enumParseService;
            this.habitsRepository = habitsRepository;
        }

        // public async Task<bool> AddAsync(HabitInputViewModel habitViewModel)
        // {
        //    if (this.habitsRepository.AllWithDeleted()
        //        .Any(x => x.Id == habitViewModel. && x.RemoteId == remoteNews.RemoteId))
        //    {
        //        // Already exists
        //        return false;
        //    }

        // var habit = habitViewModel.To<Habit>().First();
        //    news.SearchText = this.GetSearchText(news);

        // await this.habitsRepository.AddAsync(habit);
        //    await this.habitsRepository.SaveChangesAsync();
        //    return true;
        // }

        public HabitCreateViewModel GetHabitViewModel()
        {
            var dayTimes = Enum.GetNames(typeof(DayTime));
            var frequencies = Enum.GetNames(typeof(Frequency));
            var durations = Enum.GetNames(typeof(Duration));

            var dayTimesDescriptions = new List<string>();
            var frequenciesDescriptions = new List<string>();
            var durationsDescriptions = new List<string>();
            foreach (var dayTime in dayTimes)
            {
                dayTimesDescriptions.Add(this.enumParseService
                    .GetEnumDescription(dayTime, typeof(DayTime)));
            }

            foreach (var frequency in frequencies)
            {
                frequenciesDescriptions.Add(this.enumParseService
                    .GetEnumDescription(frequency, typeof(Frequency)));
            }

            foreach (var duration in durations)
            {
                durationsDescriptions.Add(this.enumParseService
                    .GetEnumDescription(duration, typeof(Duration)));
            }

            var habitViewModel = new HabitCreateViewModel
            {
                Input = new HabitInputViewModel(),
                DayTimes = dayTimesDescriptions,
                Frequencies = frequenciesDescriptions,
                Durations = durationsDescriptions,
            };

            return habitViewModel;
        }

        public T GetEnum<T>(string description)
        {
            return this.enumParseService.Parse<T>(description);
        }
    }
}
