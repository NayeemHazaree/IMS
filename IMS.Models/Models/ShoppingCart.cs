using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Models.Models
{
    public class ShoppingCart
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
