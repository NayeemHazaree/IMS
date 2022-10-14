using IMS.DataAccess.Data;
using IMS.Models.Models;
using IMS.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IMS.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/Role")]
    [Authorize(Policy = "AccessChecker")]
    public class RoleController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly RoleManager<IdentityRole> _roleManager;
        public RoleController(ApplicationDbContext db, RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _roleManager = roleManager;
        }

        [Route("Index")]
        public IActionResult Index()
        {
            return View();
        }

        #region API CALLS
        [HttpGet]
        [Route("getRoleList")]
        public async Task<IActionResult> getRoleList()
        {
            IEnumerable<IdentityRole> role_List = await _roleManager.Roles.ToListAsync();
            return Json(new { data = role_List });
        }
        #endregion

        [HttpGet]
        [Route("Upsert")]
        public async Task<IActionResult> Upsert(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                //create view
                return View();
            }
            else
            {
                //update view
                var roleListinDb = await _db.Roles.FirstOrDefaultAsync(x => x.Id == id);
                return View(roleListinDb);
            }
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        [Route("Upsert")]
        public async Task<IActionResult> Upsert(IdentityRole roleObj)
        {
            var roleFromDb = await _roleManager.Roles.FirstOrDefaultAsync(x => x.Id == roleObj.Id);
            if (roleFromDb == null)
            {
                //create role
                await _roleManager.CreateAsync(new IdentityRole() { Id = roleObj.Id, Name = roleObj.Name });
            }
            else
            {
                //update role
                roleFromDb.Name = roleObj.Name;
                roleFromDb.NormalizedName = roleObj.NormalizedName;
                await _roleManager.UpdateAsync(roleFromDb);
            }
            return RedirectToAction("Index");
        }

        //[HttpGet]
        //public async Task<IActionResult> Delete(string id)
        //{
        //    if (String.IsNullOrEmpty(id))
        //    {
        //        //Delete Id not found
        //        return NotFound();
        //    }
        //    else
        //    {
        //        //Delete view

        //        var roleListinDb = await _db.Roles.FirstOrDefaultAsync(x => x.Id == id);
        //        if(roleListinDb != null)
        //        {
        //            if (roleListinDb.Name == "Admin")
        //            {
        //                //will add a notification later = "this role can't be deleted"
        //                return View("Index");
        //            }
        //            else
        //            {
        //                return View(roleListinDb);
        //            }
        //        }
        //        return View(roleListinDb);
        //    }
        //}

        [HttpPost]
        [Route("Delete")]
        public async Task<IActionResult> Delete(string id)
        {
            var AllRoles = await _db.UserRoles.ToListAsync();
            var del_role = await _roleManager.Roles.FirstOrDefaultAsync(x => x.Id == id);
            if (del_role != null)
            {
                if (AllRoles != null)
                {
                    var isRoleAssigned = AllRoles.FirstOrDefault(x => x.RoleId == id);
                    if (isRoleAssigned != null)
                    {
                        //Can't be deleted
                        return Json(new { success = false });
                    }
                    else
                    {
                        //Delete the role
                        await _roleManager.DeleteAsync(del_role);
                    }
                }
            }
            return RedirectToAction("Index");
        }

        [Route("RoleExists")]
        public async Task<IActionResult> RoleExists(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                var roleExists = await _roleManager.Roles.Where(x => x.Name.ToLower() == name.ToLower()).ToListAsync();
                if (roleExists.Count() > 0)
                {
                    return Json(new { success = false });
                }
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }
    }
}
