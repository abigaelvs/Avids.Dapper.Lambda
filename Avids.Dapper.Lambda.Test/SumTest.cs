using Npgsql;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;

using Avids.Dapper.Lambda.Test.Entity;

namespace Avids.Dapper.Lambda.Test
{
    public class SumTest
    {
        [Fact]
        public void TestSum()
        {
            string expected = @"SELECT COALESCE(SUM(""Id""),0)   FROM ""Invoice""";
            string actual = new NpgsqlConnection().CommandSet<Invoice>()
                .SqlProvider.FormatSum<Invoice, long>(inv => inv.Id).SqlString.Trim();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestSumMsSql()
        {
            string expected = @"SELECT ISNULL(SUM([Id]),0)   FROM [Invoice]";
            string actual = new SqlConnection().CommandSet<Invoice>()
                .SqlProvider.FormatSum<Invoice, long>(inv => inv.Id).SqlString.Trim();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestSumMySql()
        {
            string expected = @"SELECT IFNULL(SUM(`Id`),0)   FROM `Invoice`";
            string actual = new MySqlConnection().CommandSet<Invoice>()
                .SqlProvider.FormatSum<Invoice, long>(inv => inv.Id).SqlString.Trim();
            Assert.Equal(expected, actual);
        }
    }
}
