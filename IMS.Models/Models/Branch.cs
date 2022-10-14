using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Models.Models
{
    public class Branch
    {
        [Key]
        public Guid BranchId { get; set; }
        [Required]
        [DisplayName("Branch Name")]
        public string? BranchName { get; set; }
        [Required]
        [DisplayName("Branch Location")]
        public string? BranchLoc { get; set; }
    }
}
