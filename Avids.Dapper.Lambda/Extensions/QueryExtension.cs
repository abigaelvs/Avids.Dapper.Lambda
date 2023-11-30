using System.Data;
using System.Linq.Expressions;

using Dapper;

using Avids.Dapper.Lambda.Core.SetQ;

namespace Avids.Dapper.Lambda.PostgreSql.Extension
{
    public static class QueryExtension
    {
        /// <summary>
        /// Dapper Query method extension for UpdateSelect
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="updator"></param>
        /// <returns></returns>
        public static List<T> UpdateSelect<T>(this Query<T> query, Expression<Func<T, T>> updator)
        {
            SqlProvider sqlProvider = query.SqlProvider;
            IDbConnection dbCon = query.DbCon;
            IDbTransaction dbTransaction = query.DbTransaction;
            sqlProvider.FormatUpdateSelect(updator);

            return dbCon.Query<T>(sqlProvider.SqlString, sqlProvider.Params, dbTransaction).ToList();
        }

        /// <summary>
        /// async Dapper Query method extension for UpdateSelect
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="updator"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<T>> UpdateSelectAsync<T>(this Query<T> query, Expression<Func<T, T>> updator)
        {
            SqlProvider sqlProvider = query.SqlProvider;
            IDbConnection dbCon = query.DbCon;
            IDbTransaction dbTransaction = query.DbTransaction;
            sqlProvider.FormatUpdateSelect(updator);

            return await dbCon.QueryAsync<T>(sqlProvider.SqlString, sqlProvider.Params, dbTransaction);
        }
    }
}
