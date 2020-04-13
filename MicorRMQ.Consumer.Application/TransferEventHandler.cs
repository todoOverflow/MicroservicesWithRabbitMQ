using System.Threading.Tasks;
using MicorRMQ.Consumer.Domain;
using MicorRMQ.Consumer.Persistence;
using MicroRMQ.Bus;

namespace MicorRMQ.Consumer.Application
{
    public class TransferEventHandler : IEventHandler<TransferEvent>
    {
        private readonly DataContext _dataContext;
        public TransferEventHandler(DataContext dataContext)
        {
            this._dataContext = dataContext;

        }
        public Task Handle(TransferEvent @event)
        {
            _dataContext.TransferLogs.Add(
                new TransferLog()
                {
                    FromAccount = @event.From,
                    ToAccount = @event.To,
                    TransferAmount = @event.Amount
                }
            );
            _dataContext.SaveChanges();
            return Task.CompletedTask;
        }
    }
}