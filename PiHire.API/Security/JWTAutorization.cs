using Microsoft.Extensions.Configuration;
using Microsoft.Graph;
using Microsoft.IdentityModel.Tokens;
using OfficeOpenXml.FormulaParsing.Utilities;
using PiHire.BAL.IRepositories;
using PiHire.BAL.ViewModels;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using static PiHire.BAL.Common.Types.AppConstants;

namespace PiHire.API.Security
{
    public class JWTAutorization
    {
        private readonly IConfiguration Configuration;
        public JwtViewModel jwt { get; set; }
        private IAccountRepository _accountRepository;
        private IUserRespository _userRepository;
        public JWTAutorization(IConfiguration configuration, IAccountRepository accountRepository, IUserRespository userRepository)
        {
            _accountRepository = accountRepository;
            _userRepository = userRepository;
            Configuration = configuration;
            var AppSettingConfig = Configuration.GetSection("AppSettings");
            this.jwt = getJwtSettings(AppSettingConfig.GetValue<string>("JwtIssuer"), AppSettingConfig.GetValue<string>("JwtSecret"), AppSettingConfig.GetValue<string>("JwtValidityMinutes"));
        }

        //public JWTAutorization(IConfiguration configuration, IUserRespository userRepository)
        //{
        //    _userRepository = userRepository;
        //    Configuration = configuration;
        //    var AppSettingConfig = Configuration.GetSection("AppSettings");
        //    this.jwt = getJwtSettings(AppSettingConfig.GetValue<string>("JwtIssuer"), AppSettingConfig.GetValue<string>("JwtSecret"), AppSettingConfig.GetValue<string>("JwtValidityMinutes"));
        //}

        public static JwtViewModel getJwtSettings(string issuer, string secret, string validity)
        {
            return new JwtViewModel
            {
                JwtIssuer = issuer,
                JwtSecret = secret,
                JwtValidityMinutes = Convert.ToInt32(validity)
            };
        }


        public string generateToken(IEnumerable<Claim> claims)
        {
            // authentication successful so generate jwt token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = System.Text.Encoding.ASCII.GetBytes(jwt.JwtSecret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = jwt.JwtIssuer,
                IssuedAt = DateTime.UtcNow,
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(jwt.JwtValidityMinutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateJwtSecurityToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        internal ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var key = System.Text.Encoding.ASCII.GetBytes(jwt.JwtSecret);
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false, //you might want to validate the audience and issuer depending on your use case
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateLifetime = false //here we are saying that we don't care about the token's expiration date
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");

            return principal;
        }

        internal string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        internal async Task<Tuple<AuthorizationViewModel, string>> Authenticate(AccountAuthenticate model, System.Net.IPAddress clientLocation)
        {
            var user = await _accountRepository.Authenticate(model, clientLocation);
            if (user.Item1 == AuthenticateStatus.Failed)
                return Tuple.Create<AuthorizationViewModel, string>(null, "Username/password is wrong");
            if (user.Item1 == AuthenticateStatus.AlreadyLogin)
            {
                return Tuple.Create<AuthorizationViewModel, string>(null, "Already_Login~" + user.token);
            }
            var AppSettingConfig = Configuration.GetSection("AppSettings");

            var AppId = AppSettingConfig.GetValue<string>("AppId");
            user.Item2.Token = generateToken(new Claim[]
            {
                new Claim(ClaimTypes.Name, user.Item2.Id.ToString()),
                new Claim(ClaimTypes.Role, AppId + string.Empty),
                new Claim(ClaimTypes.UserData,Newtonsoft.Json.JsonConvert.SerializeObject(user.Item2))
            });
            _accountRepository.LoginTracking(user.Item2.Id, user.Item2.Name, string.Empty);
            user.Item2.RefreshToken = GenerateRefreshToken();
            return Tuple.Create<AuthorizationViewModel, string>(user.Item2, string.Empty);
        }

        /// <summary>
        /// Authenticate the user
        /// </summary>
        /// <param name="model"></param>
        /// <param name="clientLocation"></param>
        /// <returns></returns>
        internal async Task<(UserAuthorizationViewModel data, string msg, List<UserAuthorizationViewModel_UserDetail_Permission> permissions, List<UserAuthorizationViewModel_UserDetail_UserPubuList> pubuLists)> Authenticate(string username, string password, System.Net.IPAddress clientLocation)
        {
            var user = await _userRepository.authenticate(username, password);
            if (user.loginStatus == AuthenticateStatus.Failed)
                return (null, "Username/password is wrong", null, null);
            if (user.loginStatus == AuthenticateStatus.AlreadyLogin)
                return (null, "Already_Login~" + user.token, null, null);
            var AppSettingConfig = Configuration.GetSection("AppSettings");
            user.data.RefreshToken = null;
            user.data.Token = null;
            var AppId = AppSettingConfig.GetValue<string>("AppId");
            user.data.Token = "";
            UserAuthorizationViewModel userDetails = new UserAuthorizationViewModel();

            userDetails.Id = user.data.Id;
            userDetails.Name = user.data.Name ?? (user.data.UserDetails.FirstName + " " + user.data.UserDetails.LastName);
            userDetails.RefreshToken = user.data.RefreshToken;
            userDetails.EmpId = user.data.EmpId;
            userDetails.Token = user.data.Token;
            userDetails.UserTypeId = user.data.UserTypeId;
            userDetails.RoleName = user.data.RoleName;
            userDetails.UserDetails = new UserAuthorizationViewModel_UserDetail();
            userDetails.UserDetails.Id = user.data.UserDetails.Id;
            userDetails.UserDetails.FirstName = user.data.UserDetails.FirstName;
            userDetails.UserDetails.LastName = user.data.UserDetails.LastName;
            //userDetails.UserDetails.Location = user.data.UserDetails.Location;
            userDetails.UserDetails.MobileNo = user.data.UserDetails.MobileNo;
            userDetails.UserDetails.Timezone = user.data.UserDetails.Timezone;
            //userDetails.UserDetails.UserPubuList = user.data.UserDetails.UserPubuList;
            userDetails.UserDetails.UserType = user.data.UserDetails.UserType;
            userDetails.UserDetails.Designation = user.data.UserDetails.Designation;
            userDetails.UserDetails.Email = user.data.UserDetails.Email;
            //userDetails.UserDetails.Permission = user.data.UserDetails.Permission;

            userDetails.SessionTxnId = user.data.SessionTxnId;

            user.data.Token = generateToken(new Claim[]
            {
                new Claim(ClaimTypes.Name, user.data.Id.ToString()),
                new Claim(ClaimTypes.Role, AppId + string.Empty),
                new Claim(ClaimTypes.UserData,Newtonsoft.Json.JsonConvert.SerializeObject(userDetails))
            });
            user.data.RefreshToken = GenerateRefreshToken();
            return (user.data, "", user.permissions, user.pubuLists);
        }
        internal async Task<(bool IsCompleted, bool IsSuccess, UserAuthorizationViewModel data, string MicrosoftToken, string msg, List<UserAuthorizationViewModel_UserDetail_Permission> permissions, List<UserAuthorizationViewModel_UserDetail_UserPubuList> pubuLists)> authenticateMicrosoft(string RequestCode, System.Net.IPAddress clientLocation)
        {
            var user = await _userRepository.authenticateMicrosoft(RequestCode, clientLocation);
            if (user.IsCompleted == false)
                return (user.IsCompleted, user.IsSuccess, null, string.Empty, user.msg, null, null);
            else if (user.IsSuccess == false)
                return (user.IsCompleted, user.IsSuccess, null, string.Empty, user.msg, null, null);
            else
            {
                var AppSettingConfig = Configuration.GetSection("AppSettings");
                var AppId = AppSettingConfig.GetValue<string>("AppId");

                user.data.RefreshToken = null;
                {
                    user.data.Token = "";
                    UserTokenAuthorizationViewModel userDetails = new UserTokenAuthorizationViewModel();

                    userDetails.Id = user.data.Id;
                    userDetails.Name = user.data.Name ?? (user.data.UserDetails.FirstName + " " + user.data.UserDetails.LastName);
                    userDetails.RefreshToken = user.data.RefreshToken;
                    userDetails.EmpId = user.data.EmpId;
                    userDetails.Token = user.data.Token;
                    userDetails.UserTypeId = user.data.UserTypeId;
                    userDetails.RoleName = user.data.RoleName;
                    userDetails.UserDetails = new UserAuthorizationViewModel_UserDetail { };
                    userDetails.UserDetails.Id = user.data.UserDetails.Id;
                    userDetails.UserDetails.FirstName = user.data.UserDetails.FirstName;
                    userDetails.UserDetails.LastName = user.data.UserDetails.LastName;
                    //userDetails.UserDetails.Location = user.data.UserDetails.Location;
                    userDetails.UserDetails.MobileNo = user.data.UserDetails.MobileNo;
                    userDetails.UserDetails.Timezone = user.data.UserDetails.Timezone;
                    //userDetails.UserDetails.UserPubuList = user.data.UserDetails.UserPubuList;
                    userDetails.UserDetails.UserType = user.data.UserDetails.UserType;
                    userDetails.UserDetails.Designation = user.data.UserDetails.Designation;
                    userDetails.UserDetails.Email = user.data.UserDetails.Email;
                    //userDetails.UserDetails.Permission = user.data.UserDetails.Permission;

                    userDetails.SessionTxnId = user.data.SessionTxnId;

                    userDetails.setMicrosoftToken_jwt(user.data);
                    var _data = Newtonsoft.Json.JsonConvert.SerializeObject(userDetails);
                    user.data.Token = generateToken(new Claim[]
                    {
                    new Claim(ClaimTypes.Name, user.data.Id.ToString()),
                    new Claim(ClaimTypes.Role, AppId + string.Empty),
                    new Claim(ClaimTypes.UserData,_data)
                    });
                }
                user.data.RefreshToken = GenerateRefreshToken();
                return (true, true, user.data, user.data.MicrosoftToken, user.msg, user.permissions, user.pubuLists);
            }
        }


        /// <summary>
        /// Authenticate the user
        /// </summary>
        /// <param name="model"></param>
        /// <param name="clientLocation"></param>
        /// <returns></returns>
        internal async Task<Tuple<UserAuthorizationViewModel, string>> OdooAuthenticate(string username, string password, System.Net.IPAddress clientLocation)
        {
            var user = await _userRepository.OdooAuthenticate(username, password);

            if (user.loginStatus == AuthenticateStatus.Failed)
                return Tuple.Create<UserAuthorizationViewModel, string>(null, "Username/password is wrong");


            var AppSettingConfig = Configuration.GetSection("AppSettings");
            user.data.RefreshToken = null;
            user.data.Token = null;

            var AppId = AppSettingConfig.GetValue<string>("AppId");
            user.data.Token = "";

            user.data.Token = generateToken(new Claim[]
            {
                new Claim(ClaimTypes.Name, user.data.Id.ToString()),
                new Claim(ClaimTypes.Role, AppId + string.Empty),
                new Claim(ClaimTypes.UserData,Newtonsoft.Json.JsonConvert.SerializeObject(user.data))
            });
            user.data.RefreshToken = GenerateRefreshToken();
            return Tuple.Create<UserAuthorizationViewModel, string>(user.data, "");
        }


        internal async Task<Tuple<AuthorizationViewModel, string>> Authenticate(AccountAuthenticateGoogle model, System.Net.IPAddress clientLocation)
        {
            var user = await _accountRepository.AuthenticateGoogle(model, clientLocation);
            if (user.Item3 != true)
                return Tuple.Create<AuthorizationViewModel, string>(null, "Failed to authenticate from google.");
            if (user.Item1 != true)
                return Tuple.Create<AuthorizationViewModel, string>(null, "Username/password is wrong");

            var AppSettingConfig = Configuration.GetSection("AppSettings");

            var AppId = AppSettingConfig.GetValue<string>("AppId");
            user.Item2.Token = generateToken(new Claim[]
            {
                new Claim(ClaimTypes.Name, user.Item2.Id.ToString()),
                new Claim(ClaimTypes.Role, AppId + string.Empty),
                new Claim(ClaimTypes.UserData,Newtonsoft.Json.JsonConvert.SerializeObject(user.Item2))
            });
            _accountRepository.LoginTracking(user.Item2.Id, user.Item2.Name, "by Google.");
            user.Item2.RefreshToken = GenerateRefreshToken();
            return Tuple.Create<AuthorizationViewModel, string>(user.Item2, string.Empty);
        }

        internal async Task<Tuple<AuthorizationViewModel, string>> Authenticate(AccountAuthenticateFacebook model, System.Net.IPAddress clientLocation)
        {
            var user = await _accountRepository.AuthenticateFacebook(model, clientLocation);
            if (user.Item3 != true)
                return Tuple.Create<AuthorizationViewModel, string>(null, "Failed to authenticate from facebook.");
            if (user.Item1 != true)
                return Tuple.Create<AuthorizationViewModel, string>(null, "Username/password is wrong");

            var AppSettingConfig = Configuration.GetSection("AppSettings");

            var AppId = AppSettingConfig.GetValue<string>("AppId");
            user.Item2.Token = generateToken(new Claim[]
            {
                new Claim(ClaimTypes.Name, user.Item2.Id.ToString()),
                new Claim(ClaimTypes.Role, AppId + string.Empty),
                new Claim(ClaimTypes.UserData,Newtonsoft.Json.JsonConvert.SerializeObject(user.Item2))
            });
            _accountRepository.LoginTracking(user.Item2.Id, user.Item2.Name, "by Facebook.");
            user.Item2.RefreshToken = GenerateRefreshToken();
            return Tuple.Create<AuthorizationViewModel, string>(user.Item2, string.Empty);
        }



    }

    public class JwtViewModel
    {
        public string JwtIssuer { get; set; }
        public string JwtSecret { get; set; }
        public int JwtValidityMinutes { get; set; }
    }
}
