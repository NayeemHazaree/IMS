using IMS.Models.Models.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Models.Models
{
    public class RolePrivilege
    {
        public int MenuId { get; set; }
        public MenuList? MenuList { get; set; }
        public string? RoleId { get; set; }
        public Role? Role { get; set; }

        [NotMapped]
        public IEnumerable<SelectListItem>? RoleListFromDb { get; set; }
        [NotMapped]
        public string? Set_Permission_Role_Id { get; set; }
        
        
    }
}
