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
        public async Task<bool> CreateDocumentAsync(T document)
        {
            var response = await _elasticClient.IndexDocumentAsync(document);
            return response.IsValid ? true : false;
        }
        public async Task<bool> UpdateDocumentAsync(T document)
        {
            var response = await _elasticClient.UpdateAsync(new DocumentPath<T>(document), u => u.Doc(document).RetryOnConflict(3));
            return response.IsValid ? true : false;
        }
        public async Task<bool> DeleteDocumentAsync(string id)
        {
            var response = await _elasticClient.DeleteAsync(new DocumentPath<T>(id));
            return response.IsValid ? true : false;
        }

        public async Task<List<T>> GetAllDocumentAsync()
        {
            var response = await _elasticClient.SearchAsync<T>(s => s.MatchAll());
            return response.Documents.ToList();
        }

        public async Task<T> GetDocumentAsync(string id)
        {
            var response = await _elasticClient.GetAsync<T>(id);
            return response.Source;
        }
        public async Task<bool> BulkInsertDocumentAsync(List<T> documents)
        {
            var response = await _elasticClient.IndexManyAsync<T>(documents);
            return response.IsValid ? true : false;
        }
    }
}
