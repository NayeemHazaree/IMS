using IMS.DataAccess.Data;
using IMS.Models.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IMS.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/Brand")]
    [Authorize(Policy = "AccessChecker")]
    public class BrandController : Controller
    {
        private readonly ApplicationDbContext _db;
        public BrandController(ApplicationDbContext db)
        {
            _db = db;
        }

        [Route("Index")]
        public IActionResult Index()
        {
            IEnumerable<Brand> brandList = _db.Brand.ToList();
            return View(brandList);
        }

        //GET- Upsert
        [HttpGet]
        [Route("Upsert")]
        public IActionResult Upsert(Guid id)
        {
            Brand brand = new Brand();
            if (id == Guid.Empty)
            {
                //create brand view
                return View(brand);
            }
            else
            {
                //update brand view
                var br = _db.Brand.FirstOrDefault(x => x.Brand_Id == id);
                return View(br);
            }
        }

        //POST- Upsert
        [HttpPost]
        [Route("Upsert")]
        [AutoValidateAntiforgeryToken]
        public IActionResult Upsert(Brand br)
        {
            if (br.Brand_Id == Guid.Empty)
            {
                //Save new entry
                if (ModelState.IsValid)
                {
                    _db.Brand.Add(br);
                    _db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            else
            {
                //Update The existing entry
                if (ModelState.IsValid)
                {
                    _db.Brand.Update(br);
                    _db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            return View("Index");
        }

        //GET-Delete
        [HttpGet]
        [Route("Delete")]
        public IActionResult Delete(Guid id)
        {
            if (id == Guid.Empty)
            {
                return NotFound();
            }
            else
            {
                var del_br = _db.Brand.FirstOrDefault(x => x.Brand_Id == id);
                return View(del_br);
            }
        }

        //POST- Delete
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        [Route("DeleteBr")]
        public IActionResult DeleteBr(Brand br)
        {
            var del_br = _db.Brand.FirstOrDefault(x => x.Brand_Id == br.Brand_Id);
            if (del_br == null)
            {
                return NotFound();
            }
            else
            {
                _db.Brand.Remove(del_br);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
        }
    }
}
