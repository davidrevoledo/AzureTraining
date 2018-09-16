using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace InvoiceV1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoiceController : ControllerBase
    {
        [HttpGet]
        [Route("")]
        public async Task<IActionResult> Get()
        {
            var invoiceNumber = await GenerateInvoice();

            return Ok($"Your Invoice has been generated {invoiceNumber}");
        }

        private async Task<string> GenerateInvoice()
        {
            var task = Task.Delay(10000);

            await task;

            return Guid.NewGuid().ToString("N");
        }
    }
}
