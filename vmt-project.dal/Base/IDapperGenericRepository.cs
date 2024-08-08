using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vmt_project.dal.Base
{
    public interface IDapperGenericRepository<T> where T : class
    {
        T Get(object key, string tableName, string keyColumn);

        void Insert(T obj, string tableName);
        void Update(T obj, string tableName, string keyColumn);
        void Delete(object key, string tableName, string keyColumn);
    }
}
