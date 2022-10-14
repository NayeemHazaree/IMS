using IMS.DataAccess.Data;
using IMS.Models.Models.Users;
using IMS.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Security.Claims;
using System.Text.Json.Nodes;

namespace IMS.Areas.Admin.Controllers
{

    [Area("Admin")]
    [Authorize(Policy = "AccessChecker")]
    [Route("Admin/User")]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserController(ApplicationDbContext db,
                               UserManager<IdentityUser> userManager,
                               RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [Route("Index")]
        public IActionResult Index()
        {
            
            return View();

        }
        [Route("SearchRole")]
        public async Task<IActionResult> SearchRole(string id)
        {
            var userList = await _db.ApplicationUser.ToListAsync();
            var roleList = await _db.Roles.ToListAsync();
            var userRole = await _db.UserRoles.ToListAsync();
            foreach (var user in userList)
            {
                var role = userRole.Where(x => x.UserId == user.Id).ToList();
                List<string> roleName = new List<string>();
                List<string> roleIdList = new List<string>();

                foreach (var roles in role)
                {
                    roleName.Add(roleList.FirstOrDefault(x => x.Id == roles.RoleId).Name);
                    roleIdList.Add(roleList.FirstOrDefault(x => x.Id == roles.RoleId).Id);
                }
                user.Role = roleName;
                user.RoleId = roleIdList;
            }
            userList = userList.Where(x => x.RoleId.Contains(id)).ToList();

            //if (id == "all" || id == "undefined")
            //{
            //    var S_Role = userFromDb;
            //    return Json(new { data = S_Role });
            //}
            //else
            //{
            //    //var Role_name = RolesFromDb.FirstOrDefault(x => x.Id == id);
            //    var S_UserRole = UserRoleFromDb.Where(x => x.RoleId == id).ToList();
            //    List<ApplicationUser> userList = new List<ApplicationUser>();
            //    foreach (var user in S_UserRole)
            //    {
            //        ApplicationUser S_User = (ApplicationUser)userFromDb.Where(x=>x.Id == user.UserId).ToList();
            //        userList.Add(S_User);
            //    }
            return Json(new { data = userList });
        }

        [Route("UserTable")]
        public async Task<IActionResult> UserTable(string id)
        {
            var userList = await _db.ApplicationUser.ToListAsync();
            var roleList = await _db.Roles.ToListAsync();
            var userRole = await _db.UserRoles.ToListAsync();
            foreach (var user in userList)
            {
                var role = userRole.Where(x => x.UserId == user.Id).ToList();
                List<string> roleName = new List<string>();
                List<string> roleIdList = new List<string>();

                foreach (var roles in role)
                {
                    roleName.Add(roleList.FirstOrDefault(x => x.Id == roles.RoleId).Name);
                    roleIdList.Add(roleList.FirstOrDefault(x => x.Id == roles.RoleId).Id);
                }
                user.Role = roleName;
                user.RoleId = roleIdList;
            }
            if (!string.IsNullOrEmpty(id) && id != "all")
            {
                userList = userList.Where(x => x.RoleId.Contains(id)).ToList();
            }
            if (id == "all")
            {
                userList = userList;
            }

            UserVM userVM = new UserVM()
            {
                ApplicationUsers = userList,
                CurrentUser = await _userManager.GetUserAsync(HttpContext.User),
                UserRole = await _db.UserRoles.ToListAsync(),
                IdentityRoles = _db.Roles.ToList(),
                RoleForUser = _db.Roles.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                })
            };
            return PartialView("_UserTable", userVM);
            //if (id == "all" || id == "undefined")
            //{
            //    return PartialView("_UserTable", userVM);
            //    //return Json(new { data = userList });
            //}
            //else
            //{
            //    userList = userList.Where(x => x.RoleId.Contains(id)).ToList();
            //    return PartialView("_UserTable", userVM);
            //}

        }

        [HttpGet]
        [Route("Edit")]
        public async Task<IActionResult> Edit(string id)
        {
            var userDetails = await _db.ApplicationUser.FirstOrDefaultAsync(x => x.Id == id);
            var roleList = await _db.Roles.ToListAsync();
            var userRole = await _db.UserRoles.ToListAsync();
            List<string> roleName = new List<string>();
            List<string> roleIdList = new List<string>();
            var role = userRole.Where(x => x.UserId == userDetails.Id).ToList();
            foreach (var roles in role)
            {
                roleName.Add(roleList.FirstOrDefault(x => x.Id == roles.RoleId).Name);
                roleIdList.Add(roleList.FirstOrDefault(x => x.Id == roles.RoleId).Id);
                userDetails.Role = roleName;
                userDetails.RoleId = roleIdList;
            }
            //var userDetails = userList.FirstOrDefault(x => x.Id == id);
            //if (userDetails != null)
            //{
            //    return Json(new { data = userDetails });
            //}
            //return View();
            return Json(new { data = userDetails });
        }

        [HttpPost]
        [Route("Edit")]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Edit(UserVM userVM)
        {
            if (userVM.applicationUser != null)
            {
                var user_details = await _db.ApplicationUser.FirstOrDefaultAsync(x => x.Id == userVM.applicationUser.Id);
                if (user_details != null)
                {
                    var ExistingRole = await _db.UserRoles.Where(x => x.UserId == userVM.applicationUser.Id).ToListAsync();
                    var ChangingRole = userVM.applicationUser.RoleList.ToList();

                    //Delete the existing all role first
                    foreach (var roleId in ExistingRole)
                    {
                        var RemoveRole = await _roleManager.Roles.FirstOrDefaultAsync(x => x.Id == roleId.RoleId);
                        await _userManager.RemoveFromRoleAsync(user_details, RemoveRole.ToString());
                    }

                    //Add new role
                    foreach (var Role in ChangingRole)
                    {
                        var addNewRole = await _roleManager.Roles.FirstOrDefaultAsync(x => x.Id == Role);
                        await _userManager.AddToRoleAsync(user_details, addNewRole.ToString());
                    }
                }
            }
            return Json(new { success = true });
        }

        [HttpPost]
        [Route("Delete")]
        public async Task<IActionResult> Delete(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                var userDetails = await _db.ApplicationUser.FirstOrDefaultAsync(x => x.Id == id);
                if (userDetails != null)
                {
                    _db.ApplicationUser.Remove(userDetails);
                    _db.SaveChanges();

                }
            }
            return Json(new { success = true });
        }

        [Route("BlockUnblock")]
        public async Task<IActionResult> BlockUnblock(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                var userDetails = await _db.ApplicationUser.FirstOrDefaultAsync(x => x.Id == id);
                var currentDate = DateTime.Now;
                if (userDetails != null)
                {
                    if (userDetails.LockoutEnd == null)
                    {
                        userDetails.LockoutEnd = currentDate.AddYears(100);
                        //_db.ApplicationUser.Update(userDetails);
                        _db.SaveChanges();
                        return Json(new { success = true });

                    }
                    else
                    {
                        userDetails.LockoutEnd = null;
                        //_db.ApplicationUser.Update(userDetails);
                        _db.SaveChanges();
                        return Json(new { success = false });

                    }

                }
                else
                {
                    return NotFound();
                }

            }
            return BadRequest();
        }
    }
}
