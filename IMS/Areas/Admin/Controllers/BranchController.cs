using IMS.DataAccess.Data;
using IMS.Models.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IMS.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/Branch")]
    public class BranchController : Controller
    {
        private readonly ApplicationDbContext _db;
        public BranchController(ApplicationDbContext db)
        {
            _db = db;
        }

        [Route("Index")]
        public async Task<IActionResult> Index()
        {
            IEnumerable<Branch> BranchList = await _db.Branch.ToListAsync();
            return View(BranchList);
        }

        [Route("Upsert")]
        [HttpGet]
        public async Task<IActionResult> Upsert(Guid id)
        {
            if(id == Guid.Empty)
            {
                //create view
                Branch branch = new Branch();
                return View(branch);
            }
            else
            {
                //update view
                var branch = await _db.Branch.FirstOrDefaultAsync(x => x.BranchId == id);
                return View(branch);
            }
        }

        [Route("Upsert")]
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Upsert(Branch br)
        {
            if(br.BranchId == Guid.Empty)
            {
                //save new 
                if (ModelState.IsValid)
                {
                    await _db.Branch.AddAsync(br);
                    await _db.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            else
            {
                //update 
                if (ModelState.IsValid)
                {
                    _db.Branch.Update(br);
                    await _db.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            return View();
        }

        [Route("Delete")]
        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
            if (id == Guid.Empty)
            {
                return NotFound();
            }
            else
            {
                var del_br = await _db.Branch.FirstOrDefaultAsync(x => x.BranchId == id);
                return View(del_br);
            }
        }

        [Route("Delete")]
        [HttpPost]
        public async Task<IActionResult>Delete(Branch br)
        {
            var del_br = await _db.Branch.FirstOrDefaultAsync(x => x.BranchId == br.BranchId);
            if (del_br == null)
            {
                return NotFound();
            }
            else
            {
                _db.Branch.Remove(del_br);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
