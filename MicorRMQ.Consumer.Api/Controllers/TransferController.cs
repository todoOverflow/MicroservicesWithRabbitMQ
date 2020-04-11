using System.Collections.Generic;
using System.Threading.Tasks;
using MicorRMQ.Consumer.Domain;
using MicorRMQ.Consumer.Application;
using Microsoft.AspNetCore.Mvc;
using MediatR;

namespace MicorRMQ.Consumer.Api.Controllers
{
    [ApiController]
    public class TransferController : ControllerBase
    {
        private readonly IMediator _mediator;
        public TransferController(IMediator mediator)
        {
            this._mediator = mediator;
        }

        [HttpGet]
        [Route("TransferList")]
        public async Task<ActionResult<List<TransferLog>>> List()
        {
            return await _mediator.Send(new TransferList.Query());
        }
    }
}