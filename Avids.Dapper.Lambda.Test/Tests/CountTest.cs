using Npgsql;

using Avids.Dapper.Lambda.Test.Entity;

namespace Avids.Dapper.Lambda.Test.Tests
{
    public class CountTest
    {
        [Fact]
        public void TestCount()
        {
            string expected = @"SELECT COUNT(*) FROM ""Invoice""";
            string actual = new NpgsqlConnection().CommandSet<Invoice>()
                .SqlProvider.FormatCount().SqlString.Trim();
            Assert.Equal(expected, actual);
        }
    }
}
