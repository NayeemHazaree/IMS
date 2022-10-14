using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Models.Models
{
    public class Supplier
    {
        [Key]
        public Guid SupplierId { get; set; }
        [Display(Name ="Supplier Name")]
        public string? SupplerName { get; set; }
        [Display(Name = "Email")]
        public string? SupplierEmail { get; set; }
        [Display(Name = "Phone Number")]
        public string? SupplierPhone { get; set; }
        [Display(Name = "Address")]
        public string? SupplierAddress { get; set; }
        [Display(Name = "Store Name")]
        public string? SupplierStoreName { get; set; }
        public bool Status { get; set; }
    }
}
