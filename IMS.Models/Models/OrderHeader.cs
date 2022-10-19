using IMS.Models.Models.Users;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Models.Models
{
    public class OrderHeader
    {
        [Key]
        public Guid Id { get; set; }
        public string? OrderStatus { get; set; }
        public Guid BranchId { get; set; }
        public Guid StoreId { get; set; }
        [NotMapped]
        public Guid ProductId { get; set; }
        public string? Responsible_User { get; set; }
        public DateTime OrderDate { get; set; }
        [NotMapped]
        public string? Responsible_Persone_Name { get; set; }
        [NotMapped]
        public string? Product_Name { get; set; }
        [NotMapped]
        public string? Store_Name { get; set; }
        [NotMapped]
        public string? Branch_Name { get; set; }
        [NotMapped]
        public IEnumerable<SelectListItem>? Stores { get; set; }
        [NotMapped]
        public IEnumerable<SelectListItem>? Products { get; set; }
        [NotMapped]
        public IEnumerable<SelectListItem>? Branch { get; set; }
    }
}
