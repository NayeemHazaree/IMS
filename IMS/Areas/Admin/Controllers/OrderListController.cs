
using IMS.DataAccess.Data;
using IMS.Models.Models;
using IMS.Models.ViewModels;
using IMS.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Nodes;
using static NuGet.Packaging.PackagingConstants;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
using SelectPdf;
using Microsoft.CodeAnalysis.RulesetToEditorconfig;
using System.Drawing;

namespace IMS.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/OrderList")]
    public class OrderListController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IEmailSender _emailSender;
        private readonly IHostingEnvironment _hostingEnvironment;
       

        public OrderListController(ApplicationDbContext db, IEmailSender emailSender, IHostingEnvironment hostingEnvironment)
        {
            _db = db;
            _emailSender = emailSender;
            _hostingEnvironment = hostingEnvironment;
        }

        [Route("Index")]
        public async Task<IActionResult> Index()
        {
            var storesName = _db.Suppliers.Select(i => new SelectListItem
            {
                Text = i.SupplierStoreName,
                Value = i.SupplierId.ToString(),
                Selected = i.Status
            });
            var prodName = _db.Product.Select(i => new SelectListItem
            {
                Text = i.Product_Name,
                Value = i.Product_Id.ToString()
            });

            var AvailableStores = storesName.Where(x => x.Selected == true);
            storesName = AvailableStores;
            OrderList orderList = new()
            {
                Stores = storesName,
                Products = prodName
            };
            return View(orderList);
        }

        [Route("GetOrderQty")]
        public async Task<IActionResult> GetOrderQty()
        {
            OrderList orderlist = new();
            return PartialView("_OrderDetails", orderlist);
        }

        [HttpPost]
        [Route("SaveOrder")]
        public async Task<IActionResult> SaveOrder(Guid storeId, Guid prodId, int qty)
        {
            List<Order> orders = new List<Order>();

            if (HttpContext.Session.Get<IEnumerable<Order>>(WC.OrderCart) != null &&
                HttpContext.Session.Get<IEnumerable<Order>>(WC.OrderCart).Count() > 0)
            {
                orders = HttpContext.Session.Get<List<Order>>(WC.OrderCart);
            }

            var delItem = orders.FirstOrDefault(x => x.ProductId == prodId);
            if (delItem != null)
            {
                orders.Remove(delItem);
            }

            orders.Add(new Order { StoreId = storeId, ProductId = prodId, Quantity = qty });
            HttpContext.Session.Set(WC.OrderCart, orders);
            var countObj = HttpContext.Session.Get<IEnumerable<Order>>(WC.OrderCart).Count();
            var Products = await _db.Product.ToListAsync();
            List<Product> ProdItem = new();
            foreach (var prod_Item in orders)
            {
                foreach (var RProd in Products)
                {
                    if (prod_Item.ProductId == RProd.Product_Id)
                    {
                        RProd.tempQty = prod_Item.Quantity;
                        ProdItem.Add(RProd);
                    }
                }
            }
            return Json(new { data = ProdItem, countObj });
        }

        [HttpPost]
        [Route("SendRequest")]
        public async Task<IActionResult> SendRequest()
        {
            //List<Order> orders = new List<Order>();
            List<OrderList> orders = new List<OrderList>();
            var userEmail = User.FindFirst(ClaimTypes.Name).Value;
            var responsible_User = await _db.ApplicationUser.FirstOrDefaultAsync(x => x.Email == userEmail);

            if (HttpContext.Session.Get<IEnumerable<Order>>(WC.OrderCart) != null &&
                HttpContext.Session.Get<IEnumerable<Order>>(WC.OrderCart).Count() > 0)
            {
                orders = HttpContext.Session.Get<List<OrderList>>(WC.OrderCart);
            }
            var Products = await _db.Product.ToListAsync();
            string StoreEmail = "";
            foreach (var prod_Item in orders)
            {
                foreach (var RProd in Products)
                {
                    if (prod_Item.ProductId == RProd.Product_Id)
                    {
                        //RProd.tempQty = prod_Item.Quantity;
                        RProd.Quantity = RProd.Quantity + prod_Item.Quantity;
                        _db.Product.Update(RProd);
                    }
                }
                StoreEmail = await _db.Suppliers.Where(x => x.SupplierId == prod_Item.StoreId).Select(x => x.SupplierEmail).FirstOrDefaultAsync();
                OrderList orderList = new();
                var dateTime = DateTime.Now.ToShortDateString();
                prod_Item.OrderDate = Convert.ToDateTime(dateTime);
                prod_Item.Responsible_User = responsible_User.Id;
                await _db.OrderLists.AddAsync(prod_Item);
            }
            await _db.SaveChangesAsync();

            var subject = "New Order for Products!";
            string HtmlBody = "<strong>Hello there!</strong> <br />  <p><span style='font-size:17px;'>Need some products from your store! Giving the products list bellow<strong>:</strong></span><br />{0}<br /></p><strong>Address: Dhaka <br />Phone:01234567</strong> <br /><br /><br /> <strong>Kind Regards,<br /> IMS Team</strong> ";
            StringBuilder prodListSb = new();
            foreach (var prod in orders)
            {
                prod.Product_Name = await _db.Product.Where(x => x.Product_Id == prod.ProductId).Select(x => x.Product_Name).FirstOrDefaultAsync();
                prodListSb.Append($" - <span style='font-size:20px;font-family: cursive;'>Name<strong>:</strong> {prod.Product_Name}</span> <mark style='font-size:20px;'>(Quantity: {prod.Quantity})</mark><br />");
            }

            string messageBody = string.Format(HtmlBody, prodListSb.ToString());

            await _emailSender.SendEmailAsync(StoreEmail, subject, messageBody);
            //HttpContext.Session.Remove(WC.OrderCart);
            var SupplierInfo = await _db.Suppliers.Where(x => x.SupplierEmail == StoreEmail).FirstOrDefaultAsync();
            List<Product> ProdItem = new();
            foreach (var order in orders)
            {
                foreach (var RProd in Products)
                {
                    if (order.ProductId == RProd.Product_Id)
                    {
                        RProd.tempQty = order.Quantity;
                        ProdItem.Add(RProd);
                    }
                }
            }
            OrderInvoiceVM orderInvoiceVM = new()
            {
                Supplier = SupplierInfo,
                OrderList = ProdItem
            };
            HttpContext.Session.Remove(WC.OrderCart);

            return View(orderInvoiceVM);
        }

        [Route("GenerateInvoice")]
        [HttpPost]
        public async Task<IActionResult> GenerateInvoice(string html)
        {
            ////Initialize HTML to PDF converter with Blink rendering engine
            //HtmlToPdfConverter htmlConverter = new HtmlToPdfConverter(HtmlRenderingEngine.Blink);

            //BlinkConverterSettings settings = new BlinkConverterSettings();

            ////Set the BlinkBinaries folder path 
            //settings.BlinkPath = Path.Combine(_hostingEnvironment.ContentRootPath, "BlinkBinariesWindows");

            ////Assign Blink settings to HTML converter
            //htmlConverter.ConverterSettings = settings;

            ////Convert URL to PDF
            //PdfDocument document = htmlConverter.Convert("https://localhost:44355/Admin/Brand/Index");

            ////Saving the PDF to the MemoryStream
            //MemoryStream stream = new MemoryStream();

            //document.Save(stream);

            ////Download the PDF document in the browser
            //return File(stream.ToArray(), System.Net.Mime.MediaTypeNames.Application.Pdf, "Output.pdf");

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

        [HttpPost]
        [Route("PurchaseHistory")]
        public async Task<IActionResult> PurchaseHistory(string OrderDate)
        {
            DateTime OrderDT = DateTime.Parse(OrderDate);
            var OrderedItems = await _db.OrderLists.ToListAsync();
            List<OrderList> history = new();
            foreach (var order in OrderedItems)
            {
                if (order.OrderDate <= OrderDT)
                {
                    order.Store_Name = await _db.Suppliers.Where(x => x.SupplierId == order.StoreId).Select(x => x.SupplerName).FirstOrDefaultAsync();
                    order.Product_Name = await _db.Product.Where(x => x.Product_Id == order.ProductId).Select(x => x.Product_Name).FirstOrDefaultAsync();
                    order.Responsible_Persone_Name = await _db.ApplicationUser.Where(x => x.Id == order.Responsible_User).Select(x => x.Full_Name).FirstOrDefaultAsync();
                    history.Add(order);
                }
            }
            return PartialView("_PurchaseHistory", history);
        }
    }
}
