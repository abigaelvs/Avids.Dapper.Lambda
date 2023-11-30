using System.Data;

using Avids.Dapper.Lambda.Core.Interfaces;
using Avids.Dapper.Lambda.Model;
using static Dapper.SqlMapper;

namespace Avids.Dapper.Lambda.Core.SetQ
{
    /// <summary>
    /// Represent Query from Dapper
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Query<T> : AbstractSet, IQuery<T>
    {
        protected Query(IDbConnection dbCon, SqlProvider sqlProvider) : base(dbCon, sqlProvider) { }
        protected Query(IDbConnection dbCon, SqlProvider sqlProvider, IDbTransaction dbTransaction) 
            : base(dbCon, sqlProvider, dbTransaction) { }

        public T Get()
        {
            SqlProvider.FormatGet<T>();

            return DbCon.QueryFirstOrDefault<T>(SqlProvider.SqlString, SqlProvider.Params, DbTransaction);
        }

        public async Task<T> GetAsync()
        {
            SqlProvider.FormatGet<T>();

            return await DbCon.QueryFirstOrDefaultAsync<T>(SqlProvider.SqlString, SqlProvider.Params, DbTransaction);
        }

        public IEnumerable<T> ToList()
        {
            SqlProvider.FormatToList<T>();

            Console.WriteLine(SqlProvider.SqlString);
            
            return DbCon.Query<T>(SqlProvider.SqlString, SqlProvider.Params, DbTransaction);
        }

        public async Task<IEnumerable<T>> ToListAsync()
        {
            SqlProvider.FormatToList<T>();

            return await DbCon.QueryAsync<T>(SqlProvider.SqlString, SqlProvider.Params, DbTransaction);
        }

        public PageList<T> PageList(int pageIndex, int pageSize)
        {
            SqlProvider.FormatToPageList<T>(pageIndex, pageSize);

            using GridReader queryResult = DbCon.QueryMultiple(SqlProvider.SqlString, SqlProvider.Params, DbTransaction);
            
            int pageTotal = queryResult.ReadFirst<int>();

            IEnumerable<T> itemList = queryResult.Read<T>();

            return new PageList<T>(pageIndex, pageSize, pageTotal, itemList);
        }
    }
}
