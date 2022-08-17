using Microsoft.EntityFrameworkCore;
using Hzg.Iot.Models;

namespace Hzg.Iot.Data;

public class AccountContext : DbContext
{
    public AccountContext(DbContextOptions<AccountContext> options) : base(options)
    {}

    // 用户
    public virtual DbSet<User> Users { get; set; }
    // 用户分组
    public virtual DbSet<Group> Groups { get; set; }
    // 用户角色
    public virtual DbSet<Role> Roles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().ToTable("User");
            modelBuilder.Entity<Group>().ToTable("Group");
            modelBuilder.Entity<Role>().ToTable("Role");
        }
}