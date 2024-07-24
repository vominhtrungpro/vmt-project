using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vmt_project.services.Elastic.Base
{
    public class GenericElasticService<T> : IGenericElasticService<T> where T : class
    {
        private readonly ElasticClient _elasticClient;
        public GenericElasticService(ElasticClient elasticClient)
        {
            _elasticClient = elasticClient;
        }
        public async Task<string> CreateDocumentAsync(T document)
        {
            throw new NotImplementedException();
        }

        public async Task<string> DeleteDocumentAsync(T document)
        {
            throw new NotImplementedException();
        }

        public async Task<List<T>> GetAllDocument()
        {
            throw new NotImplementedException();
        }

        public async Task<T> GetDocumentAsync(string id)
        {
            var response = await _elasticClient.GetAsync<T>(id);
            return response.Source;
        }

        public async Task<string> UpdateDocument(T document)
        {
            throw new NotImplementedException();
        }
    }
}
