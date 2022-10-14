using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Models.Models
{
    public class Category
    {
        [Key]
        public Guid Category_Id { get; set; }
        [Required]
        [DisplayName("Category Name")]
        public string? Category_Name { get; set; }


    }
}
