using Avids.Dapper.Lambda.Test.Entity;
using Npgsql;

namespace Avids.Dapper.Lambda.Test.Tests
{
    public class GroupByTest
    {
        [Fact]
        public void TestGroupBy()
        {
            string expected = @"SELECT  * FROM ""Invoice""    GROUP BY ""Id"", ""No""";
            string result = new NpgsqlConnection().QuerySet<Invoice>().GroupBy(inv => inv.Id)
                .GroupBy(inv => inv.No).SqlProvider.FormatToList<Invoice>().SqlString.Trim();
            Assert.Equal(expected, result);
        }
    }
}
