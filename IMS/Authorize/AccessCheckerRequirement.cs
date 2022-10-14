using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Models.Models.Authorize
{
    public class AccessCheckerRequirement : IAuthorizationRequirement
    {
        public AccessCheckerRequirement(string areaName, string controllerName, string actionName)
        {
            ActionName = actionName;
            ControllerName = controllerName;
            AreaName = areaName;
        }

        public string AreaName { get; set; }
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
    }
}
