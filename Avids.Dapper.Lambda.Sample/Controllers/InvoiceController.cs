using Microsoft.AspNetCore.Mvc;

using Avids.Dapper.Lambda.Sample.Services;
using Avids.Dapper.Lambda.Sample.Models;
using Avids.Dapper.Lambda.Sample.Entity;

namespace Avids.Dapper.Lambda.Sample.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class InvoiceController : ControllerBase
    {
        private readonly IInvoiceService _invoiceService;

        public InvoiceController(IInvoiceService invoiceService)
        {
            _invoiceService = invoiceService;
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> SearchInvoice([FromQuery] SearchInvoiceRequestDto request)
        {
            IEnumerable<SearchInvoiceList> result = await _invoiceService.SearchInvoice(request);
            return Ok(result);
        }
    }
}
