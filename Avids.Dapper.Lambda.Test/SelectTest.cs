using Npgsql;
using System.Data.SqlClient;

using Avids.Dapper.Lambda.Test.Entity;

namespace Avids.Dapper.Lambda.PostgreSql.Test
{
    public class SelectTest
    {
        [Fact]
        public void TestSelectAll()
        {
            string expected = @"SELECT  * FROM ""Invoice""";
            string actual = new NpgsqlConnection().QuerySet<Invoice>().SqlProvider.FormatToList<Invoice>()
                .SqlString.Trim();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void MsSqlTestSelectAll()
        {
            string expected = @"SELECT  * FROM [Invoice]";
            string actual = new SqlConnection().QuerySet<Invoice>().SqlProvider.FormatToList<Invoice>().SqlString.Trim();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestSelectCertainField()
        {
            string expected = @"SELECT  ""Id"", ""No"" FROM ""Invoice""";
            string actual = new NpgsqlConnection().QuerySet<Invoice>()
                .Select(inv => new Invoice { Id = inv.Id, No = inv.No })
                .SqlProvider.FormatToList<Invoice>().SqlString.Trim();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void MsSqlTestSelectCertainField()
        {
            string expected = @"SELECT  [Id], [No] FROM [Invoice]";
            string actual = new SqlConnection().QuerySet<Invoice>()
                .Select(inv => new Invoice { Id = inv.Id, No = inv.No })
                .SqlProvider.FormatToList<Invoice>().SqlString.Trim();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestSelectDistinct()
        {
            string expected = @"SELECT DISTINCT ""No"" FROM ""Invoice""";
            string actual = new NpgsqlConnection().QuerySet<Invoice>()
                .Select(inv => inv.No).Distinct().SqlProvider.FormatToList<Invoice>().SqlString.Trim();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void MsSqlTestSelectDistinct()
        {
            string expected = @"SELECT DISTINCT [No] FROM [Invoice]";
            string actual = new SqlConnection().QuerySet<Invoice>()
                .Select(inv => inv.No).Distinct().SqlProvider.FormatToList<Invoice>().SqlString.Trim();
            Assert.Equal(expected, actual);
        }
    }
}
