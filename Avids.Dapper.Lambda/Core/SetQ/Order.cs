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
    public abstract class Order<T> : Option<T>, IOrder<T>
    {
        protected Order(IDbConnection conn, SqlProvider sqlProvider) : base(conn, sqlProvider)
        {

        }

        protected Order(IDbConnection conn, SqlProvider sqlProvider, IDbTransaction dbTransaction) : base(conn, sqlProvider, dbTransaction)
        {

        }

        /// <inheritdoc />
        public virtual Order<T> OrderBy<TProperty>(Expression<Func<T, TProperty>> field)
        {
            if (field != null)
                SqlProvider.SetContext.OrderbyExpressionList.Add(EOrderBy.Asc, field);

            return this;
        }

        /// <inheritdoc />
        public virtual Order<T> OrderByDesc<TProperty>(Expression<Func<T, TProperty>> field)
        {
            if (field != null)
                SqlProvider.SetContext.OrderbyExpressionList.Add(EOrderBy.Desc, field);

            return this;
        }

        public virtual Order<T> GroupBy<TProperty>(Expression<Func<T, TProperty>> field)
        {
            if (field != null) SqlProvider.SetContext.GroupByExpressionList.Enqueue(field); 
            return this;
        }
    }
}
