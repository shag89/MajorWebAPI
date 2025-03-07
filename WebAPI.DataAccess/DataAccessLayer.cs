using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPI.Core.DataAccess;

namespace WebAPI.DataAccess
{
    public class DataAccessLayer : IDataAccessLayer
    {
        // public readonly string _connectionstring;
        private readonly DapperContext _dapperContext;
        protected Dictionary<string, dynamic> _Repositories;
        //public DataAccessLayer(string connectionstring)
        public DataAccessLayer(DapperContext dataAccessLayer)
        {
            //_connectionstring = connectionstring;
            _dapperContext = dataAccessLayer;
            _Repositories = new Dictionary<string, dynamic>();
        }

        public IRepository<TEntity> Repository<TEntity>() where TEntity : class
        {
            if (_Repositories == null)
            {
                _Repositories = new Dictionary<string, dynamic>();
            }

            var type = typeof(TEntity).Name;

            if (_Repositories.ContainsKey(type))
            {
                return (IRepository<TEntity>)_Repositories[type];
            }

            var repositoryType = typeof(Repository<>);
            _Repositories.Add(type, Activator.CreateInstance(repositoryType.MakeGenericType(typeof(TEntity)), _dapperContext));
            return _Repositories[type];
        }
    }
}
