using System;
using System.Linq.Expressions;

namespace Avids.Dapper.Lambda.Core.Interfaces
{
    public interface ICommandSet<T>
    {
        /// <summary>
        /// Create where statement for Insert, Update, and Delete
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        ICommand<T> Where(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Create If not exists statement for Insert, Update, and Delete
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        IInsert<T> IfNotExists(Expression<Func<T, bool>> predicate);

        //void BatchInsert(IEnumerable<T> entities, int timeout = 120);
    }
}
