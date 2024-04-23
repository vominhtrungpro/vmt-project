using Azure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using vmt_project.common.Models;
using vmt_project.dal.Models.Entities;

namespace vmt_project.dal.Models.Context
{
    public class VmtDbContext : IdentityDbContext<User, Role, string, IdentityUserClaim<string>, UserRoleMapping, IdentityUserLogin<string>, IdentityRoleClaim<string>, IdentityUserToken<string>>
    {
        #region Constructors
        public VmtDbContext()
        {
            
        }
        public VmtDbContext(string connectionString) : base(GetOptions(connectionString))
        {
            
        }
        private static DbContextOptions GetOptions(string connectionString)
        {
            var optionsBuilder = new DbContextOptionsBuilder<VmtDbContext>();
            optionsBuilder.UseSqlServer(connectionString);
            return optionsBuilder.Options;
        }
        public VmtDbContext(DbContextOptions<VmtDbContext> options) : base(options)
        {
        }
        #endregion

        #region DbSet
        public DbSet<Character> Characters { get; set; }
        #endregion

        #region Configuration & Navigation
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var appSetting = JsonConvert.DeserializeObject<AppSetting>(File.ReadAllText("appsettings.json"));
                optionsBuilder.UseSqlServer(appSetting.ConnectionString);
            }
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
        #endregion

        #region Methods
        public void Commit()
        {
            using (var tran = base.Database.BeginTransaction())
            {
                try
                {
                    var result = base.SaveChanges();
                    tran.Commit();
                }
                catch (DbUpdateException ex)
                {
                    tran.Rollback();
                    throw ex;
                }
            }
        }

        public DbSet<TEntity> CreateSet<TEntity>() where TEntity : class
        {
            return base.Set<TEntity>();
        }

        public void SetModified<TEntity>(TEntity item) where TEntity : class
        {
            base.Entry<TEntity>(item).State = EntityState.Modified;
        }
        #endregion
    }
}
