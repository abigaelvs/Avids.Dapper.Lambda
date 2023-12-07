using Avids.Dapper.Lambda.Sample.Entity;
using Avids.Dapper.Lambda.Sample.Models;

namespace Avids.Dapper.Lambda.Sample.Services
{
    public interface IInvoiceService
    {
        Task<IEnumerable<SearchInvoiceList>> SearchInvoice(SearchInvoiceRequestDto request);
    }
}
