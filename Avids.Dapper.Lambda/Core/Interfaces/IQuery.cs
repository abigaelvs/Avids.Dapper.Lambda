using System.Collections.Generic;
using System.Threading.Tasks;

using Avids.Dapper.Lambda.Model;

namespace Avids.Dapper.Lambda.Core.Interfaces
{
    public interface IQuery<T>
    {
        /// <summary>
        /// Get First row from query
        /// </summary>
        /// <returns></returns>
        T Get();

        /// <summary>
        /// Get First row from query async
        /// </summary>
        /// <returns></returns>

        Task<T> GetAsync();

        /// <summary>
        /// Execute sql statement and convert result to list
        /// </summary>
        /// <returns></returns>

        IEnumerable<T> ToList();

        /// <summary>
        /// Execute sql statement and convert result to list async
        /// </summary>
        /// <returns></returns>

        Task<IEnumerable<T>> ToListAsync();

        /// <summary>
        /// Execute sql statement and convert result to pagination
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>

        PageList<T> PageList(int pageIndex, int pageSize);
    }
}
