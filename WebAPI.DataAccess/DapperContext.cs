using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAPI.DataAccess
{
    public class DapperContext
    {
        private readonly string _dbConnectionString;
        public DapperContext(string dbConnectionString)
        {
            _dbConnectionString = dbConnectionString;
        }
        public IDbConnection GetConnectiion() => new SqlConnection(_dbConnectionString);
    }
}
