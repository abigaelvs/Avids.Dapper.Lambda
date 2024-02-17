using Avids.Dapper.Lambda.Test.Entity;
using Npgsql;

namespace Avids.Dapper.Lambda.Test.Tests
{
    public class WhereTest
    {
        [Fact]
        public void TestWhere()
        {
            List<long> ids = new() { 1, 2, 3 };
            List<string> nos = new() { "IV1", "IV2" };
            string expected = @"SELECT  * FROM ""Invoice""   WHERE ""Id"" = @Id1 AND ""StatusId"" > @StatusId2 "
                + @"AND ""StatusId"" < @StatusId3 OR ""StatusId"" <= @StatusId4 AND ""StatusId"" >= @StatusId5 "
                + @"AND ""Id"" != @Id6 OR ""No"" LIKE @No7 AND ""No"" LIKE @No8 AND ""No"" LIKE @No9 "
                + @"OR ""No"" NOT LIKE @No10 AND ""No"" NOT LIKE @No11 AND ""No"" NOT LIKE @No12 "
                + @"OR ""Id"" IN (@Id13, @Id14, @Id15) AND ""No"" IN (@No16, @No17) AND ""No"" IS NULL "
                + @"OR ""Id"" NOT IN (@Id19, @Id20, @Id21) AND ""No"" NOT IN (@No22, @No23)";
            SqlProvider provider = new NpgsqlConnection().QuerySet<Invoice>()
                .Where(inv => inv.Id == 1 && inv.StatusId > 1 && inv.StatusId < 1
                || inv.StatusId <= 1 && inv.StatusId >= 1 && inv.Id != 1
                || inv.No.Contains("IV") && inv.No.StartsWith("IV") && inv.No.EndsWith("IV")
                || !inv.No.Contains("IV") && !inv.No.StartsWith("IV") && !inv.No.EndsWith("IV")
                || ids.Contains(inv.Id) && nos.Contains(inv.No) && inv.No == null
                || !ids.Contains(inv.Id) && !nos.Contains(inv.No))
                .SqlProvider;

            string actual = provider.FormatToList<Invoice>().SqlString.Trim();
            Assert.Equal(expected, actual);
        }
    }
}
