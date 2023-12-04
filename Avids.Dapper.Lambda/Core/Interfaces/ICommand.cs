using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Avids.Dapper.Lambda.Core.Interfaces
{
    public interface ICommand<T>
    {
        /// <summary>
        /// Create Update statement from object
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        int Update(T entity);

        /// <summary>
        /// Create Update statement from object async
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<int> UpdateAsync(T entity);

        /// <summary>
        /// Create Update statement from expression
        /// </summary>
        /// <param name="updateExpression"></param>
        /// <returns></returns>
        int Update(Expression<Func<T, T>> updateExpression);

        /// <summary>
        /// Create Update statement from expression async
        /// </summary>
        /// <param name="updateExpression"></param>
        /// <returns></returns>
        Task<int> UpdateAsync(Expression<Func<T, T>> updateExpression);

        /// <summary>
        /// Create Delete statement
        /// </summary>
        /// <returns></returns>
        int Delete();

        /// <summary>
        /// Create Delete statement async
        /// </summary>
        /// <returns></returns>
        Task<int> DeleteAsync();
    }
}
