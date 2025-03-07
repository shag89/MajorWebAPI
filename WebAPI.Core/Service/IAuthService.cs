using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WebAPI.Core.Entity;
using WebAPI.Core.Model;

namespace WebAPI.Core.Service
{
    public interface IAuthService
    {
        Task<IActionResult> Login(LoginModel loginModel);
        Task<string> GenerateToken(User user);
        Task<string> GenerateRefreshToken(int UserId);
        Task<ClaimsPrincipal> GetPrincipalFromExpiredToken(string token);
    }
}
