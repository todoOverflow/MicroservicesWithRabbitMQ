using System.Text;
using RabbitMQ.Client;
using Newtonsoft.Json;
namespace MicroRMQ.Bus
{
    public class RabbitMQBus : IEventBus
    {
        public void Publish<T>(T @event) where T : Event
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    var eventName = @event.GetType().Name;
                    var message = JsonConvert.SerializeObject(@event);
                    var body = Encoding.UTF8.GetBytes(message);

                    channel.QueueDeclare(eventName, false, false, false, null);
                    channel.BasicPublish("", eventName, false, null, body);
                }
            }
        }

        public void Subscribe<T, TH>()
            where T : Event
            where TH : IEventHandler
        {
            throw new System.NotImplementedException();
        }
    }
}