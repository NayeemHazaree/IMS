using IMS.DataAccess.Data;
using IMS.Models.Models;
using IMS.Models.Models.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace IMS.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/RolePrivilege")]
    //[Authorize(Policy = "AccessChecker")]
    public class RolePrivilegeController : Controller
    {
        private readonly ApplicationDbContext _db;
        public RolePrivilegeController(ApplicationDbContext db)
        {
            _db = db;
        }

        [Route("Index")]
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirst(ClaimTypes.Name);
            var userlist = await _db.Users.Where(x => x.UserName == userId.Value).ToListAsync();
            RolePrivilege rolePrivilege = new RolePrivilege()
            {
                RoleListFromDb = _db.Roles.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                })
            };
            return View(rolePrivilege);
        }

        [HttpGet]
        [Route("GetPageList")]
        public async Task<IActionResult> GetPageList(string id)
        {
            var is_permit = await _db.RolePrivileges.Where(x => x.RoleId == id).ToListAsync();
            var page_list = await _db.MenuList.ToListAsync();

            if (is_permit.Count != 0)
            {
                foreach (var item in page_list)
                {
                    foreach (var menu in is_permit)
                    {
                        if (item.Menu_Id == menu.MenuId)
                        {
                            item.Status = true;
                        }
                    }
                }

                MenuList menuList = new MenuList()
                {
                    PageList = page_list,
                    Exist_Role = is_permit
                };
                return PartialView("_PageList", menuList);
            }
            else
            {
                MenuList menuList = new MenuList()
                {
                    PageList = page_list,
                };
                return PartialView("_PageList", menuList);
            }


        }
        [HttpPost]
        [Route("AddRolePrivilege")]
        public async Task<IActionResult> AddRolePrivilege(string menuList_Id, string roleId)
        {
            var rolePrivilegeList = await _db.RolePrivileges.Where(x => x.RoleId == roleId).ToListAsync();
            List<string> MenusList = new List<string>();
            if (menuList_Id != null)
            {
                MenusList = menuList_Id.Split(',').ToList();
            }

            if (rolePrivilegeList != null)
            {
                foreach (var rolePri in rolePrivilegeList)
                {
                    _db.RolePrivileges.Remove(rolePri);
                    await _db.SaveChangesAsync();
                }

            }
            if (MenusList.Count > 0)
            {
                RolePrivilege rolePrivilege = new RolePrivilege();
                foreach (var menu in MenusList)
                {
                    var menuObj = await _db.MenuList.FirstOrDefaultAsync(x => x.Menu_Id == Convert.ToInt32(menu));
                    rolePrivilege.MenuId = menuObj.Menu_Id;
                    rolePrivilege.RoleId = roleId;
                    await _db.RolePrivileges.AddAsync(rolePrivilege);
                    await _db.SaveChangesAsync();
                }
            }

            return Json(new { success = true, message = "Successfully Assigned" });
        }
    }
}
