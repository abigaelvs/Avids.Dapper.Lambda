using System;
using System.Data;

using Avids.Dapper.Lambda.Core.SetC;
using Avids.Dapper.Lambda.Core.SetQ;
using Avids.Dapper.Lambda.Model;
using Avids.Dapper.Lambda.PostgreSql;
using Avids.Dapper.Lambda.MsSql;
using Avids.Dapper.Lambda.MySql;
using Avids.Dapper.Lambda.Exception;

namespace Avids.Dapper.Lambda
{
    /// <summary>
    /// Method extension for Dapper
    /// </summary>
    public static class Database
    {
        /// <summary>
        /// Query extension method with DbConnection only
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sqlConnection"></param>
        /// <returns></returns>
        public static QuerySet<T> QuerySet<T>(this IDbConnection sqlConnection)
        {
            SqlProvider provider = GetSqlProvider(sqlConnection);
            return new QuerySet<T>(sqlConnection, provider);
        }

        /// <summary>
        /// Query extension method with DbConnection and DbTransaction
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sqlConnection"></param>
        /// <param name="dbTransaction"></param>
        /// <returns></returns>
        public static QuerySet<T> QuerySet<T>(this IDbConnection sqlConnection, IDbTransaction dbTransaction)
        {
            SqlProvider provider = GetSqlProvider(sqlConnection);
            return new QuerySet<T>(sqlConnection, provider, dbTransaction);
        }

        /// <summary>
        /// Command Extension with DbConnection only
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sqlConnection"></param>
        /// <returns></returns>
        public static CommandSet<T> CommandSet<T>(this IDbConnection sqlConnection)
        {
            SqlProvider provider = GetSqlProvider(sqlConnection);
            return new CommandSet<T>(sqlConnection, provider);
        }

        /// <summary>
        /// Command extension method with DbConnection and dbTransaction
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sqlConnection"></param>
        /// <param name="dbTransaction"></param>
        /// <returns></returns>
        public static CommandSet<T> CommandSet<T>(this IDbConnection sqlConnection, IDbTransaction dbTransaction)
        {
            SqlProvider provider = GetSqlProvider(sqlConnection);
            return new CommandSet<T>(sqlConnection, provider, dbTransaction);
        }

        /// <summary>
        /// Transaction Extension method for Begin Transaction and Commit on the go
        /// </summary>
        /// <param name="sqlConnection"></param>
        /// <param name="action"></param>
        /// <param name="exAction"></param>
        public static void Transaction(this IDbConnection sqlConnection, Action<TransContext> action,
            Action<System.Exception> exAction = null)
        {
            if (sqlConnection.State == ConnectionState.Closed)
                sqlConnection.Open();

            IDbTransaction transaction = sqlConnection.BeginTransaction();
            try
            {
                SqlProvider provider = GetSqlProvider(sqlConnection);
                action(new TransContext(sqlConnection, transaction, provider));
                transaction.Commit();
            }
            catch (System.Exception ex)
            {
                if (exAction != null)
                    exAction(ex);
                else
                {
                    transaction.Rollback();
                }
            }
            finally
            {
                sqlConnection.Close();
            }
        }

        /// <summary>
        /// Get SQL Provider based on IDbConnection instance
        /// </summary>
        /// <param name="sqlConnection"></param>
        /// <returns></returns>
        public static SqlProvider GetSqlProvider(IDbConnection sqlConnection)
        {
            switch (sqlConnection.GetType().Name)
            {
                case "NpgsqlConnection":
                    return new PostgreSqlProvider();
                case "SqlConnection":
                    return new MsSqlProvider();
                case "MySqlConnection":
                    return new MySqlProvider();
                default:
                    throw new DapperExtensionException("Your database provider currently not supported");
            }
        }
    }
}
