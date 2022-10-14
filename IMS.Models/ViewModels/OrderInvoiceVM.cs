using IMS.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Models.ViewModels
{
    public class OrderInvoiceVM
    {
        public Supplier? Supplier { get; set; }
        public IEnumerable<Product>? OrderList { get; set; }
        public DateOnly Date { get; set; }
    }
}
