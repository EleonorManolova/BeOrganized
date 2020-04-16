namespace BeOrganized.Services
{
    using System.Threading.Tasks;

    using Nest;

    public interface ISearchService
    {
        Task<Result> CreateIndexAsync<T>(T model)
            where T : class;

        Task<Result> DeleteIndexAsync<T>(T model)
            where T : class;

        Task<Result> UpdateIndexAsync<T>(T model)
            where T : class;
    }
}
