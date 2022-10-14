using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Models.Models.Users
{
    public class Role : IdentityRole
    {
        public Role() : base()
        {

        }
        public bool Status { get; set; }
        //[NotMapped]
        public IEnumerable<RolePrivilege>? RolePrivileges { get; set; }
    }
}
