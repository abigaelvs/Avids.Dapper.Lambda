using Avids.Dapper.Lambda.Test.Entity;
using Npgsql;

namespace Avids.Dapper.Lambda.Test
{
    public class DeleteTest
    {
        [Fact]
        public void TestDelete()
        {
            string expected = @"DELETE FROM ""Invoice""";
            string actual = new NpgsqlConnection().CommandSet<Invoice>()
                .SqlProvider.FormatDelete().SqlString.Trim();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestDeleteWithWhere()
        {
            string expected = @"DELETE FROM ""Invoice"" WHERE ""Id"" = @Id1";
            string actual = new NpgsqlConnection().CommandSet<Invoice>()
                .Where(inv => inv.Id == 1)
                .SqlProvider.FormatDelete().SqlString.Trim();
            Assert.Equal(expected, actual);
        }
    }
}
