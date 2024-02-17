using Npgsql;

using Avids.Dapper.Lambda.Core.SetC;
using Avids.Dapper.Lambda.Test.Entity;

namespace Avids.Dapper.Lambda.Test.Tests
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
            string actual = (new NpgsqlConnection().CommandSet<Invoice>()
                .Where(inv => inv.Id == 1) as Command<Invoice>)
                .SqlProvider.FormatDelete().SqlString.Trim();
            Assert.Equal(expected, actual);
        }
    }
}
