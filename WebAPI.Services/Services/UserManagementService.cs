using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using WebAPI.Core.DataAccess;
using WebAPI.Core.EntitiManagmentService;
using WebAPI.Core.Entity;
using WebAPI.Core.Model;

namespace WebAPI.Services.Services
{
    public class UserManagementService : IUserManagementService
    {
        private readonly IRepository<User> _userRepository;
        private readonly IDataAccessLayer _dataAccessLayer;
        public UserManagementService(IRepository<User> repository, IDataAccessLayer dataAccessLayer)
        {
            _userRepository = repository;
            _dataAccessLayer = dataAccessLayer;
        }

        public async Task<User> GetUserbyId(string id)
        {
            try
            {
                var user = await _dataAccessLayer.Repository<User>().GetAsync(id);
                return user;
            }
            catch (Exception ex)
            {
                return null;
                throw ex;
            }
        }

        public async Task<User> GetUserbyUserName(string name)
        {
            try
            {
                var parameters = new Dictionary<string, Tuple<string, DbType, ParameterDirection>>
                {
                   { "UserName", Tuple.Create(name, DbType.String, ParameterDirection.Input) }
                };
                var user = await _userRepository.GetEntityBySPAsync("[dbo].[GetUserbyUsername]", parameters);

                return user;
            }
            catch (Exception ex)
            {
                return null;
                throw ex;
            }
        }

        public async Task<UserModel> GetUserRefreshTokenId(string id)
        {
            try
            {
                var parameters = new Dictionary<string, Tuple<string, DbType, ParameterDirection>>
                {
                   { "UserId", Tuple.Create(id.ToString(), DbType.String, ParameterDirection.Input) }
                };
                var user = await _dataAccessLayer.Repository<UserModel>().GetEntityBySPAsync("[dbo].[GetUserRefreshTokenbyUserId]", parameters);

                return user;
            }
            catch (Exception ex)
            {
                return null;
                throw ex;
            }
        }
    }
}
