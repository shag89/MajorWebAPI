using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAPI.Core.DataAccess
{
    public interface IDataAccessLayer
    {
        public IRepository<TEntity> Repository<TEntity>() where TEntity : class;
    }
}
