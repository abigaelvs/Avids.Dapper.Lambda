using Npgsql;

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
    }
}
