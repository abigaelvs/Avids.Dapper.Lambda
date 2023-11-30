using System.Linq.Expressions;

namespace Avids.Dapper.Lambda.Core.Interfaces
{
    public interface IAggregation<T>
    {
        /// <summary>
        /// Count Row
        /// </summary>
        /// <returns></returns>
        int Count();

        /// <summary>
        /// Check if value exists
        /// </summary>
        /// <returns></returns>
        bool Exists();

        /// <summary>
        /// Sum row
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="sumExpression"></param>
        /// <returns></returns>
        TResult Sum<TResult>(Expression<Func<T, TResult>> sumExpression);
    }
}
