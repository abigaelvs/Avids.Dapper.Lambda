﻿using System.ComponentModel.DataAnnotations.Schema;

namespace Avids.Dapper.Lambda.Test.Entity
{
    public class Cashier
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long UserId { get; set; }
    }
}
