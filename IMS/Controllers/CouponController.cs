using IMS.DataAccess.Data;
using IMS.Models.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IMS.Controllers
{
    public class CouponController : Controller
    {
        private readonly ApplicationDbContext _db;
        public CouponController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            IEnumerable<CouponCodes> couponsList = _db.CouponCodes.ToList();
            return View(couponsList);
        }

        //GET- Upsert
        [HttpGet]
        public IActionResult Upsert(Guid id)
        {
            CouponCodes coupon = new();
            if (id == Guid.Empty)
            {
                //create brand view
                return View(coupon);
            }
            else
            {
                //update brand view
                var ExistCC = _db.CouponCodes.FirstOrDefault(x => x.Coupon_Id == id);
                return View(ExistCC);
            }
        }

        //POST- Upsert
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Upsert(CouponCodes cc)
        {
            if (cc.Coupon_Id == Guid.Empty)
            {
                //Save new entry
                if (ModelState.IsValid)
                {
                    await _db.CouponCodes.AddAsync(cc);
                    await _db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
            }
            else
            {
                //Update The existing entry
                if (ModelState.IsValid)
                {
                    _db.CouponCodes.Update(cc);
                    await _db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
            }
            return View("Index");
        }

        //GET-Delete
        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
            if (id == Guid.Empty)
            {
                return NotFound();
            }
            else
            {
                var del_cc = await _db.CouponCodes.FirstOrDefaultAsync(x => x.Coupon_Id == id);
                return View(del_cc);
            }
        }

        //POST- Delete
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> DeleteCc(CouponCodes cc)
        {
            var del_cc = await _db.CouponCodes.FirstOrDefaultAsync(x => x.Coupon_Id == cc.Coupon_Id);
            if (del_cc == null)
            {
                return NotFound();
            }
            else
            {
                _db.CouponCodes.Remove(del_cc);
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
        }
    }
}
