using IMS.Models.Models;
using IMS.Models.Models.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.DataAccess.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public DbSet<Category> Category { get; set; }
        public DbSet<Brand> Brand { get; set; }
        public DbSet<Clib> Clib { get; set; }
        public DbSet<Product> Product { get; set; }
        public DbSet<ApplicationUser> ApplicationUser {get;set;}
        public DbSet<MenuList> MenuList {get;set;}
        public DbSet<RolePrivilege> RolePrivileges { get; set; }
        public DbSet<UserPrivilege> UserPrivileges { get; set; }
        public DbSet<Role> Role { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<OrderList> OrderLists { get; set; }
        public DbSet<Sales> Sales { get; set; }
        public DbSet<CouponCodes> CouponCodes { get; set; }
        public DbSet<Branch> Branch { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<OrderList>().
                Property(x => x.OrderDate).HasColumnType("date");

            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Sales>().
                Property(x => x.Sale_Date).HasColumnType("date");


            modelBuilder.Entity<UserPrivilege>().HasKey(x => new { x.MenuId, x.UserId });
            modelBuilder.Entity<UserPrivilege>().
                HasOne<MenuList>(x => x.MenuList).
                WithMany(x => x.UserPrivileges).
                HasForeignKey(x => x.MenuId);

            modelBuilder.Entity<UserPrivilege>().
                HasOne<ApplicationUser>(x => x.ApplicationUser).
                WithMany(x => x.UserPrivileges).
                HasForeignKey(x => x.UserId);

            modelBuilder.Entity<RolePrivilege>().HasKey(x => new { x.MenuId, x.RoleId });
            modelBuilder.Entity<RolePrivilege>().
                HasOne<MenuList>(x => x.MenuList).
                WithMany(x => x.RolePrivileges).
                HasForeignKey(x => x.MenuId);

            modelBuilder.Entity<RolePrivilege>().
                HasOne<Role>(x => x.Role).
                WithMany(x => x.RolePrivileges).
                HasForeignKey(x => x.RoleId);
        }
    }
}
