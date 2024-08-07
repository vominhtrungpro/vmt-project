using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using vmt_project.dal.Models.Entities;

namespace vmt_project.dal.Base
{
    public interface IGenericRepository<T> where T : Entity
    {
        IQueryable<T> GetAll();

        IQueryable<T> FindBy(Expression<Func<T, bool>> predicate);

        Task<IQueryable<T>> FindByAsync(Expression<Func<T, bool>> predicate);

        void Add(T entity);

        void Delete(T entity);

        void Edit(T entity);

        T Get(Guid id);

        Task<T> GetAsync(Guid id);

        T Get(Expression<Func<T, bool>> predicate);

        Task<T> GetAsync(Expression<Func<T, bool>> predicate);

        bool IsExists(Expression<Func<T, bool>> predicate);

        bool IsExists(Guid id);
        Task<bool> IsExistsAsync(Expression<Func<T, bool>> predicate);

        Task<bool> IsExistsAsync(Guid id);

        void AddRange(List<T> entities, bool isCommit = true);

        void DeleteRange(List<T> entities);

        void EditRange(List<T> entities, bool isCommit = true);

        void Commit();
        void ClearTracker();

        Task<IQueryable<T>> FindByRawQueryAsync(string query, object[] parameters);

        Task<int> CountRecordsAsync(Expression<Func<T, bool>> predicate);

        IQueryable<T> IncludeTables(IQueryable<T> entity, string tables);

        Task<DataTable> SqlCommand(string query);

        Task<int> ExcuteSearchStoreProcudure(string spName, SqlParameter[] parameters, DataTable dt);
        void BulkInsert(IList<T> items, int packageSize = 1000);
        Task BulkInsert(IList<T> entities, CancellationToken cancellationToken);
        Task BulkUpdate(IList<T> entities, CancellationToken cancellationToken);
        Task BulkSaveChangesAsync(CancellationToken cancellationToken);
        string GetTableName();
    }
}
