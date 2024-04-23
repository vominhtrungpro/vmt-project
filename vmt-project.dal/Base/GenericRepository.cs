using EFCore.BulkExtensions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using vmt_project.dal.Models.Context;
using vmt_project.dal.Models.Entities;

namespace vmt_project.dal.Base
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : Entity
    {
        #region Properties
        public VmtDbContext _context;
        private bool disposed = false;

        public VmtDbContext DbContext
        {
            get
            {
                return _context;
            }
        }
        #endregion

        #region Constructor
        public GenericRepository(VmtDbContext unitOfWork)
        {
            _context = unitOfWork;
        }
        #endregion

        #region Method
        public virtual void Add(TEntity item)
        {
            if (item != null)
            {
                GetSet().Add(item);
                _context.Commit();
            }
        }

        public virtual TEntity Get(Guid id)
        {
            if (id != Guid.Empty)
            {
                return GetSet().Find(id);
            }
            return null;
        }

        public virtual async Task<TEntity> GetAsync(Guid id)
        {
            if (id != Guid.Empty)
            {
                return await GetSet().FindAsync(id);
            }
            return null;
        }

        public virtual TEntity Get(Expression<Func<TEntity, bool>> predicate)
        {
            return GetSet().FirstOrDefault(predicate);
        }

        public virtual async Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await GetSet().FirstOrDefaultAsync(predicate);
        }

        public virtual IQueryable<TEntity> GetAll()
        {
            return GetSet().AsQueryable();
        }

        public IQueryable<TEntity> FindBy(Expression<Func<TEntity, bool>> predicate)
        {
            return GetSet().Where(predicate).AsQueryable();
        }

        public async Task<IQueryable<TEntity>> FindByAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await Task.Run(() => GetSet().Where(predicate).AsQueryable());
        }

        public void Delete(TEntity entity)
        {
            if (entity != null)
            {
                _context.Attach(entity);
                GetSet().Remove(entity);
                _context.Commit();
            }
        }

        public void Edit(TEntity entity)
        {
            if (entity != null)
            {
                _context.SetModified(entity);
                _context.Commit();
            }
        }
        public void AddRange(List<TEntity> entities, bool isCommit = true)
        {
            try
            {
                GetSet().AddRange(entities);
                if (isCommit)
                {
                    _context.Commit();
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public void DeleteRange(List<TEntity> entities)
        {
            try
            {
                GetSet().RemoveRange(entities);
                _context.Commit();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public void EditRange(List<TEntity> entities, bool isCommit = true)
        {
            try
            {
                if (entities.Any())
                {
                    foreach (var entity in entities)
                    {
                        _context.SetModified(entity);
                    }
                }
                if (isCommit)
                {
                    _context.Commit();
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public void Commit()
        {
            try
            {
                _context.Commit();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public DbSet<TEntity> GetSet()
        {
            return _context.CreateSet<TEntity>();
        }

        protected virtual Guid ParseGuid(string guidStr)
        {
            try
            {
                return Guid.Parse(guidStr);
            }
            catch { return Guid.Empty; }
        }
        public bool IsExists(Expression<Func<TEntity, bool>> predicate)
        {
            return GetSet().Any(predicate);
        }

        public bool IsExists(Guid id)
        {
            return IsExists(p => p.Id == id);
        }
        public async Task<bool> IsExistsAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await GetSet().AnyAsync(predicate);
        }

        public async Task<bool> IsExistsAsync(Guid id)
        {
            return await IsExistsAsync(p => p.Id == id);
        }

        public async Task<IQueryable<TEntity>> FindByRawQueryAsync(string query, object[] parameters)
        {
            return await Task.Run(() => GetSet().FromSqlRaw(query, parameters).AsQueryable());
        }

        public async Task<int> CountRecordsAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await GetSet().CountAsync(predicate);
        }

        public IQueryable<TEntity> IncludeTables(IQueryable<TEntity> entity, string tables)
        {
            return entity.Include(tables);
        }


        public async Task<DataTable> SqlCommand(string query)
        {
            string connStr = _context.Database.GetConnectionString();
            var conn = new SqlConnection(connStr);
            SqlCommand cmd = new SqlCommand(query, conn);
            conn.Open();
            var reader = await cmd.ExecuteReaderAsync();
            var dt = new DataTable();
            dt.Load(reader);
            conn.Close();
            return dt;
        }

        public async Task<int> ExcuteSearchStoreProcudure(string spName, SqlParameter[] parameters, DataTable dt)
        {
            string connStr = _context.Database.GetConnectionString();
            var conn = new SqlConnection(connStr);
            SqlCommand cmd = new SqlCommand();
            conn.Open();
            cmd.Connection = conn;
            cmd.CommandText = spName;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddRange(parameters);
            var reader = await cmd.ExecuteReaderAsync();
            dt.Load(reader);
            int totalRows = int.Parse(cmd.Parameters["@ROW_COUNT"].Value.ToString() ?? "0");
            conn.Close();
            return totalRows;
        }
        public void BulkInsert(IList<TEntity> items, int packageSize = 1000)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                _context.BulkInsert(items, new BulkConfig { BatchSize = packageSize });
                transaction.Commit();
            }
        }
        public async Task BulkInsert(IList<TEntity> entities, CancellationToken cancellationToken)
        {
            await _context.BulkInsertAsync(entities, cancellationToken: cancellationToken);
        }
        public async Task BulkUpdate(IList<TEntity> entities, CancellationToken cancellationToken)
        {
            await _context.BulkUpdateAsync(entities, cancellationToken: cancellationToken);
        }
        public async Task BulkSaveChangesAsync(CancellationToken cancellationToken)
        {
            await _context.BulkSaveChangesAsync(cancellationToken: cancellationToken);
        }

        public void ClearTracker()
        {
            _context.ChangeTracker.Clear();
        }
        #endregion
    }
}
