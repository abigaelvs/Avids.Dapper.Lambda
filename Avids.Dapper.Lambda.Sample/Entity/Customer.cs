using System.ComponentModel.DataAnnotations.Schema;

namespace Avids.Dapper.Lambda.Sample.Entity
{
    public class Customer
    {
        public long CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerAddress { get; set; }

        public Customer() { }
    }
}
