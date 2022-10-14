using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Models.Models.Users
{
    public class ApplicationUser :IdentityUser
    {
        [Display(Name ="Full Name")]
        public string? Full_Name { get; set; }

        //public string? RoleId { get; set; }]\
        [NotMapped]
        public IEnumerable<string>? RoleId { get; set; }
        [NotMapped]
        public IEnumerable<string>? Role { get; set; }
        [NotMapped]
        public IEnumerable<string>? RoleList { get; set; }
        //for search
        [NotMapped]
        public string? S_Role { get; set; }
        public IEnumerable<UserPrivilege>? UserPrivileges { get; set; }
    }
}
