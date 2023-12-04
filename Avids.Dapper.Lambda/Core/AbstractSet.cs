using System.Data;

namespace Avids.Dapper.Lambda.Core
{
    /// <summary>
    /// Abstraction for QuerySet and CommandSet
    /// </summary>
    public abstract class AbstractSet
    {
        public SqlProvider SqlProvider { get; protected set; }
        public IDbConnection DbCon { get; protected set; }
        public IDbTransaction DbTransaction { get; protected set; }

        protected AbstractSet(IDbConnection dbCon, SqlProvider sqlProvider, IDbTransaction dbTransaction)
        {
            SqlProvider = sqlProvider;
            DbCon = dbCon;
            DbTransaction = dbTransaction;
        }

        protected AbstractSet(IDbConnection dbCon, SqlProvider sqlProvider)
        {
            SqlProvider = sqlProvider;
            DbCon = dbCon;
        }
    }
}
