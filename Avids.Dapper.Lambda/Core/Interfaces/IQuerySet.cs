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
        /// Inner Join
        /// </summary>
        /// <typeparam name="I"></typeparam>
        /// <param name="on"></param>
        /// <returns></returns>
        QuerySet<T> InnerJoin<TJoinTable>(Expression<Func<TJoinTable, T, bool>> on);

        /// <summary>
        /// Left Join
        /// </summary>
        /// <typeparam name="I"></typeparam>
        /// <param name="on"></param>
        /// <returns></returns>
        QuerySet<T> LeftJoin<TJoinTable>(Expression<Func<TJoinTable, T, bool>> on);

        /// <summary>
        /// Right Join
        /// </summary>
        /// <typeparam name="I"></typeparam>
        /// <param name="on"></param>
        /// <returns></returns>
        QuerySet<T> RightJoin<TJoinTable>(Expression<Func<TJoinTable, T, bool>> on);

        /// <summary>
        /// Full Join
        /// </summary>
        /// <typeparam name="I"></typeparam>
        /// <param name="on"></param>
        /// <returns></returns>
        QuerySet<T> FullJoin<TJoinTable>(Expression<Func<TJoinTable, T, bool>> on);

        /// <summary>
        /// Create sql statement NO LOCK
        /// </summary>
        /// <returns></returns>
        QuerySet<T> WithNoLock();
    }
}
