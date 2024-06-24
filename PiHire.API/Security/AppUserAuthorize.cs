using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using PiHire.BAL.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PiHire.API.Security
{
    public class AppUserAuthorize : Microsoft.AspNetCore.Mvc.TypeFilterAttribute
    {
        public AppUserAuthorize(string moduleName, string Permission) : base(typeof(AppUserAuthorizeFilter))
        {
            Arguments = new object[] { moduleName, Permission };
        }
    }
    public class AppUserAuthorizeFilter : IAuthorizationFilter
    {
        readonly string usrType, per;
        public AppUserAuthorizeFilter(string UserType, string Permission)
        {
            usrType = UserType;
            per = Permission;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (!context.HttpContext.User.Identity.IsAuthenticated)
            {
                context.Result = new Microsoft.AspNetCore.Mvc.UnauthorizedResult();
                return;
            }
            var usrClaims = context.HttpContext.User.Claims.Where(da => da.Type == ClaimTypes.UserData).Select(da => da.Value).FirstOrDefault();
            if (!CheckAccess(usrClaims))
            {
                context.Result = new Microsoft.AspNetCore.Mvc.ForbidResult();
            }
        }
        private bool CheckAccess(string usrClaim)
        {
            if (string.IsNullOrEmpty(usrClaim)) return false;
            var data = Newtonsoft.Json.JsonConvert.DeserializeObject<AppUserAuthorizationViewModel>(usrClaim);
            if (data == null) return false;
            return true; //data.UserTypeId + "" == usrType;
        }
    }
}
