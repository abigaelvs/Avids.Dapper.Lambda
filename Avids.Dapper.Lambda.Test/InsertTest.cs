﻿using Npgsql;

using Avids.Dapper.Lambda.Core.SetC;
using Avids.Dapper.Lambda.Test.Entity;

namespace Avids.Dapper.Lambda.Test
{
    public class InsertTest
    {
        [Fact]
        public void TestInsertInto()
        {
            string expected = @"INSERT INTO ""Invoice"" (""Id"",""No"",""StatusId""," 
                + @"""PaymentStatusId"",""CashierId"",""UpdatedByUserId"",""CustomerId""," 
                + @"""CreatedDate"",""UpdatedDate"") VALUES (@Id,@No,@StatusId,@PaymentStatusId,"
                + @"@CashierId,@UpdatedByUserId,@CustomerId,@CreatedDate,@UpdatedDate)";

            Invoice inv = new();
            inv.Id = 1;
            inv.No = "IV123";
            inv.CustomerId = 1;
            inv.CashierId = 1;
            inv.StatusId = 1;
            inv.CreatedDate = DateTime.Now;
            inv.UpdatedByUserId = null;
            inv.UpdatedDate = null;

            string actual = new NpgsqlConnection().CommandSet<Invoice>()
                .SqlProvider.FormatInsert(inv).SqlString.Trim();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestInsertIfNotExists()
        {
            string expected = @"INSERT INTO ""Invoice"" (""Id"",""No"",""StatusId"","
                + @"""PaymentStatusId"",""CashierId"",""UpdatedByUserId"",""CustomerId"","
                + @"""CreatedDate"",""UpdatedDate"") SELECT @Id,@No,@StatusId,@PaymentStatusId,"
                + @"@CashierId,@UpdatedByUserId,@CustomerId,@CreatedDate,@UpdatedDate "
                + @"WHERE NOT EXISTS(SELECT 1 FROM ""Invoice"" WHERE ""Id"" = @INT_Id1)";

            Invoice inv = new();
            inv.Id = 1;
            inv.No = "IV123";
            inv.CustomerId = 1;
            inv.CashierId = 1;
            inv.StatusId = 1;
            inv.CreatedDate = DateTime.Now;
            inv.UpdatedByUserId = null;
            inv.UpdatedDate = null;

            string actual = (new NpgsqlConnection().CommandSet<Invoice>()
                .IfNotExists(inv => inv.Id == 1) as Command<Invoice>)
                .SqlProvider.FormatInsert(inv).SqlString.Trim();
            Assert.Equal(expected, actual);
        }
    }
}
