using IMS.DataAccess.Data;
using IMS.Models.Models;
using IMS.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace IMS.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/Transfer")]
    [Authorize(Policy = "AccessChecker")]
    public class TransferController : Controller
    {
        private readonly ApplicationDbContext _db;
        public TransferController(ApplicationDbContext db)
        {
            _db = db;
        }

        [Route("Index")]
        public async Task<IActionResult> Index()
        {
            var prod = _db.Product.Select(i => new SelectListItem
            {
                Text = i.Product_Name,
                Value = i.Product_Id.ToString()
            });
            var branch = _db.Branch.Select(i => new SelectListItem
            {
                Text = i.BranchName,
                Value = i.BranchId.ToString()
            });
            TransferVM transferVM = new()
            {
                ProductList = prod,
                BranchList = branch
            };
            return View(transferVM);
        }

        [Route("TransferProd")]
        public async Task<IActionResult> TransferProd(List<Guid> ProdVal,Guid branchID, int qty, bool IsBranch)
        {
            if(ProdVal != null && branchID != Guid.Empty && qty != 0)
            {
                foreach (var prodId in ProdVal)
                {
                    if(IsBranch == true)
                    {
                        //send products to branches from head office
                        var realProd = await _db.Product.FirstOrDefaultAsync(x => x.Product_Id == prodId);
                        if (realProd != null)
                        {
                            realProd.Quantity = realProd.Quantity - qty;
                            if (realProd.Quantity > 0)
                            {
                                _db.Product.Update(realProd);
                                var branchProd = await _db.BranchProducts.FirstOrDefaultAsync(x => x.BranchId == branchID && x.ProductId == prodId);
                                branchProd.Quantity = branchProd.Quantity + qty;
                                _db.BranchProducts.Update(branchProd);
                            }
                            else
                            {
                                return Json(new { success = false });
                            }
                        }
                        else
                        {
                            return Json(new { success = false });
                        }
                    }
                    else
                    {
                        //send products to head office from branches
                        var branchProd = await _db.BranchProducts.FirstOrDefaultAsync(x => x.BranchId == branchID && x.ProductId == prodId);
                        if (branchProd != null)
                        {
                            branchProd.Quantity = branchProd.Quantity - qty;
                            if (branchProd.Quantity > 0)
                            {
                                _db.BranchProducts.Update(branchProd);
                                var realProd = await _db.Product.FirstOrDefaultAsync(x => x.Product_Id == prodId);
                                realProd.Quantity = realProd.Quantity + qty;
                                _db.Product.Update(realProd);
                            }
                            else
                            {
                                return Json(new { success = false });
                            }
                        }
                        else
                        {
                            return Json(new { success = false });
                        }
                    }
                }
                await _db.SaveChangesAsync();
                return Json(new { success = true });
            }
            else
            {
                return Json(new { success = false });
            }
        }

        
    }
}
