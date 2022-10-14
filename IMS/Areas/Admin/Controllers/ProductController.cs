using IMS.DataAccess.Data;
using IMS.Models.Models;
using IMS.Models.ViewModels;
using IMS.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace IMS.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/Product")]
    [Authorize(Policy = "AccessChecker")]
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(ApplicationDbContext db, IWebHostEnvironment webHostEnvironment)
        {
            _db = db;
            _webHostEnvironment = webHostEnvironment;
        }

        [Route("Index")]
        public IActionResult Index()
        {
            ProductVM productVm = new ProductVM()
            {
                Product = new Product(),
                CategorySelectList = _db.Category.Select(i => new SelectListItem
                {
                    Text = i.Category_Name,
                    Value = i.Category_Id.ToString()
                }),
                BrandSelectList = _db.Brand.Select(i => new SelectListItem
                {
                    Text = i.Brand_Name,
                    Value = i.Brand_Id.ToString(),
                    Selected = i.Brand_Status
                })
            };
            //select brand whose status is true.
            var brand_items = productVm.BrandSelectList.Where(x => x.Selected == true);
            productVm.BrandSelectList = brand_items;
            return View(productVm);
        }
        #region API CALLS
        [HttpGet]
        [Route("getProductList")]
        public IActionResult getProductList()
        {
            IEnumerable<Product> prod_List = _db.Product.Include(x => x.Category).Include(x => x.Brand).ToList();
            return Json(new { data = prod_List });
        }
        #endregion

        [HttpGet]
        [Route("Upsert")]
        public IActionResult Upsert(Guid id)
        {
            ProductVM productVm = new ProductVM()
            {
                Product = new Product(),
                CategorySelectList = _db.Category.Select(i => new SelectListItem
                {
                    Text = i.Category_Name,
                    Value = i.Category_Id.ToString()
                }),
                BrandSelectList = _db.Brand.Select(i => new SelectListItem
                {
                    Text = i.Brand_Name,
                    Value = i.Brand_Id.ToString()
                })
            };
            if (id == Guid.Empty)
            {
                //create view
                return Json(new { data = productVm });
            }
            else
            {
                //update view
                var prod = _db.Product.FirstOrDefault(x => x.Product_Id == id);
                if (prod != null)
                {
                    //get the category and brand value
                    var cateVal = productVm.CategorySelectList.Where(x => x.Value == prod.Parent_Cat_Id.ToString().ToUpper()).First().Value;
                    var brandVal = productVm.BrandSelectList.Where(x => x.Value == prod.Parent_brand_Id.ToString().ToUpper()).First().Value;
                    var imgList = _db.Clib.Where(x => x.Prod_Id == id).Select(x => x.Image_url).ToList();
                    return Json(new { data = prod, cateVal, brandVal, img_list = imgList });
                }
            }
            return View();
        }

        //POST- Upsert
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        [Route("Upsert")]
        public IActionResult Upsert(ProductVM productVm, IList<IFormFile> up_img)
        {

            if (productVm.Product != null)
            {
                if (productVm.Product.Product_Id == Guid.Empty)
                {
                    //save the entry
                    if (ModelState.IsValid)
                    {
                        _db.Product.Add(productVm.Product);
                        _db.SaveChanges();
                        if (up_img.Count > 0)
                        {
                            foreach (var img in up_img)
                            {
                                string webRootPath = _webHostEnvironment.WebRootPath;
                                string prod_photo = webRootPath + WC.p_image_path;
                                string photo_name = Guid.NewGuid().ToString();
                                string photo_extension = Path.GetExtension(img.FileName);

                                using (var fileStream = new FileStream(Path.Combine(prod_photo, photo_name + photo_extension), FileMode.Create))
                                {
                                    img.CopyTo(fileStream);
                                }
                                Clib clib = new Clib();
                                clib.Image_url = photo_name + photo_extension;

                                clib.Prod_Id = productVm.Product.Product_Id;
                                _db.Clib.Add(clib);
                                _db.SaveChanges();
                            }
                        }
                        return Json(new { data = productVm });
                    }


                }
                else
                {
                    //update
                    if (ModelState.IsValid)
                    {
                        _db.Product.Update(productVm.Product);
                        _db.SaveChanges();

                        if (up_img.Count > 0)
                        {

                            foreach (var img in up_img)
                            {
                                string webRootPath = _webHostEnvironment.WebRootPath;
                                string prod_photo = webRootPath + WC.p_image_path;
                                string photo_name = Guid.NewGuid().ToString();
                                string photo_extension = Path.GetExtension(img.FileName);

                                var pathInDB = _db.Clib.Where(x => x.Prod_Id == productVm.Product.Product_Id).Select(x => x.Image_url).ToList();

                                foreach (var path in pathInDB)
                                {
                                    var delete_img_Id = _db.Clib.FirstOrDefault(x => x.Prod_Id == productVm.Product.Product_Id);
                                    Clib clib = new Clib();
                                    if (delete_img_Id != null)
                                    {
                                        _db.Clib.Remove(delete_img_Id);
                                        _db.SaveChanges();
                                    }

                                    if (path != null)
                                    {
                                        var imageData = Path.Combine(prod_photo, path.TrimStart('\\'));
                                        if (System.IO.File.Exists(imageData))
                                        {
                                            System.IO.File.Delete(imageData);
                                        }
                                        //Clib clib_img = new Clib();
                                        //_db.SaveChanges();
                                    }

                                }



                                using (var fileStream = new FileStream(Path.Combine(prod_photo, photo_name + photo_extension), FileMode.Create))
                                {
                                    img.CopyTo(fileStream);
                                }
                                Clib clibTable = new Clib();
                                clibTable.Image_url = photo_name + photo_extension;

                                clibTable.Prod_Id = productVm.Product.Product_Id;
                                _db.Clib.Add(clibTable);
                                _db.SaveChanges();
                            }
                        }
                        return Json(new { data = productVm });
                    }
                }
            }

            return View("Index");
        }

        [HttpPost]
        [Route("Delete")]
        public IActionResult Delete(Guid id)
        {
            var del_prod = _db.Product.FirstOrDefault(x => x.Product_Id == id);
            if (del_prod != null)
            {
                var pathInDB = _db.Clib.Where(x => x.Prod_Id == id).Select(x => x.Image_url).ToList();


                foreach (var path in pathInDB)
                {
                    if (path != null)
                    {
                        string webRootPath = _webHostEnvironment.WebRootPath;
                        string prod_photo = webRootPath + WC.p_image_path;
                        var imageData = Path.Combine(prod_photo, path.TrimStart('\\'));
                        if (System.IO.File.Exists(imageData))
                        {
                            System.IO.File.Delete(imageData);
                        }
                    }
                }
                _db.Product.Remove(del_prod);
                _db.SaveChanges();
                return Json(new { success = true, message = "Successfully Deleted" });
            }
            return Json(new { success = false, message = "Couldn't Delete" });
        }
    }

}
