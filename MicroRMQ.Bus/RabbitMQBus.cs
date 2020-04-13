using System.Text;
using RabbitMQ.Client;
using Newtonsoft.Json;
using System.Collections.Generic;
using System;
using System.Linq;
using RabbitMQ.Client.Events;
using System.Threading.Tasks;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
namespace MicroRMQ.Bus
{
    public class RabbitMQBus : IEventBus
    {
        private readonly List<Type> _eventTypes;
        private readonly Dictionary<string, List<Type>> _handlerTypes;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public RabbitMQBus(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _eventTypes = new List<Type>();
            _handlerTypes = new Dictionary<string, List<Type>>();
        }

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
            var eventType = typeof(T);
            var eventName = typeof(T).Name;
            var handlerType = typeof(TH);

            if (!_eventTypes.Contains(eventType))
            {
                _eventTypes.Add(eventType);
            }

            if (!_handlerTypes.ContainsKey(eventName))
            {
                _handlerTypes.Add(eventName, new List<Type>());
            }
            if (!_handlerTypes[eventName].Any(h => h.GetType() == handlerType))
            {
                _handlerTypes[eventName].Add(handlerType);
            }
            startBasicConsume<T>();
        }

        private void startBasicConsume<T>() where T : Event
        {
            var factory = new ConnectionFactory() { HostName = "localhost", DispatchConsumersAsync = true };
            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();

            var eventName = typeof(T).Name;
            channel.QueueDeclare(eventName, false, false, false, null);

            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.Received += consumer_Received;

            channel.BasicConsume(eventName, true, consumer);
        }

        private async Task consumer_Received(object sender, BasicDeliverEventArgs eventArgs)
        {
            var message = Encoding.UTF8.GetString(eventArgs.Body);
            var eventName = eventArgs.RoutingKey;
            try
            {
                await ProcessEvent(eventName, message).ConfigureAwait(false);
            }
            catch (System.Exception ex)
            {
                throw ex;
            }

        }

        private async Task ProcessEvent(string eventName, string message)
        {
            if (!_handlerTypes.ContainsKey(eventName))
            {
                throw new Exception($"No handlers for {eventName}");
            }
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var eventType = _eventTypes.SingleOrDefault(e => e.Name == eventName);
                var eventObj = JsonConvert.DeserializeObject(message, eventType);

                foreach (var subHandler in _handlerTypes[eventName])
                {
                    var handlerObj = scope.ServiceProvider.GetService(subHandler);
                    if (handlerObj == null) continue;

                    var handlerConcreteType = typeof(IEventHandler<>).MakeGenericType(eventType);
                    MethodInfo handle = handlerConcreteType.GetMethod("Handle");
                    await (Task)handle.Invoke(handlerObj, new object[] { eventObj });
                }
            }
        }
    }
}