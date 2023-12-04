using System;
using System.Linq.Expressions;

using Avids.Dapper.Lambda.Core.SetQ;

namespace Avids.Dapper.Lambda.Core.Interfaces
{
    public interface IQuerySet<T>
    {
        /// <summary>
        /// Create where statement
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        QuerySet<T> Where(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Create sql statement NO LOCK
        /// </summary>
        /// <returns></returns>
        QuerySet<T> WithNoLock();
    }
}
