using Npgsql;

using Avids.Dapper.Lambda.Test.Entity;

namespace Avids.Dapper.Lambda.Test
{
    public class ExistsTest
    {
        [Fact]
        public void TestExists()
        {
            string expected = @"SELECT 1 FROM ""Invoice""  WHERE ""Id"" = @Id1 LIMIT 1";
            string actual = new NpgsqlConnection().CommandSet<Invoice>()
                .Where(inv => inv.Id == 1).SqlProvider
                .FormatExists().SqlString.Trim();
            Assert.Equal(expected, actual);
        }
    }
}
