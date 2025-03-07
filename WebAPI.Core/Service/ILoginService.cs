using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPI.Core.Model;

namespace WebAPI.Core.Service
{
    public interface ILoginService
    {
        Task<WebAPICommonResponse> UserLogin(LoginModel loginModel);
        Task<WebAPICommonResponse> RefreshToken(TokenRequest loginModel);

    }
}
