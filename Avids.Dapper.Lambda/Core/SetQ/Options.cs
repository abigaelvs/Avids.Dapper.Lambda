using System;
using System.Data;
using System.Linq.Expressions;

using Avids.Dapper.Lambda.Model;
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
        public virtual Option<T> Select<E>(Expression<Func<T, E>> selector)
        {
            CreateSelect(selector);
            return this;
        }

        public virtual Option<T> Distinct()
        {
            SqlProvider.SetContext.Distinct = true;
            return this;
        }

        /// <inheritdoc />
        private void CreateSelect(LambdaExpression predicate)
        {
            Select select = new Select();
            select.SelectExpression = predicate;
            SqlProvider.SetContext.SelectExpressions.Enqueue(select);
        }

        /// <inheritdoc />
        public virtual Option<T> Limit(int num)
        {
            SqlProvider.SetContext.LimitNum = num;
            return this;
        }

        /// <inheritdoc />
        public virtual Option<T> Offset(int num)
        {
            SqlProvider.SetContext.OffsetNum = num;
            return this;
        }
    }
}
