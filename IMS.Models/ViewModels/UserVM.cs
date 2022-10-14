using IMS.Models.Models.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Models.ViewModels
{
    public class UserVM
    {
        public IEnumerable<ApplicationUser>? ApplicationUsers { get; set; }

        public IdentityUser? CurrentUser { get; set; }

        public IEnumerable<IdentityRole>? IdentityRoles { get; set; }

        public IEnumerable <IdentityUserRole<string>>? UserRole { get; set; }

        public IEnumerable<SelectListItem>? RoleForUser { get; set; }

        public ApplicationUser? applicationUser { get; set; }
        
    }
}
