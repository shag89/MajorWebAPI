using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WebAPI.Core;
using WebAPI.Core.DataAccess;
using WebAPI.Core.EntitiManagmentService;
using WebAPI.Core.Entity;
using WebAPI.Core.Model;
using WebAPI.Core.Service;

namespace WebAPI.Services
{
    public class LoginService : ILoginService
    {
        private readonly IWebApiResponceService _apiresponce;
        private readonly IDataAccessLayer _dataAccessLayer;
        private readonly IAuthService _authService;
        private readonly IUserManagementService _userManagementService;
        private readonly ILogger<LoginService> _logger;
        public LoginService(IDataAccessLayer dataAccessLayer,IWebApiResponceService webApiResponceService, IAuthService authService, IUserManagementService userManagementService,ILogger<LoginService> logger)
        {
            _apiresponce = webApiResponceService;
            _dataAccessLayer = dataAccessLayer;
            _authService = authService;
            _userManagementService = userManagementService;
            _logger = logger;
        }


        public async Task<WebAPICommonResponse> UserLogin(LoginModel loginModel)
        {
            try
            {
                _logger.LogInformation("{0} || LoginService.UserLogin() method started | User:{1}", LoggFilesInformation.LogginInformation, loginModel.UserName);
                User user = await _userManagementService.GetUserbyUserName(loginModel.UserName);

                if(user is null)
                {
                    return await _apiresponce.GenerateResponseMessage((int)WebResponseCode.Notfound, "User does not exist", null);
                }

                if (user.UserName != loginModel.UserName)
                {
                    return await _apiresponce.GenerateResponseMessage((int)WebResponseCode.Unathorized, "Invalid credentials", null);
                    //return Unauthorized("Invalid credentials 1");
                }

                //var saltedPassword = loginModel.Password + user.Salt;

                //var result = _passwordHasher.VerifyHashedPassword(user, user.Password, saltedPassword);

                if (loginModel.Password != user.Password)
                {
                    return await _apiresponce.GenerateResponseMessage((int)WebResponseCode.Unathorized, "Invalid credentials", null);
                }

                // Generate token
                var token = await _authService.GenerateToken(user);
                var newRefreshToken = await _authService.GenerateRefreshToken(user.Id);

                // Return the token
                var responce = new AuthResponse
                {
                    UserId = user.Id.ToString(),
                    Token = token,
                    RefreshToken = newRefreshToken
                };
                return await _apiresponce.GenerateResponseMessage((int)WebResponseCode.Success, "Successfully Loged", responce);
            }
            catch (Exception ex)
            {
                //return await _apiresponce.GenerateResponseMessage((int)WebResponseCode.Exception, "Internal Error", null);
                _logger.LogError("{0} || Error occured in LoginService.UserLogin() | User={1} | Exception msg \n {2}", LoggFilesInformation.Loggerror, loginModel.UserName, ex);
                throw ex;
            }
        }

        public async Task<WebAPICommonResponse> RefreshToken(TokenRequest loginModel)
        {
            try
            {
                _logger.LogInformation("{0} || LoginService.RefreshToken() method started ", LoggFilesInformation.LogginInformation);
                var principal = await _authService.GetPrincipalFromExpiredToken(loginModel.AccessToken);
                var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                var user = await _userManagementService.GetUserRefreshTokenId(userId);
                var userTwo = await _userManagementService.GetUserbyId(userId);

                if (user == null || user.RefreshToken != loginModel.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
                {
                    return await _apiresponce.GenerateResponseMessage((int)WebResponseCode.Unathorized, "Invalid Token", null);
                }

                var newAccessToken = await _authService.GenerateToken(userTwo);
                var newRefreshToken = await _authService.GenerateRefreshToken(user.UserId);

                var responce = new AuthResponse
                {
                    UserId = user.UserId.ToString(),
                    Token = newAccessToken,
                    RefreshToken = newRefreshToken
                };
                //_userService.UpdateUser(user);

                return await _apiresponce.GenerateResponseMessage((int)WebResponseCode.Success, "Successfully Generate Refresh Token", responce);
            }
            catch (Exception ex)
            {
                _logger.LogError("{0} || Error occured in LoginService.RefreshToken() | Exception msg \n {2}", LoggFilesInformation.Loggerror, ex);
                throw ex;
            }
        }
    }
}
