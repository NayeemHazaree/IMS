using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Models.Models
{
    public class OrderDetails
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public Guid OrderDetailsId { get; set; }
        [ForeignKey("OrderDetailsId")]
        public OrderHeader? OrderHeader { get; set; }
        //public Guid BranchId { get; set; }
        //public Guid StoreId { get; set; }
        public Guid ProductId { get; set; }
        public string? Invoice { get; set; }
        public int Quantity { get; set; }
        //[DataType(DataType.Date)]
        //public DateTime OrderDate { get; set; }
        //public string? Responsible_User { get; set; }
        //public string? OrderStatus { get; set; }
    }
}
