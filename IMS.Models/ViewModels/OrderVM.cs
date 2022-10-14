using IMS.Models.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Models.ViewModels
{
    public class OrderVM
    {
        public Order? Order { get; set; }
        public IEnumerable<SelectListItem>? Stores { get; set; }
        public IEnumerable<SelectListItem>? Products { get; set; }
    }
}
