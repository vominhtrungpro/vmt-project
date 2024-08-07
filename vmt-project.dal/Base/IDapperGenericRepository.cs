using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vmt_project.dal.Base
{
    public interface IDapperGenericRepository<T> where T : class
    {
        void Insert(T obj, string tableName);
    }
}
