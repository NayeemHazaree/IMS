using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Models.Models
{
    public class CouponCodes
    {
        public CouponCodes()
        {
            CouponDate = DateTime.Now;
        }
        [Key]
        public Guid Coupon_Id { get; set; }
        [Display(Name ="Coupon Code")]
        public string? CouponCode { get; set; }
        [Display(Name ="Coupon Parcentange")]
        public double? CouponParcentange { get; set; }
        [Display(Name ="Till Date")]
        public DateTime CouponDate { get; set; }
    }
}
