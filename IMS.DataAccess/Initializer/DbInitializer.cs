using IMS.DataAccess.Data;
using IMS.Models.Models.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.DataAccess.Initializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DbInitializer(ApplicationDbContext db, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public void Initialize()
        {
            try
            {
                if(_db.Database.GetPendingMigrations().Count() > 0)
                {
                    _db.Database.Migrate();
                }
            }
            catch (Exception ex)
            {

            }

            if (!_roleManager.RoleExistsAsync("Admin").GetAwaiter().GetResult())
            {
                //create user and role
                _roleManager.CreateAsync(new IdentityRole("Admin")).GetAwaiter().GetResult();
                _userManager.CreateAsync(new ApplicationUser
                {
                    Full_Name = "Test Admin",
                    UserName = "test@admin.com",
                    Email = "test@admin.com",
                    PhoneNumber = "0123456789",
                    EmailConfirmed = true
                }, "Abc123*").GetAwaiter().GetResult();

                // get the user by his mail
                var user = _db.ApplicationUser.FirstOrDefault(x => x.Email == "test@admin.com");

                //assign role here
                _userManager.AddToRoleAsync(user, "Admin").GetAwaiter().GetResult();
                //create a menu
                _db.MenuList.AddAsync(new Models.Models.MenuList
                {
                    Area="Admin",
                    Controller_Name = "Menu",
                    Action_Name = "Index"
                }).GetAwaiter().GetResult();
                //give rolePriviledge
                var roleID = _db.UserRoles.Where(x => x.UserId == user.Id).Select(x=>x.RoleId).FirstOrDefault();
                _db.RolePrivileges.AddAsync(new Models.Models.RolePrivilege
                {
                    MenuId = 1,
                    RoleId = roleID.ToString()
                }).GetAwaiter().GetResult();
                _db.SaveChangesAsync().GetAwaiter().GetResult();
            }
            else
            {
                return;
            }
        }
    }
}
