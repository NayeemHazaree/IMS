using IMS.DataAccess.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Models.Models.Authorize
{
    public class AccessCheckerHandler : AuthorizationHandler<AccessCheckerHandler>, IAuthorizationRequirement
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ApplicationDbContext _db;

        public AccessCheckerHandler(ApplicationDbContext db)
        {
            _httpContextAccessor = new HttpContextAccessor();
            _db = db;
        }

        protected override async Task<Task> HandleRequirementAsync(AuthorizationHandlerContext context, AccessCheckerHandler requirement)
        {
            var user = context.User;
            if (user.Identity.IsAuthenticated)
            {
                string userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Name).Value;
                var userObj = await _db.Users.FirstOrDefaultAsync(x => x.UserName == userId);
                var userRole = await _db.UserRoles.Where(x => x.UserId == userObj.Id).ToListAsync();
                List<int> PermisionList = new List<int>();
                if (userRole.Count() > 0)
                {
                    foreach (var role in userRole)
                    {
                        var rolePermision = await _db.RolePrivileges.Where(x => x.RoleId == role.RoleId).ToListAsync();
                        foreach (var rolePri in rolePermision)
                        {
                            PermisionList.Add(rolePri.MenuId);
                        }
                    }
                }
                var userPermision = await _db.UserPrivileges.Where(x => x.UserId == userObj.Id).ToListAsync();
                foreach (var userPri in userPermision)
                {
                    PermisionList.Add(userPri.MenuId);
                }
                var userPermisson = PermisionList.Distinct().ToList();
                var request = _httpContextAccessor.HttpContext.Request.Path;
                var rq_path = request.Value;
                string[] path = request.Value.Split('/');
                //var menuObj = await _db.MenuList.FirstOrDefaultAsync(x=>x.Controller_Name == path[1]);
                var menuObj = await _db.MenuList.FirstOrDefaultAsync(x => x.Area == path[1] &&
                                     x.Controller_Name == path[2]);
                if (menuObj != null)
                {
                    if (userPermisson.Contains(menuObj.Menu_Id))
                    {
                        context.Succeed(requirement);
                        return Task.CompletedTask;
                    }
                }
                //context.Succeed(requirement);
                return Task.CompletedTask;
            }
            return Task.CompletedTask;
        }
    }
}
