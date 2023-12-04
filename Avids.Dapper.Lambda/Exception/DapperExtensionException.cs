using System;

namespace Avids.Dapper.Lambda.Exception
{
    public class DapperExtensionException : ApplicationException
    {
        public DapperExtensionException(string msg) : base(msg) { }
    }
}
