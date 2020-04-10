using Microsoft.AspNetCore.Mvc;
using MediatR;
using System.Threading.Tasks;
using System.Collections.Generic;
using MicroRMQ.Producer.Domain;
using MicroRMQ.Producer.Application;

namespace MicroRMQ.Producer.Api.Controllers
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
        [Route("AccountList")]
        public async Task<ActionResult<List<Account>>> List()
        {
            return await _mediator.Send(new AccountList.Query());
        }

        [HttpPost]
        [Route("CreateTransfer")]
        public async Task<ActionResult<Unit>> Create([FromBody] CreateTransfer.Command command)
        {
            return await _mediator.Send(command);
        }
    }
}