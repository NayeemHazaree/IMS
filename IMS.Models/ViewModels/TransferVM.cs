using IMS.Models.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Models.ViewModels
{
    public class TransferVM
    {
        public TransferVM()
        {
            Quantity = 1;
        }
        public Guid ProductId { get; set; }
        public Guid BranchId { get; set; }
        public int Quantity { get; set; }
        public IEnumerable<SelectListItem>? ProductList { get; set; }
        public IEnumerable<SelectListItem>? BranchList { get; set; }
    }
}
