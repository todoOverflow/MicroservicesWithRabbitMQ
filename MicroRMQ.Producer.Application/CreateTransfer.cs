using System.Threading;
using System.Threading.Tasks;

using MediatR;

using MicroRMQ.Bus;
using MicroRMQ.Producer.Domain;

namespace MicroRMQ.Producer.Application
{
    public class CreateTransfer
    {
        public class Command : IRequest
        {
            public int From { get; set; }
            public int To { get; set; }
            public decimal Amount { get; set; }
        }

        public class Handler : IRequestHandler<Command>
        {

            private readonly IEventBus _bus;
            public Handler(IEventBus bus)
            {
                _bus = bus;
            }
            public Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                _bus.Publish(new TransferEvent(request.From, request.To, request.Amount));
                return Task.FromResult(Unit.Value);
            }
        }
    }
}