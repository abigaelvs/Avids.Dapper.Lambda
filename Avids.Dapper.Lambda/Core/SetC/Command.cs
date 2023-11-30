using System.Data;
using System.Linq.Expressions;

using Dapper;

using Avids.Dapper.Lambda.Core.Interfaces;

namespace Avids.Dapper.Lambda.Core.SetC
{
    /// <summary>
    /// Represet Command from dapper
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Command<T> : AbstractSet, ICommand<T>, IInsert<T>
    {
        protected Command(SqlProvider sqlProvider, IDbConnection dbCon, IDbTransaction dbTransaction) : base(dbCon, sqlProvider, dbTransaction)
        {
        }

        protected Command(SqlProvider sqlProvider, IDbConnection dbCon) : base(dbCon, sqlProvider)
        {
        }

        public int Update(T entity)
        {
            SqlProvider.FormatUpdate(entity);

            Console.WriteLine(SqlProvider.SqlString);

            return DbCon.Execute(SqlProvider.SqlString, SqlProvider.Params, DbTransaction);
        }

        public async Task<int> UpdateAsync(T entity)
        {
            SqlProvider.FormatUpdate(entity);

            return await DbCon.ExecuteAsync(SqlProvider.SqlString, SqlProvider.Params, DbTransaction);
        }

        public int Update(Expression<Func<T, T>> updateExpression)
        {
            SqlProvider.FormatUpdate(updateExpression);

            return DbCon.Execute(SqlProvider.SqlString, SqlProvider.Params, DbTransaction);
        }

        public async Task<int> UpdateAsync(Expression<Func<T, T>> updateExpression)
        {
            SqlProvider.FormatUpdate(updateExpression);

            return await DbCon.ExecuteAsync(SqlProvider.SqlString, SqlProvider.Params, DbTransaction);
        }

        public int Delete()
        {
            SqlProvider.FormatDelete();

            return DbCon.Execute(SqlProvider.SqlString, SqlProvider.Params, DbTransaction);
        }

        public async Task<int> DeleteAsync()
        {
            SqlProvider.FormatDelete();

            return await DbCon.ExecuteAsync(SqlProvider.SqlString, SqlProvider.Params, DbTransaction);
        }

        public int Insert(T entity)
        {
            SqlProvider.FormatInsert(entity);

            Console.WriteLine(SqlProvider.SqlString);
            return DbCon.Execute(SqlProvider.SqlString, SqlProvider.Params, DbTransaction);
        }

        public int Insert(Dictionary<string, object> entity)
        {
            SqlProvider.FormatInsert(entity);

            Console.WriteLine(SqlProvider.SqlString);
            return DbCon.Execute(SqlProvider.SqlString, SqlProvider.Params, DbTransaction);
        }

        public async Task<int> InsertAsync(T entity)
        {
            SqlProvider.FormatInsert(entity);

            return await DbCon.ExecuteAsync(SqlProvider.SqlString, SqlProvider.Params, DbTransaction);
        }
    }
}
