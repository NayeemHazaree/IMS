using IMS.Models.Models;
using IMS.Models.Models.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Models.ViewModels
{
    public class SaleInvoiceVM
    {
        public ApplicationUser? ApplicationUser { get; set; }
        public IEnumerable<Product>? SalesItems { get; set; }
        public int TempPrice { get; set; }
    }
}
