using System.Linq.Expressions;
using System;
using System.Threading.Tasks;

namespace Avids.Dapper.Lambda.Core.Interfaces
{
    public interface IInsert<T>
    {
        /// <summary>
        /// Create Insert statement from object
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        int Insert(T entity);

        /// <summary>
        /// Create Insert statement from object async
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<int> InsertAsync(T entity);
    }
}
