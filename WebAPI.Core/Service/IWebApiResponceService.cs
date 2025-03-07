using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPI.Core.Model;

namespace WebAPI.Core.Service
{
    public interface IWebApiResponceService
    {
        Task<WebAPICommonResponse> GenerateResponseMessage(int statusCode, string message, object data);
    }
}
