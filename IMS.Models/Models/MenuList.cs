using IMS.Models.Models.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Models.Models
{
    public class MenuList
    {
        [Key]
        public int Menu_Id { get; set; }
        public string? Area { get; set; }
        public string? Controller_Name { get; set; }
        public string? Action_Name { get; set; }
        [NotMapped]
        public IEnumerable<RolePrivilege>? RolePrivileges { get; set; }
        [NotMapped]
        public IEnumerable<UserPrivilege>? UserPrivileges { get; set; }
        [NotMapped]
        public ApplicationUser? ApplicationUser { get; set; }
        [NotMapped]
        public bool Status { get; set; }
        [NotMapped]
        public IEnumerable<RolePrivilege>? Exist_Role { get; set; }
        [NotMapped]
        public IEnumerable<UserPrivilege>? Exist_User { get; set; }
        [NotMapped]
        public IEnumerable<MenuList>? PageList { get; set; }
    }
}
