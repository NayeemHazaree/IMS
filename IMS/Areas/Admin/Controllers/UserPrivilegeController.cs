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
    [Route("Admin/UserPrivilege")]
    [Authorize(Policy = "AccessChecker")]
    public class UserPrivilegeController : Controller
    {
        private readonly ApplicationDbContext _db;
        public UserPrivilegeController(ApplicationDbContext db)
        {
            _db = db;
        }

        [Route("Index")]
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirst(ClaimTypes.Name);
            if(userId != null)
            {
                var userlist = await _db.Users.Where(x => x.UserName == userId.Value).ToListAsync();
                UserPrivilege userPrivilege = new UserPrivilege()
                {
                    UserListDB = _db.ApplicationUser.Select(i => new SelectListItem
                    {
                        Text = i.Full_Name,
                        Value = i.Id,
                    })
                };
                return View(userPrivilege);
            }
            
            return View();
        }

        [HttpGet]
        [Route("UserDetails")]
        public async Task<IActionResult> UserDetails(string id)
        {
            var is_permit = await _db.UserPrivileges.Where(x => x.UserId == id).ToListAsync();
            var page_list = await _db.MenuList.ToListAsync();
            var user_details = await _db.ApplicationUser.FirstOrDefaultAsync(x => x.Id == id);
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
                    ApplicationUser = user_details,
                    PageList = page_list,
                    Exist_User = is_permit
                };
                return PartialView("_UserDetails", menuList);
            }
            else
            {
                MenuList menuList = new MenuList()
                {
                    ApplicationUser = user_details,
                    PageList = page_list,
                };
                return PartialView("_UserDetails", menuList);
            }
        }

        [HttpPost]
        [Route("AddUserPrivilege")]
        public async Task<IActionResult> AddUserPrivilege(string menuList_Id, string userId)
        {
            var userPrivilegeList = await _db.UserPrivileges.Where(x => x.UserId == userId).ToListAsync();
            List<string> MenuList = new List<string>();
            if(menuList_Id != null)
            {
                MenuList = menuList_Id.Split(',').ToList();
            }
            if (userPrivilegeList != null)
            {
                foreach (var userPri in userPrivilegeList)
                {
                    _db.UserPrivileges.Remove(userPri);
                    await _db.SaveChangesAsync();
                }
            }
            if (MenuList.Count > 0)
            {
                UserPrivilege userPrivilege = new UserPrivilege();
                foreach (var menu in MenuList)
                {
                    var menuObj = await _db.MenuList.FirstOrDefaultAsync(x => x.Menu_Id == Convert.ToInt32(menu));
                    if(menuObj != null)
                    {
                        userPrivilege.MenuId = menuObj.Menu_Id;
                        userPrivilege.UserId = userId;
                        await _db.UserPrivileges.AddAsync(userPrivilege);
                        await _db.SaveChangesAsync();
                    }
                }
            }
            return Json(new { success = true, message = "Successfully Assigned" });
        }
    }
}
