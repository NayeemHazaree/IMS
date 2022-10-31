using IMS.Models.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Models.ViewModels
{
    public class BranchWiseProductVM
    {
        public Guid branchId { get; set; }
        public IEnumerable<SelectListItem>? branchList { get; set; }
        public BranchProducts? BranchProducts { get; set; }
    }
}
