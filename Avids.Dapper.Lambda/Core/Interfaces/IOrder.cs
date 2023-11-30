using System.Linq.Expressions;
using Avids.Dapper.Lambda.Core.SetQ;

namespace Avids.Dapper.Lambda.Core.Interfaces
{
    public interface IOrder<T>
    {
        /// <summary>
        /// Order by ASC
        /// </summary>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="field"></param>
        /// <returns></returns>
        Order<T> OrderBy<TProperty>(Expression<Func<T, TProperty>> field);

        /// <summary>
        /// Order by DESC
        /// </summary>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="field"></param>
        /// <returns></returns>
        Order<T> OrderByDesc<TProperty>(Expression<Func<T, TProperty>> field);
    }
}
