using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Models.Models
{
    public class Product
    {
        [Key]
        public Guid Product_Id { get; set; }
        [Required]
        [DisplayName("Name")]
        public string? Product_Name { get; set; }
        [Required]
        [DisplayName("Description")]
        public string? Product_Description { get; set; }
        [Required]
        [DisplayName("Price")]
        public int Product_Price { get; set; }
        public int Quantity { get; set; }
        [DisplayName("In Stock")]
        public bool inStock { get; set; }
        [Required]
        [DisplayName("Category")]
        public Guid Parent_Cat_Id { get; set; }
        [ForeignKey("Parent_Cat_Id")]
        public virtual Category? Category { get; set; }
        [DisplayName("Brand")]
        public Guid Parent_brand_Id { get; set; }
        [ForeignKey("Parent_brand_Id")]
        public virtual Brand? Brand { get; set; }

        //for HomeVM show img
        [NotMapped]
        public string? ImgPath { get; set; }
        [NotMapped]
        public int tempQty { get; set; }


    }
}
