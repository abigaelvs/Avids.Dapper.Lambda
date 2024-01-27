using System.ComponentModel.DataAnnotations;

namespace Avids.Dapper.Lambda.Test.Entity
{
    internal class InvoiceBilling
    {
        [Key]
        public long InvoiceId { get; set; }
        [Key]
        public long BillingId { get; set; }
        public string BillingNo { get; set; }
    }
}
