using IMS.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Models.ViewModels
{
    public class ProductDetailsVM
    {
        public ProductDetailsVM()
        {
            Product = new Product();
            Quantity = 1;
        }
        public Product Product { get; set; }
        public List<string> imgList { get; set; }
        public bool ExistInCart { get; set; }
        public int Quantity { get; set; }
    }
}
