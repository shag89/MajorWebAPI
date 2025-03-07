using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPI.Core.Entity;
using WebAPI.Core.Model;

namespace WebAPI.Core.EntitiManagmentService
{
    public interface IUserManagementService
    {
        Task<User> GetUserbyUserName(string name);
        Task<User> GetUserbyId(string id);
        Task<UserModel> GetUserRefreshTokenId(string id);

    }
}
