using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Models.Models
{
    public class Brand
    {
        [Key]
        public Guid Brand_Id { get; set; }
        [Required]
        [DisplayName("Brand Name")]
        public string? Brand_Name { get; set; }
        [DisplayName("Status")]
        public bool Brand_Status { get; set; }
    }
}
