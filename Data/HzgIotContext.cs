using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Hzg.Iot.Models;

namespace Hzg.Iot.Data;

public class HzgIotContext : DbContext
{    
    public HzgIotContext(DbContextOptions<HzgIotContext> options) : base (options)
    {}

    // 公司
    public virtual DbSet<Company> Companys { get; set; }
    // 设备
    public virtual DbSet<Device> Devices { get; set; }
    // 菜单
    public virtual DbSet<Menu> Menus { get; set; }
    // 菜单权限
    public virtual DbSet<MenuPermission> MenuPermissions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // 公司表
        modelBuilder.Entity<Company>().ToTable("company");
        // 设备表
        modelBuilder.Entity<Device>().ToTable("device");
        // 菜单表
        modelBuilder.Entity<Menu>().ToTable("menu");
        // 菜单权限表
        modelBuilder.Entity<MenuPermission>().ToTable("menuPermission");
    }
}