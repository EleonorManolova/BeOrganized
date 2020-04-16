namespace BeOrganized.Services
{
    using System;
    using System.Threading.Tasks;

    using Nest;

    public class SearchService : ISearchService
    {
        private const string ElasticSearchPrefix = "Elasticsearch: ";
        private const string InvalidPropertyErrorMessage = "One or more required properties are null.";

        private IElasticClient client;

        public SearchService(IElasticClient elasticClient)
        {
            this.client = elasticClient;
        }

        public async Task<Result> CreateIndexAsync<T>(T model)
            where T : class
        {
            if (model == null)
            {
                throw new ArgumentException(InvalidPropertyErrorMessage);
            }

            var result = await this.client.IndexDocumentAsync(model);

            if (!result.IsValid)
            {
                throw new ArgumentException(ElasticSearchPrefix + result.OriginalException);
            }

            return result.Result;
        }

        public async Task<Result> UpdateIndexAsync<T>(T model)
           where T : class
        {
            if (model == null)
            {
                throw new ArgumentException(InvalidPropertyErrorMessage);
            }

            var result = await this.client.UpdateAsync<T>(model, u => u.Doc(model));

            if (!result.IsValid)
            {
                throw new ArgumentException(ElasticSearchPrefix + result.OriginalException);
            }

            return result.Result;
        }

        public async Task<Result> DeleteIndexAsync<T>(T model)
          where T : class
        {
            if (model == null)
            {
                throw new ArgumentException(InvalidPropertyErrorMessage);
            }

            var result = await this.client.DeleteAsync<T>(model);

            if (!result.IsValid)
            {
                throw new ArgumentException(ElasticSearchPrefix + result.OriginalException);
            }

            return result.Result;
        }
    }
}
