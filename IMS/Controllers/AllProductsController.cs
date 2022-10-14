using IMS.DataAccess.Data;
using IMS.Models.Models;
using IMS.Models.ViewModels;
using IMS.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static NuGet.Packaging.PackagingConstants;

namespace IMS.Controllers
{
    public class AllProductsController : Controller
    {
        private readonly ApplicationDbContext _db;
        public AllProductsController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            var productList = await _db.Product.Include(x => x.Category).Include(x => x.Brand).ToListAsync();
            var catList = await _db.Category.ToListAsync();
            var imgList = await _db.Clib.ToListAsync();
            //var first = true;
            foreach (var prod in productList)
            {
                var imgPath = imgList.FirstOrDefault(x => x.Prod_Id == prod.Product_Id);
                if(imgPath != null)
                {
                    if (imgPath.Image_url != null)
                    {
                        prod.ImgPath = imgPath.Image_url;
                    }
                }
                //if (first)
                //{

                //    first = false;
                //}

            }
            AllProducts allProducts = new()
            {
                Products = productList,
                Categories = catList,
                Clibs = imgList
            };
            return View(allProducts);
        }

        public async Task<IActionResult> ProductDetails(Guid id)
        {
            List<ShoppingCart> ShoppingCartList = new List<ShoppingCart>();

            if (HttpContext.Session.Get<IEnumerable<Order>>(WC.SessionCart) != null &&
                HttpContext.Session.Get<IEnumerable<Order>>(WC.SessionCart).Count() > 0)
            {
                ShoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart);
            }

            var prodDetails = await _db.Product.FirstOrDefaultAsync(x => x.Product_Id == id);
            var images = await _db.Clib.Where(x => x.Prod_Id == id).Select(x=>x.Image_url).ToListAsync();
            ProductDetailsVM productDetailsVM = new()
            {
                Product = prodDetails,
                imgList = images
            };

            foreach (var item in ShoppingCartList)
            {
                if(item.ProductId == id)
                {
                    productDetailsVM.ExistInCart = true;
                }
            }
            return View("ProductDetails", productDetailsVM);
        }

        public async Task<IActionResult> AddToCart(Guid id, ProductDetailsVM productDetailsVM)
        {
            List<ShoppingCart> ShoppingCartList = new List<ShoppingCart>();

            if (HttpContext.Session.Get<IEnumerable<Order>>(WC.SessionCart) != null &&
                HttpContext.Session.Get<IEnumerable<Order>>(WC.SessionCart).Count() > 0)
            {
                ShoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart);
            }
            ShoppingCartList.Add(new ShoppingCart { ProductId = id , Quantity = productDetailsVM.Quantity });
            HttpContext.Session.Set(WC.SessionCart, ShoppingCartList);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> RemoveFromCart(Guid id)
        {
            List<ShoppingCart> ShoppingCartList = new List<ShoppingCart>();

            if (HttpContext.Session.Get<IEnumerable<Order>>(WC.SessionCart) != null &&
                HttpContext.Session.Get<IEnumerable<Order>>(WC.SessionCart).Count() > 0)
            {
                ShoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart);
            }
            var remove_prod = ShoppingCartList.FirstOrDefault(x => x.ProductId == id);
            if(remove_prod != null)
            {
                ShoppingCartList.Remove(remove_prod);
            }
            HttpContext.Session.Set(WC.SessionCart, ShoppingCartList);
            return RedirectToAction(nameof(Index));
        }
    }
}
