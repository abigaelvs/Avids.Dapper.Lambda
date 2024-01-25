using System.ComponentModel.DataAnnotations.Schema;

namespace Avids.Dapper.Lambda.Test.Entity
{
    public class Customer
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public long UserId { get; set; }

        public Customer() { }
    }
}
