using IMS.DataAccess.Data;
using IMS.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Transactions;

namespace IMS.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/BranchWiseProduct")]
    [Authorize(Policy = "AccessChecker")]
    public class BranchWiseProductController : Controller
    {
        private readonly ApplicationDbContext _db;

        public BranchWiseProductController(ApplicationDbContext db)
        {
            _db = db;
        }

        [Route("Index")]
        public async Task<IActionResult> Index()
        {
            var branch = _db.Branch.Select(i => new SelectListItem
            {
                Text = i.BranchName,
                Value = i.BranchId.ToString()
            });
            BranchWiseProductVM branchWiseProductVM = new()
            {
                branchList = branch
            };
            return View(branchWiseProductVM);
        }

        [Route("SearchBranch")]
        public async Task<IActionResult> SearchBranch(Guid id)
        {
            var userId = User.FindFirst(ClaimTypes.Name);
            var branchProd = await _db.BranchProducts.Where(x => x.BranchId == id).ToListAsync();
            foreach (var item in branchProd)
            {
                item.ProductName = await _db.Product.Where(x => x.Product_Id == item.ProductId).Select(x => x.Product_Name).FirstOrDefaultAsync();
                item.StoreName = await _db.Suppliers.Where(x => x.SupplierId == item.StoreId).Select(x => x.SupplerName).FirstOrDefaultAsync();
                item.ResponsiblePersonName = await _db.ApplicationUser.Where(x => x.Email == userId.Value).Select(x => x.Full_Name).FirstOrDefaultAsync();
            }
            return PartialView("_SearchBranch", branchProd);
        }
    }
}
