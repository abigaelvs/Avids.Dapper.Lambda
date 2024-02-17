using Npgsql;

using Avids.Dapper.Lambda.Test.Entity;

namespace Avids.Dapper.Lambda.Test.Tests
{
    public class OrderByTest
    {
        [Fact]
        public void TestOrderBy()
        {
            string expected = @"SELECT  * FROM ""Invoice""     ORDER BY ""Id"" ASC, ""No"" DESC";
            string result = new NpgsqlConnection().QuerySet<Invoice>().OrderBy(inv => inv.Id)
                .OrderByDesc(inv => inv.No).SqlProvider.FormatToList<Invoice>().SqlString.Trim();
            Assert.Equal(expected, result);
        }
    }
}
