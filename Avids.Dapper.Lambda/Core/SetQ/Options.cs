using System.ComponentModel;
using System.Data;
using System.Linq.Expressions;
using System.Net;
using Avids.Dapper.Lambda.Core.Interfaces;

namespace Avids.Dapper.Lambda.Core.SetQ
{
    /// <summary>
    /// Represent Query from Dapper
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Option<T> : Query<T>, IOption<T>
    {
        protected Option(IDbConnection conn, SqlProvider sqlProvider) : base(conn, sqlProvider)
        {

        }

        protected Option(IDbConnection conn, SqlProvider sqlProvider, IDbTransaction dbTransaction) : base(conn, sqlProvider, dbTransaction)
        {

        }

        /// <inheritdoc />
        //public virtual Query<TResult> Select<TResult>(Expression<Func<T, TResult>> selector)
        //{
        //    SqlProvider.SetContext.SelectExpression = selector;

        //    return new QuerySet<TResult>(DbCon, SqlProvider, typeof(T), DbTransaction);
        //}

        public virtual Option<T> Select<E>(Expression<Func<T, E>> selector)
        {
            CreateSelect(selector);
            return this;
        }

        public virtual Option<T> Select<S, E>(Expression<Func<S, E>> selector)
        {
            CreateSelect(selector);
            return this;
        }

        private void CreateSelect(LambdaExpression predicate)
        {
            Avids.Dapper.Lambda.Model.Select select = new();
            select.SelectExpression = predicate;
            SqlProvider.SetContext.SelectExpressions.Enqueue(select);
        }
        //public virtual QuerySet<TResult> Select<TResult>(Expression<Func<T, TResult>> selector)
        //{
        //    SqlProvider.SetContext.SelectExpression = selector;

        //    return new QuerySet<TResult>(DbCon, SqlProvider, typeof(T), DbTransaction);
        //}

        /// <inheritdoc />
        public virtual Option<T> Limit(int num)
        {
            SqlProvider.SetContext.LimitNum = num;
            return this;
        }

        public virtual Option<T> Offset(int num)
        {
            SqlProvider.SetContext.OffsetNum = num;
            return this;
        }
    }
}
