using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vmt_project.services.Elastic.Base
{
    public interface IGenericElasticService<T>
    {
        Task<string> CreateDocumentAsync(T document);
        Task<T> GetDocumentAsync(string id);
        Task<List<T>> GetAllDocument();
        Task<string> UpdateDocument(T document);
        Task<string> DeleteDocumentAsync(T document);
    }
}
