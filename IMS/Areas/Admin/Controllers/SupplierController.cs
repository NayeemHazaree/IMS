using IMS.DataAccess.Data;
using IMS.Models.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using System.Security.Cryptography.X509Certificates;

namespace IMS.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/Supplier")]
    [Authorize(Policy = "AccessChecker")]
    public class SupplierController : Controller
    {
        private readonly ApplicationDbContext _db;
        public SupplierController(ApplicationDbContext db)
        {
            _db = db;
        }
        [Route("Index")]
        public async Task<IActionResult> Index()
        {
            Supplier supplier = new();
            return View(supplier);
        }

        [Route("GetSupplierTable")]
        public async Task<IActionResult> GetSupplierTable()
        {
            IEnumerable<Supplier> suppliersList = await _db.Suppliers.ToListAsync();
            return PartialView("_SupplierTable",suppliersList);
        }

        [HttpGet]
        [Route("Upsert")]
        public async Task<IActionResult> Upsert(Guid id)
        {
            Supplier supplier = new();
            if (id == Guid.Empty)
            {
                //create view
                return Json(new { success = true , data = supplier , message="Create Mode"});
            }
            else
            {
                //update view
                var supplierObj = await _db.Suppliers.SingleOrDefaultAsync(x=>x.SupplierId == id);
                if(supplierObj != null)
                {
                    return Json(new { success = true , data = supplierObj, message = "Edit Mode" });
                }
                return View();
            }
            
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        [Route("Upsert")]
        public async Task<IActionResult> Upsert(Supplier supp)
        {
            if (supp.SupplierId == Guid.Empty)
            {
                supp.Status = true;
                //save entry
                if (ModelState.IsValid)
                {
                    await _db.Suppliers.AddAsync(supp);
                    await _db.SaveChangesAsync();
                }
                return Json(new { success = true });
            }
            else
            {
                //update entry
                if (ModelState.IsValid)
                {
                    _db.Suppliers.Update(supp);
                    await _db.SaveChangesAsync();
                }
                return Json(new { success = true });
            }
        }

        [HttpPost]
        [Route("Delete")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var del_supplier = await _db.Suppliers.FirstOrDefaultAsync(x => x.SupplierId == id);
            if(del_supplier != null)
            {
                _db.Suppliers.Remove(del_supplier);
                await _db.SaveChangesAsync();
            }
            return Json(new { success = true });
        }
    }
}
