using Dapper;
using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPI.Core.DataAccess;

namespace WebAPI.DataAccess
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private readonly DapperContext _dapperContext;
        public Repository(DapperContext dapperContext)
        {
            _dapperContext = dapperContext;
        }
        public async Task<TEntity> GetAsync(string id)
        {
            using (var connection = _dapperContext.GetConnectiion())
            {
                connection.Open();
                return await connection.GetAsync<TEntity>(id);
            }
        }

        public async Task<long> InsertAsync(TEntity Entity)
        {
            try
            {
                long id;
                using(var connection = _dapperContext.GetConnectiion())
                {
                    connection.Open();
                    id = await connection.InsertAsync(Entity);
                }
                return id;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
        public async Task<IEnumerable<TEntity>> GetUsersAsync()
        {
            try
            {
                using (var connection = _dapperContext.GetConnectiion())
                {
                    return await connection.GetAllAsync<TEntity>();
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public IEnumerable<TEntity> GetEntitiesBySP(string storedProcedureName, Dictionary<string, Tuple<string, DbType, ParameterDirection>> parameters)
        {
            try
            {
                DynamicParameters dynamicParameters = new DynamicParameters();

                foreach (KeyValuePair<string, Tuple<string, DbType, ParameterDirection>> entry in parameters)
                {
                    if (entry.Value.Item2 == DbType.Guid)
                    {
                        Guid guid = new Guid(entry.Value.Item1);
                        dynamicParameters.Add(entry.Key, guid, DbType.Guid, entry.Value.Item3);
                    }
                    else
                    {
                        dynamicParameters.Add(entry.Key, entry.Value.Item1, entry.Value.Item2, entry.Value.Item3);
                    }
                }

                using (var connection = _dapperContext.GetConnectiion())
                {
                    connection.Open();

                    IEnumerable<TEntity> result = connection.Query<TEntity>(
                                storedProcedureName,
                                param: dynamicParameters,
                                commandType: CommandType.StoredProcedure);

                    return result;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<TEntity> GetEntityBySPAsync(string storedProcedureName, Dictionary<string, Tuple<string, DbType, ParameterDirection>> parameters)
        {
            DynamicParameters dynamicParameters = new DynamicParameters();

            foreach (KeyValuePair<string, Tuple<string, DbType, ParameterDirection>> entry in parameters)
            {
                if (entry.Value.Item2 == DbType.Guid)
                {
                    Guid guid = new Guid(entry.Value.Item1);
                    dynamicParameters.Add(entry.Key, guid, DbType.Guid, entry.Value.Item3);
                }
                else
                {
                    dynamicParameters.Add(entry.Key, entry.Value.Item1, entry.Value.Item2, entry.Value.Item3);
                }
            }

            using (var connection = _dapperContext.GetConnectiion())
            {
                connection.Open();

                var result = await connection.QueryAsync<TEntity>(
                  storedProcedureName,
                  param: dynamicParameters,
                  commandType: CommandType.StoredProcedure);

                return result.FirstOrDefault();
            }
        }

    }
}
