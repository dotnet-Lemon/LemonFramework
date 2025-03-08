using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LemonFramework.Common.Helper;
using LemonFramework.Domain.Model.User;
using Microsoft.EntityFrameworkCore;

namespace LemonFramework.Core
{
    public class LemonFrameworkDBContext : DbContext
    {
        public LemonFrameworkDBContext()
        {
            
        }

        public LemonFrameworkDBContext(DbContextOptions<LemonFrameworkDBContext> options) :base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region 用户模块
            modelBuilder.Entity<Users>(e => {
                e.Property(p => p.UserId).HasComment("用户Guid");
                e.Property(p => p.UserNaame).HasComment("用户名");
                e.Property(p => p.RealName).HasComment("真实名称");
                e.Property(p => p.Phone).HasComment("联系方式");
                e.Property(p => p.CreateDate).HasComment("创建时间");
                e.Property(p => p.ModifyDate).HasComment("修改时间");
                e.Property(p => p.IsEnabled).HasComment("是否启用");
                e.Property(p => p.IsDelete).HasComment("是否删除");
            });

            #endregion
            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // optionsBuilder.UseSqlServer(Appsettings.app(new string[] {"ConnectionStrings", "Default"}));
                optionsBuilder.UseSqlServer("Server=localhost;Database=LemonFrameworkDB;uid=Lemon;pwd=lemon2501.;TrustServerCertificate=true");
            }
            base.OnConfiguring(optionsBuilder);
        }
    }
}