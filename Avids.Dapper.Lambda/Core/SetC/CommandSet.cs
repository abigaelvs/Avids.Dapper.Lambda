using System;
using System.Data;
using System.Linq.Expressions;

using Avids.Dapper.Lambda.Core.Interfaces;
using Avids.Dapper.Lambda.Helper;
using Avids.Dapper.Lambda.Model;

namespace Avids.Dapper.Lambda.Core.SetC
{
    /// <summary>
    /// Represent Command from Dapper (Insert, Update, Delete)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CommandSet<T> : Command<T>, ICommandSet<T>
    {
        public CommandSet(IDbConnection conn, SqlProvider sqlProvider) : base(sqlProvider, conn)
        {
            SqlProvider.SetContext.TableType = typeof(T);
        }

        public CommandSet(IDbConnection conn, SqlProvider sqlProvider, IDbTransaction dbTransaction) : base(sqlProvider, conn, dbTransaction)
        {
            SqlProvider.SetContext.TableType = typeof(T);
        }

        /// <inheritdoc />
        public ICommand<T> Where(Expression<Func<T, bool>> predicate)
        {
            Where where = new();
            where.WhereType = SqlProvider.SetContext.WhereExpressions.Count > 0 ? EWhere.AND : null;
            where.WhereExpression = predicate;
            SqlProvider.SetContext.WhereExpressions.Enqueue(where);
            return this;
        }

        /// <inheritdoc />
        public IInsert<T> IfNotExists(Expression<Func<T, bool>> predicate)
        {
            SqlProvider.SetContext.IfNotExistsExpression = SqlProvider.SetContext.IfNotExistsExpression == null ? 
                predicate : ((Expression<Func<T, bool>>)SqlProvider.SetContext.IfNotExistsExpression).And(predicate);

            return this;
        }

        //public void BatchInsert(IEnumerable<T> entities, int timeout = 120)
        //{
        //    SqlProvider.ExcuteBulkCopy(DbCon, entities);
        //}
    }
}
