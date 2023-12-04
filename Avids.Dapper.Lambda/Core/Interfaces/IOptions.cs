using System;
using System.Linq.Expressions;

using Avids.Dapper.Lambda.Core.SetQ;

namespace Avids.Dapper.Lambda.Core.Interfaces
{
    public interface IOption<T>
    {
        /// <summary>
        /// Select certain column
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="selector"></param>
        /// <returns></returns>
        //Query<TResult> Select<TResult>(Expression<Func<T, TResult>> selector);
        Option<T> Select<S, E>(Expression<Func<S, E>> selector);

        /// <summary>
        /// Limit result
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        Option<T> Limit(int num);

        /// <summary>
        /// Offset result
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        Option<T> Offset(int num);
    }
}
