using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Models.Models
{
    public class Clib
    {
        [Key]
        public Guid Image_Id { get; set; }
        public string? Image_url { get; set; }

        public Guid Prod_Id { get; set; }
        [ForeignKey("Prod_Id")]
        public virtual Product? Product { get; set; }
    }
}
