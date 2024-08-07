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

    }
}
