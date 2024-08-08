using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace vmt_project.dal.Base
{
    public class DapperGenericRepository<T> : IDapperGenericRepository<T> where T : class
    {
        readonly string connString = "Data Source=PC-TRUNGVO\\SQLEXPRESS;Initial Catalog=vmt-project;Integrated Security=True;TrustServerCertificate=True;";
        public void Insert(T obj, string tableName)
        {
            using (var conn = new SqlConnection(connString))
            {
                var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
                var columnNames = string.Join(", ", properties.Select(p => p.Name));
                var parameterNames = string.Join(", ", properties.Select(p => "@" + p.Name));

                string insertQuery = $"INSERT INTO {tableName} ({columnNames}) VALUES ({parameterNames})";
                conn.Execute(insertQuery, obj);
            }
        }
        public T Get(object key, string tableName, string keyColumn) 
        {
            using (var conn = new SqlConnection(connString))
            {
                string selectQuery = $"SELECT * FROM {tableName} WHERE {keyColumn} = @Key";
                return conn.QuerySingleOrDefault<T>(selectQuery, new { Key = key });
            }
        }
        public void Update(T obj, string tableName, string keyColumn)
        {
            using (var conn = new SqlConnection(connString))
            {
                var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
                var setClause = string.Join(", ", properties.Select(p => $"{p.Name} = @{p.Name}"));

                string updateQuery = $"UPDATE {tableName} SET {setClause} WHERE {keyColumn} = @{keyColumn}";
                conn.Execute(updateQuery, obj);
            }
        }
        public void Delete(object key, string tableName, string keyColumn)
        {
            using (var conn = new SqlConnection(connString))
            {
                string deleteQuery = $"DELETE FROM {tableName} WHERE {keyColumn} = @Key";
                conn.Execute(deleteQuery, new { Key = key });
            }
        }


    }
}
