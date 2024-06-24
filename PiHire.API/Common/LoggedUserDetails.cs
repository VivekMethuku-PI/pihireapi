using Microsoft.AspNetCore.Http;
using PiHire.BAL.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using static PiHire.BAL.Common.Types.AppConstants;

namespace PiHire.API.Common
{
    public interface ILoggedUserDetails
    {
        UserAuthorizationViewModel Usr { get; }
    }

    public class LoggedUserDetails : ILoggedUserDetails
    {
        private IHttpContextAccessor context;
        private HttpContext _httpContext;
        private HttpContext httpContext => _httpContext ?? (_httpContext = context.HttpContext);
        public LoggedUserDetails(IHttpContextAccessor context)
        {
            this.context = context;
        }
        public LoggedUserDetails(HttpContext httpContext)
        {
            this._httpContext = httpContext;
        }
        #region App User
        private UserAuthorizationViewModel _usr;
        public UserAuthorizationViewModel Usr
        {
            get
            {
                if (_usr == null)
                {

                    var jsonData = httpContext.User.Claims.Where(da => da.Type == ClaimTypes.UserData).Select(da => da.Value).FirstOrDefault();
                    if (jsonData != null)
                    {
                        var __usr = Newtonsoft.Json.JsonConvert.DeserializeObject<UserAuthorizationViewModel>(jsonData);
                        _usr = __usr;
                    }
                }
                return _usr;
            }
        }
        //public int AccountId => Usr.AccountId;
        //public int UserId => Usr.UserId;
        //public IList<byte> Experiences => Usr.ExperienceIds;
        #endregion
    }
}
