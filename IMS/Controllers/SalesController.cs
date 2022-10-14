using IMS.DataAccess.Data;
using IMS.Models.Models;
using IMS.Models.ViewModels;
using IMS.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SelectPdf;
using System.Drawing;
using System.Security.Claims;
using System.Text;
using static NuGet.Packaging.PackagingConstants;

namespace IMS.Controllers
{
    [Authorize]
    public class SalesController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IEmailSender _emailSender;
        public SalesController(ApplicationDbContext db, IEmailSender emailSender)
        {
            _db = db;
            _emailSender = emailSender;
        }

        public async Task<IActionResult> Index()
        {
            List<ShoppingCart> ShoppingCartList = new List<ShoppingCart>();

            if (HttpContext.Session.Get<IEnumerable<Order>>(WC.SessionCart) != null &&
                HttpContext.Session.Get<IEnumerable<Order>>(WC.SessionCart).Count() > 0)
            {
                ShoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart);
            }

            List<Guid> ProdInCart = ShoppingCartList.Select(x => x.ProductId).ToList();
            List<Product> ProdList = await _db.Product.Where(x => ProdInCart.Contains(x.Product_Id)).ToListAsync();
            if(ProdList != null)
            {
                foreach (var item in ProdList)
                {
                    item.tempQty = ShoppingCartList.FirstOrDefault(x => x.ProductId == item.Product_Id).Quantity;
                    var imgPath = _db.Clib.FirstOrDefault(x => x.Prod_Id == item.Product_Id);
                    if (imgPath != null)
                    {
                        if (imgPath.Image_url != null)
                        {
                            item.ImgPath = imgPath.Image_url;
                        }
                    }
                }
            }
            return View(ProdList);
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            List<ShoppingCart> ShoppingCartList = new List<ShoppingCart>();

            if (HttpContext.Session.Get<IEnumerable<Order>>(WC.SessionCart) != null &&
                HttpContext.Session.Get<IEnumerable<Order>>(WC.SessionCart).Count() > 0)
            {
                ShoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart);
            }
            if (ShoppingCartList != null)
            {
                var del_cart_item = ShoppingCartList.FirstOrDefault(x => x.ProductId == id);
                if(del_cart_item != null)
                {
                    ShoppingCartList.Remove(del_cart_item);
                    HttpContext.Session.Set(WC.SessionCart, ShoppingCartList);
                }
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Coupon(double ttlPrice, string coupCode)
        {
            if(coupCode == null)
            {
                return Json(new { message = "No coupon" });
            }
            var cpC = coupCode.Split(' ');
            if (cpC.Count() == 1)
            {
                var disAmnt = await _db.CouponCodes.Where(x => x.CouponCode == coupCode)
                                    .Select(x => x.CouponParcentange).FirstOrDefaultAsync();
                var couponDate = await _db.CouponCodes.Where(x => x.CouponCode == coupCode)
                                    .Select(x => x.CouponDate).FirstOrDefaultAsync();
                var todayDate = DateTime.Now;
                if (disAmnt == null || couponDate <= todayDate)
                {
                    return Json(new { success = false });
                }
                var disPrice = (ttlPrice * Convert.ToDouble(disAmnt)) / 100;
                var afterCouponAdded = Convert.ToInt32((ttlPrice - disPrice));

                //adding new price to session so that it can retrive in the invoice page
                HttpContext.Session.SetInt32("Price", afterCouponAdded);
                HttpContext.Session.SetString("couponName", coupCode);
                return Json(new { success = true, afterCouponAdded });
            }
            else
            {
                return Json(new { success = false });
            }
            
        }

        public async Task<IActionResult> Checkout()
        {
            List<Sales> ShoppingCartList = new List<Sales>();
            var userEmail = User.FindFirst(ClaimTypes.Name).Value;
            var responsible_User = await _db.ApplicationUser.FirstOrDefaultAsync(x => x.Email == userEmail);

            if (HttpContext.Session.Get<IEnumerable<Order>>(WC.SessionCart) != null &&
                HttpContext.Session.Get<IEnumerable<Order>>(WC.SessionCart).Count() > 0)
            {
                ShoppingCartList = HttpContext.Session.Get<List<Sales>>(WC.SessionCart);
            }
            var Products = await _db.Product.ToListAsync();

            var ccName = HttpContext.Session.GetString("couponName");
            foreach (var prod_Item in ShoppingCartList)
            {
                foreach (var RProd in Products)
                {
                    if (prod_Item.ProductId == RProd.Product_Id)
                    {
                        //RProd.tempQty = prod_Item.Quantity;
                        RProd.Quantity = RProd.Quantity - prod_Item.Quantity;
                        if (RProd.Quantity <= 0)
                        {
                            return Json(new { success = false });
                        }
                        else
                        {
                            _db.Product.Update(RProd);
                        }
                    }
                }
                Sales sales = new();
                var dateTime = DateTime.Now.ToShortDateString();
                prod_Item.Sale_Date = Convert.ToDateTime(dateTime);
                prod_Item.Responsible_User = responsible_User.Id;
                prod_Item.IsCoupon = false;
                prod_Item.CoupnName = null;
                if (ccName.Count() > 1)
                {
                    prod_Item.IsCoupon = true;
                    prod_Item.CoupnName = ccName;
                }
                await _db.Sales.AddAsync(prod_Item);
            }
            await _db.SaveChangesAsync();

            var subject = "New Sell Items Arrived!";
            string HtmlBody = "<strong>Hello there!</strong> <br />  <p><span style='font-size:17px;'>Need some products from your Invenoty! Here's my Information bellow.<br /> <hr /> -Name: {0} <br /> -Email: {1} <br /> Phone: {2}<br /> <hr /> Here the products list bellow<strong>:</strong></span><br /><hr />{3}<br /></p>";
            StringBuilder prodListSb = new();
            foreach (var prod in ShoppingCartList)
            {
                prod.Product_Name = await _db.Product.Where(x => x.Product_Id == prod.ProductId).Select(x => x.Product_Name).FirstOrDefaultAsync();
                prodListSb.Append($" - <span style='font-size:20px;font-family: cursive;'>Name<strong>:</strong> {prod.Product_Name}</span> <mark style='font-size:20px;'>(Quantity: {prod.Quantity})</mark><br />");
            }

            string messageBody = string.Format(HtmlBody,
                responsible_User.Full_Name,
                responsible_User.Email,
                responsible_User.PhoneNumber,
                prodListSb.ToString());

           // await _emailSender.SendEmailAsync(WC.AdminMail, subject, messageBody);

            List<Product> ProdItem = new();
            foreach (var sale_Items in ShoppingCartList)
            {
                foreach (var RProd in Products)
                {
                    if (sale_Items.ProductId == RProd.Product_Id)
                    {
                        RProd.tempQty = sale_Items.Quantity;
                        ProdItem.Add(RProd);
                    }
                }
            }
            HttpContext.Session.Remove(WC.SessionCart);
            //get the new price with coupon discount
            var price = HttpContext.Session.GetInt32("Price");
            if(price == null)
            {
                price = 0;
            }
            SaleInvoiceVM saleInvoiceVM = new()
            {
                ApplicationUser = responsible_User,
                SalesItems = ProdItem,
                //if its 0 then in invoice table we calculate the total price and send the data.
                TempPrice = (int)price,
            };
            HttpContext.Session.Remove("Price");
            HttpContext.Session.Remove("couponName");
            return View(saleInvoiceVM);
        }

        [HttpPost]
        public async Task<IActionResult> GenerateInvoice(string html)
        {
            html = html.Replace("startTag", "<").Replace("endTag", ">");
            HtmlToPdf obj = new HtmlToPdf();
            obj.Options.CssMediaType = HtmlToPdfCssMediaType.Print;
            obj.Options.ColorSpace = PdfColorSpace.CMYK;
            obj.Options.AutoFitWidth = SelectPdf.HtmlToPdfPageFitMode.AutoFit;
            obj.Options.PdfPageSize = PdfPageSize.Custom;
            obj.Options.PdfPageCustomSize = new SizeF(200, 300);
            obj.Options.MarginBottom = 10;
            obj.Options.MarginLeft = 10;
            obj.Options.MarginRight = 10;
            obj.Options.MarginTop = 10;
            PdfDocument doc = obj.ConvertHtmlString(html);
            byte[] pdf = doc.Save();
            doc.Close();
            return File(pdf, "application/pdf");
        }
    }
}
