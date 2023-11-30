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

        public QuerySet<T> Where(Expression<Func<T, bool>> predicate)
        {
            Where where = new();
            if (SqlProvider.SetContext.WhereExpressions.Count > 0) where.WhereType = EWhere.AND;
            where.WhereExpression = predicate;
            SqlProvider.SetContext.WhereExpressions.Enqueue(where);
            return this;
        }

        public QuerySet<T> Where<W>(Expression<Func<W, bool>> predicate)
        {
            Where where = new();
            where.WhereType = EWhere.AND;
            where.WhereExpression = predicate;
            SqlProvider.SetContext.WhereExpressions.Enqueue(where);
            //SqlProvider.SetContext.WhereExpressions.Add(predicate);
            return this;
        }

        public QuerySet<T> And<W>(Expression<Func<W, bool>> predicate)
        {
            Where where = new();
            where.WhereType = EWhere.AND;
            where.WhereExpression = predicate;
            SqlProvider.SetContext.WhereExpressions.Enqueue(where);
            return this;
        }

        public QuerySet<T> Or<W>(Expression<Func<W, bool>> predicate)
        {
            Where where = new();
            where.WhereType = EWhere.OR;
            where.WhereExpression = predicate;
            SqlProvider.SetContext.WhereExpressions.Enqueue(where);
            return this;
        }

        public QuerySet<T> WithNoLock()
        {
            SqlProvider.SetContext.NoLock = true;
            return this;
        }

        public QuerySet<T> InnerJoin<I, O>(Type tableType, Expression<Func<I, O, bool>> on)
        {
            CreateJoin(tableType, "INNER JOIN", on);
            return this;
        }

        public QuerySet<T> LeftJoin<I, O>(Type tableType, Expression<Func<I, O, bool>> on)
        {
            CreateJoin(tableType, "LEFT JOIN", on);
            return this;
        }

        public QuerySet<T> Join<I, O>(Expression<Func<I, O, bool>> on)
            where I : class
            where O : class
        {
            return this;
        }

        protected void CreateJoin<I, O>(Type tableType, string joinType, Expression<Func<I, O, bool>> onExpression)
        {
            Join join = new();
            join.TableType = tableType;
            join.JoinType = joinType;
            join.OnExpression = onExpression;
            SqlProvider.SetContext.JoinExpressions.Enqueue(join);
        }
    }
}
