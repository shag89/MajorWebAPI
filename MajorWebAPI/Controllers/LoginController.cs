using MajorWebAPI.Extensions.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebAPI.Core;
using WebAPI.Core.Model;
using WebAPI.Core.Service;

namespace MajorWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly ILogger<LoginController> _logger;
        private readonly ILoginService _loginService;
        private readonly IAuthService _authService;
        public LoginController(ILogger<LoginController> logger, ILoginService loginService,IAuthService authService)
        {
            _logger = logger;
            _loginService = loginService;
            _authService = authService;
        }
        
        [HttpPost]
        [Route("UserLogin")]
        public async Task<WebAPICommonResponse> UserLogin([FromBody] LoginModel loginModel)
        {
            //_logger.LogError("{0} || hello error {1}",LoggFilesInformation.LogginController,$"{userName}-{password}");
            //var result = new WebAPICommonResponse
            //{
            //    StatusCode = 200,
            //    Body = ""

            //};
            //return Ok(result);
            var result = await _loginService.UserLogin(loginModel);
            return result;

        }

        [HttpPost("RefreshAccessToken")]
        public async Task<WebAPICommonResponse> Refresh([FromBody] TokenRequest request)
        {
            var result = await _loginService.RefreshToken(request);
            return result;
        }

    }
}
