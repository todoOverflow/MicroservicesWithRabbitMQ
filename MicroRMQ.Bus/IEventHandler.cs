
using System.Threading.Tasks;

namespace MicroRMQ.Bus
{
    public interface IEventHandler
    {

    }
    public interface IEventHandler<in TEvent> : IEventHandler where TEvent : Event
    {
        Task Handle(TEvent @event);
    }
}