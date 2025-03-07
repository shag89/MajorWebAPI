using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPI.Core.Service;

namespace WebAPI.Core.Model
{
    public class WebAPICommonResponse : IWebApiResponceService
    {
        public int StatusCode { get; set; }
        public object Body { get; set; }
        public string Message { get; set; }

        public async Task<WebAPICommonResponse> GenerateResponseMessage(int statusCode, string message, object data)
        {
            this.StatusCode = statusCode;
            this.Message = message;
            this.Body = data;

            return this;
        }
    }
}
