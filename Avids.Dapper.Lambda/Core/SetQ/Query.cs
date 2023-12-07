using System;
using System.Data;
using System.Linq;

using static Dapper.SqlMapper;

using Avids.Dapper.Lambda.Core.Interfaces;
using Avids.Dapper.Lambda.Model;
using System.Threading.Tasks;
using System.Collections.Generic;

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

        /// <inheritdoc />
        public T Get()
        {
            SqlProvider.FormatGet<T>();

            return DbCon.QueryFirstOrDefault<T>(SqlProvider.SqlString, SqlProvider.Params, DbTransaction);
        }

        /// <inheritdoc />
        public async Task<T> GetAsync()
        {
            SqlProvider.FormatGet<T>();

            return await DbCon.QueryFirstOrDefaultAsync<T>(SqlProvider.SqlString, SqlProvider.Params, DbTransaction);
        }

        /// <inheritdoc />
        public List<T> ToList()
        {
            SqlProvider.FormatToList<T>();
            
            return DbCon.Query<T>(SqlProvider.SqlString, SqlProvider.Params, DbTransaction).ToList();
        }

        /// <inheritdoc />
        public async Task<List<T>> ToListAsync()
        {
            SqlProvider.FormatToList<T>();

            IEnumerable<T> result = await DbCon.QueryAsync<T>(SqlProvider.SqlString, SqlProvider.Params, DbTransaction);
            return result.ToList();
        }

        /// <inheritdoc />
        public PageList<T> PageList(int pageIndex, int pageSize)
        {
            SqlProvider.FormatToPageList<T>(pageIndex, pageSize);

            using (GridReader queryResult = DbCon.QueryMultiple(SqlProvider.SqlString, SqlProvider.Params, DbTransaction))
            {
                int pageTotal = queryResult.ReadFirst<int>();

                IEnumerable<T> itemList = queryResult.Read<T>();

                return new PageList<T>(pageIndex, pageSize, pageTotal, itemList);
            }
        }
    }
}
