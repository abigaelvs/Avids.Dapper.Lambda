using System;
using System.Linq.Expressions;

using Avids.Dapper.Lambda.Core.SetC;

namespace Avids.Dapper.Lambda.Core.Interfaces
{
    public interface ICommandSet<T>
    {
        /// <summary>
        /// Create where statement for Insert, Update, and Delete
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        Command<T> Where(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Create If not exists statement for Insert, Update, and Delete
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        Command<T> IfNotExists(Expression<Func<T, bool>> predicate);

        //void BatchInsert(IEnumerable<T> entities, int timeout = 120);
    }
}
