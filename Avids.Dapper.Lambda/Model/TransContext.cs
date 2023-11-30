using System.Data;

using Avids.Dapper.Lambda.Core.SetC;
using Avids.Dapper.Lambda.Core.SetQ;

namespace Avids.Dapper.Lambda.Model
{
    public class TransContext
    {
        private readonly IDbConnection _sqlConnection;

        private readonly IDbTransaction _dbTransaction;

        private readonly SqlProvider _sqlProvider;

        public TransContext(IDbConnection sqlConnection, IDbTransaction dbTransaction, SqlProvider sqlProvider)
        {
            _sqlConnection = sqlConnection;
            _dbTransaction = dbTransaction;
            _sqlProvider = sqlProvider;
        }

        public QuerySet<T> QuerySet<T>()
        {
            return new QuerySet<T>(_sqlConnection, _sqlProvider, _dbTransaction);
        }

        public CommandSet<T> CommandSet<T>()
        {
            return new CommandSet<T>(_sqlConnection, _sqlProvider, _dbTransaction);
        }
    }
}