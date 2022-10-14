using IMS.DataAccess.Data;
using IMS.Models;
using IMS.Models.Models;
using IMS.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Diagnostics;

namespace IMS.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            return View();
        }

        public async Task<IActionResult> ProductDetails(Guid id)
        {
            if(id != Guid.Empty)
            {
                var DetailsImgList = await _db.Clib.Where(x => x.Prod_Id == id).ToListAsync();
                List<string> collectedImgPath = new List<string>();
                foreach (var img in DetailsImgList)
                {
                    collectedImgPath.Add(img.Image_url);
                }
                ProductDetailsVM productDetailsVM = new ProductDetailsVM()
                {
                    Product = await _db.Product.Include(x => x.Category).Include(x => x.Brand)
                    .Where(x => x.Product_Id == id).FirstOrDefaultAsync(),
                    imgList = collectedImgPath,
                    ExistInCart = false
                };
                return View(productDetailsVM);
            }
            return NotFound();
        }
    

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
}