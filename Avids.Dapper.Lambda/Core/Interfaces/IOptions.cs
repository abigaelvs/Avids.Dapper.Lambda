using System;
using System.Linq.Expressions;

using Avids.Dapper.Lambda.Core.SetQ;

namespace Avids.Dapper.Lambda.Core.Interfaces
{
    public interface IOption<T>
    {

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
