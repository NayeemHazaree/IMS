using IMS.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Models.ViewModels
{
    public class OrderListVM
    {
        public OrderHeader? OrderHeader { get; set; }
        public IEnumerable<Product>? Product { get; set; }
    }
}
