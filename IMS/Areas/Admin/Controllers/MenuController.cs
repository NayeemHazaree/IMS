using IMS.DataAccess.Data;
using IMS.Models.Models;
using IMS.Models.Models.Authorize;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace IMS.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/Menu")]
    [Authorize(Policy = "AccessChecker")]
    public class MenuController : Controller
    {
        private readonly ApplicationDbContext _db;

        public MenuController(ApplicationDbContext db)
        {
            _db = db;
        }

        [Route("Index")]
        public IActionResult Index()
        {
            IEnumerable<MenuList> menuLists = _db.MenuList.ToList();
            return View(menuLists);
        }

        [HttpGet]
        [Route("Upsert")]
        public async Task<IActionResult> Upsert(int id)
        {
            MenuList menuList = new MenuList();
            if (id == 0)
            {
                //create view
                return View(menuList);
            }
            else
            {
                //edit view
                var Menu = await _db.MenuList.FirstOrDefaultAsync(x => x.Menu_Id == id);
                return View(Menu);
            }
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        [Route("Upsert")]
        public async Task<IActionResult> Upsert(MenuList menuList)
        {
            if (menuList.Menu_Id == 0)
            {
                if (ModelState.IsValid)
                {
                    _db.MenuList.Add(menuList);
                    _db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            else
            {
                if (ModelState.IsValid)
                {
                    _db.MenuList.Update(menuList);
                    _db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            return View("Index");
        }

        [HttpGet]
        [Route("Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }
            else
            {
                var menu = await _db.MenuList.FirstOrDefaultAsync(x => x.Menu_Id == id);
                return View(menu);
            }
        }

        [HttpPost]
        [Route("Delete")]
        public async Task<IActionResult> Delete(MenuList menu)
        {
            var del_menu = await _db.MenuList.FirstOrDefaultAsync(x => x.Menu_Id == menu.Menu_Id);
            if (del_menu == null)
            {
                return NotFound();
            }
            else
            {
                _db.Remove(del_menu);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
        }
    }
}
