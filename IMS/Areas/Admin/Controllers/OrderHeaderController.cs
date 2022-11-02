using IMS.DataAccess.Data;
using IMS.Models.Models;
using IMS.Models.ViewModels;
using IMS.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;
using static NuGet.Packaging.PackagingConstants;

namespace IMS.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/OrderHeader")]
    [Authorize(Policy = "AccessChecker")]
    public class OrderHeaderController : Controller
    {
        private readonly ApplicationDbContext _db; 
        private readonly IEmailSender _emailSender;
        public OrderHeaderController(ApplicationDbContext db, IEmailSender emailSender)
        {
            _db = db;
            _emailSender = emailSender;
        }

        [Route("Index")]
        public async Task<IActionResult> Index()
        {
            IEnumerable<OrderHeader> orderHeaders = await _db.OrderHeaders.ToListAsync();
            return View(orderHeaders);
        }

        [Route("Details")]
        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var OrderDetails = await _db.OrderDetails.Where(x => x.OrderDetailsId == id).ToListAsync();
            var orderHeader = await _db.OrderHeaders.FirstOrDefaultAsync(x => x.Id == id);

            orderHeader.Store_Name = _db.Suppliers
                                    .FirstOrDefault(x => x.SupplierId == orderHeader.StoreId).SupplierStoreName;

            orderHeader.Responsible_Persone_Name = _db.ApplicationUser
                                    .FirstOrDefault(x => x.Id == orderHeader.Responsible_User).Full_Name;

            if(orderHeader.BranchId != Guid.Empty)
            {
                orderHeader.Branch_Name = _db.Branch
                                    .FirstOrDefault(x => x.BranchId == orderHeader.BranchId).BranchName;
            }
            else
            {
                orderHeader.Branch_Name = "Head Office";
            }
            
            List<Product> Products = new();
            if(OrderDetails != null)
            {
                foreach (var OrderItem in OrderDetails)
                {
                    //branchName = _db.Branch.FirstOrDefault(x => x.BranchId == OrderItem.BranchId).BranchName;
                    //OrderItem.BranchName = branchName;
                    var prod_Item = await _db.Product.FirstOrDefaultAsync(x => x.Product_Id == OrderItem.ProductId);
                    prod_Item.tempQty = OrderItem.Quantity;
                    Products.Add(prod_Item);
                }
            }
            OrderListVM orderListVM = new()
            {
                OrderHeader = orderHeader,
                Product = Products
            };
            return View(orderListVM);
        }

        [Route("Approve")]
        [HttpGet]
        public async Task<IActionResult> Approve(Guid id)
        {
            if(id != Guid.Empty)
            {
                var orderHeaderEle = await _db.OrderHeaders.FirstOrDefaultAsync(x => x.Id == id);
                var orderDtEle = _db.OrderDetails.Where(x => x.OrderDetailsId == id).ToList();
                bool sentMail = false;
                if(orderHeaderEle.OrderStatus != WC.Submitted && orderHeaderEle.OrderStatus != WC.Cancel)
                {
                    orderHeaderEle.OrderStatus = WC.Submitted;
                }
                else
                {
                    return Json(new { success = false, mssage = "Already Submitted"});
                }
                foreach (var ordrEle in orderDtEle)
                {
                    if (orderHeaderEle.BranchId == Guid.Empty)
                    {
                        var existProd = await _db.Product.FirstOrDefaultAsync(x => x.Product_Id == ordrEle.ProductId);
                        existProd.Quantity = existProd.Quantity + ordrEle.Quantity;
                        _db.Product.Update(existProd);
                    }
                    else
                    {
                        var brProd = await _db.BranchProducts.FirstOrDefaultAsync(x => x.ProductId == ordrEle.ProductId);
                        brProd.Quantity = brProd.Quantity + ordrEle.Quantity;
                        _db.BranchProducts.Update(brProd);
                    }
                    if(sentMail == false)
                    {
                        sentMail = true;
                    }
                }
                if(sentMail == true)
                {
                    var subject = "New Order for Products!";
                    var StoreEmail = await _db.Suppliers.Where(x => x.SupplierId == orderHeaderEle.StoreId).Select(x => x.SupplierEmail).FirstOrDefaultAsync();
                    string HtmlBody = "<strong>Hello there!</strong> <br />  <p><span style='font-size:17px;'>Need some products from your store! Giving the products list bellow<strong>:</strong></span><br />{0}<br /></p><strong>Address: Dhaka <br />Phone:01234567</strong> <br /><br /><br /> <strong>Kind Regards,<br /> IMS Team</strong> ";
                    StringBuilder prodListSb = new();
                    foreach (var item in orderDtEle)
                    {
                        item.Product_Name = await _db.Product.Where(x => x.Product_Id == item.ProductId).Select(x => x.Product_Name).FirstOrDefaultAsync();
                        prodListSb.Append($" - <span style='font-size:20px;font-family: cursive;'>Name<strong>:</strong> {item.Product_Name}</span> <mark style='font-size:20px;'>(Quantity: {item.Quantity})</mark><br />");
                    }
                    string messageBody = string.Format(HtmlBody, prodListSb.ToString());
                    await _emailSender.SendEmailAsync(StoreEmail, subject, messageBody);
                }

                await _db.SaveChangesAsync();
                return Json(new { success = true });
            }
            else
            {
                return Json(new { success = false });
            }
        }

        [Route("Decline")]
        [HttpGet]
        public async Task<IActionResult> Decline(Guid id)
        {
            var orderHeaderEle = await _db.OrderHeaders.FirstOrDefaultAsync(x => x.Id == id);
            if(orderHeaderEle.OrderStatus != WC.Cancel && orderHeaderEle.OrderStatus != WC.Submitted)
            {
                orderHeaderEle.OrderStatus = WC.Cancel;
            }
            else
            {
                return Json(new { success = false });
            }
            await _db.SaveChangesAsync();
            return Json(new { success = true });
        }
    }
}
