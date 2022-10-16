using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Models.Models
{
    public class BranchProducts
    {
        [Key]
        public Guid Id { get; set; }
        public Guid BranchId { get; set; }
        public Guid ProductId { get; set; }
        public Guid StoreId { get; set; }
        public int Quantity { get; set; }
        public Guid ResponsiblePerson { get; set; }
        public DateTime OrderDate { get; set; }
    }
}
