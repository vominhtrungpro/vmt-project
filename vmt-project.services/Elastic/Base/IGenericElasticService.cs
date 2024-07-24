using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vmt_project.services.Elastic.Base
{
    public interface IGenericElasticService<T>
    {
        Task<bool> CreateDocumentAsync(T document);
        Task<bool> UpdateDocumentAsync(T document);
        Task<T> GetDocumentAsync(string id);
        Task<List<T>> GetAllDocumentAsync();
        Task<bool> DeleteDocumentAsync(string document);
        Task<bool> BulkInsertDocumentAsync(List<T> documents);
    }
}
