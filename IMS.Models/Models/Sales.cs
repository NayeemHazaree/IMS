using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Models.Models
{
    public class Sales
    {
        [Key]
        public Guid Sales_Id { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        [DataType(DataType.Date)]
        public DateTime Sale_Date { get; set; }
        public string? Responsible_User { get; set; }
        public bool IsCoupon { get; set; }
        public string? CoupnName { get; set; }
        public string? Invoice { get; set; }
        [NotMapped]
        public string? Product_Name { get; set; }
        
    }
}
