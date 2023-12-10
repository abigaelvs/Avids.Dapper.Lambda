using System;
using System.Data;
using System.Linq.Expressions;

using Avids.Dapper.Lambda.Core.Interfaces;

using Avids.Dapper.Lambda.Model;

namespace Avids.Dapper.Lambda.Core.SetQ
{
    /// <summary>
    /// Represent Query from Dapper
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class QuerySet<T> : Aggregation<T>, IQuerySet<T>
    {
        public QuerySet(IDbConnection conn, SqlProvider sqlProvider) : base(conn, sqlProvider)
        {
            SqlProvider.SetContext.TableType = typeof(T);
        }

        public QuerySet(IDbConnection conn, SqlProvider sqlProvider, IDbTransaction dbTransaction) : base(conn, sqlProvider, dbTransaction)
        {
            SqlProvider.SetContext.TableType = typeof(T);
        }

        internal QuerySet(IDbConnection conn, SqlProvider sqlProvider, Type tableType, IDbTransaction dbTransaction) : base(conn, sqlProvider, dbTransaction)
        {
            SqlProvider.SetContext.TableType = tableType;
        }

        /// <inheritdoc />
        public QuerySet<T> Where(Expression<Func<T, bool>> predicate)
        {
            Where where = new Where();
            if (SqlProvider.SetContext.WhereExpressions.Count > 0) where.WhereType = EWhere.AND;
            where.WhereExpression = predicate;
            SqlProvider.SetContext.WhereExpressions.Enqueue(where);
            return this;
        }

        /// <inheritdoc />
        public QuerySet<T> Where<W>(Expression<Func<W, bool>> predicate)
        {
            Where where = new Where();
            where.WhereType = EWhere.AND;
            where.WhereExpression = predicate;
            SqlProvider.SetContext.WhereExpressions.Enqueue(where);
            return this;
        }

        /// <inheritdoc />
        public QuerySet<T> And<W>(Expression<Func<W, bool>> predicate)
        {
            Where where = new Where();
            where.WhereType = EWhere.AND;
            where.WhereExpression = predicate;
            SqlProvider.SetContext.WhereExpressions.Enqueue(where);
            return this;
        }

        /// <inheritdoc />
        public QuerySet<T> Or<W>(Expression<Func<W, bool>> predicate)
        {
            Where where = new Where();
            where.WhereType = EWhere.OR;
            where.WhereExpression = predicate;
            SqlProvider.SetContext.WhereExpressions.Enqueue(where);
            return this;
        }

        /// <inheritdoc />
        public QuerySet<T> WithNoLock()
        {
            SqlProvider.SetContext.NoLock = true;
            return this;
        }

        /// <inheritdoc />
        public QuerySet<T> InnerJoin<TJoinTable>(Expression<Func<TJoinTable, T, bool>> on)
        {
            CreateJoin(EJoin.InnerJoin, on);
            return this;
        }

        /// <inheritdoc />
        public QuerySet<T> LeftJoin<TJoinTable>(Expression<Func<TJoinTable, T, bool>> on)
        {
            CreateJoin(EJoin.LeftJoin, on);
            return this;
        }

        /// <inheritdoc />
        public QuerySet<T> RightJoin<TJoinTable>(Expression<Func<TJoinTable, T, bool>> on)
        {
            CreateJoin(EJoin.RightJoin, on);
            return this;
        }

        /// <inheritdoc />
        public QuerySet<T> FullJoin<ITJoinTable>(Expression<Func<ITJoinTable, T, bool>> on)
        {
            CreateJoin(EJoin.FullJoin, on);
            return this;
        }

        /// <summary>
        /// Add Join Expression to Queue Join Expressions
        /// </summary>
        /// <typeparam name="I"></typeparam>
        /// <typeparam name="O"></typeparam>
        /// <param name="joinType"></param>
        /// <param name="onExpression"></param>
        protected void CreateJoin<TJoinTable, O>(EJoin joinType, 
            Expression<Func<TJoinTable, O, bool>> onExpression)
        {
            // Set Has Join to true, to mark that sql provider has join
            if (SqlProvider.SetContext.JoinExpressions.Count < 1) SqlProvider.SetContext.HasJoin = true;

            Join join = new Join();
            join.TableType = typeof(TJoinTable);
            join.JoinType = joinType;
            join.OnExpression = onExpression;
            SqlProvider.SetContext.JoinExpressions.Enqueue(join);
        }
    }
}
