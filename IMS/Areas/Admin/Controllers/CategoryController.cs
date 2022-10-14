using IMS.DataAccess.Data;
using IMS.Models.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IMS.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/Category")]
    [Authorize(Policy = "AccessChecker")]
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _db;
        public CategoryController(ApplicationDbContext db)
        {
            _db = db;
        }

        [Route("Index")]
        public IActionResult Index()
        {
            IEnumerable<Category> catList = _db.Category.ToList();
            return View(catList);
        }

        //GET- Upsert
        [HttpGet]
        [Route("Upsert")]
        public IActionResult Upsert(Guid id)
        {
            Category category = new Category();
            if (id == Guid.Empty)
            {
                //create category view
                return View(category); //view form for submit the data
            }
            else
            {
                //update category view
                var cat = _db.Category.FirstOrDefault(x => x.Category_Id == id); //KHOJ - The Search
                return View(cat); //view with the data
            }

        }

        //POST- Upsert
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        [Route("Upsert")]
        public IActionResult Upsert(Category catObj)
        {
            if (catObj.Category_Id == Guid.Empty)
            {
                //Save new entry
                if (ModelState.IsValid)
                {
                    _db.Category.Add(catObj);
                    _db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            else
            {
                //Update The existing entry
                if (ModelState.IsValid)
                {
                    _db.Category.Update(catObj);
                    _db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            return View("Index");
        }

        //GET- Delete
        [HttpGet]
        [Route("Delete")]
        public IActionResult Delete(Guid id)
        {
            var del_cat = _db.Category.FirstOrDefault(x => x.Category_Id == id);
            return View(del_cat);
        }

        //POST- Delete
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        [Route("DeleteCat")]
        public IActionResult DeleteCat(Category catId)
        {
            var del_cat = _db.Category.FirstOrDefault(x => x.Category_Id == catId.Category_Id);
            if (del_cat == null)
            {
                return NotFound();
            }
            else
            {
                _db.Category.Remove(del_cat);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
        }
    }
}
