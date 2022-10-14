using IMS.Models.Models.Users;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Models.Models
{
    public class UserPrivilege
    {
        public int MenuId { get; set; }
        public MenuList? MenuList { get; set; }
        public string? UserId { get; set; }
        public ApplicationUser? ApplicationUser { get; set; }

        [NotMapped]
        public IEnumerable<SelectListItem>? UserListDB { get; set; }
        [NotMapped]
        public string? Set_User_Permission { get; set; }

    }
}
