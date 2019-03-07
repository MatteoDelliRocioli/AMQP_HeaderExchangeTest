using System;
using System.Collections.Generic;
using System.Text;
using RabbitMQ.Client;

namespace AMQP_HeaderExchangeTestPublisher.models
{
    public class HeadersMessages
    {
        private const string _UserName = "guest";
        private const string _Password = "guest";
        private const string _HostName = "localhost";
        string message;

        public void SendMessage()
        {
            var connectionFactory = new ConnectionFactory()
            {
                UserName = _UserName,
                Password = _Password,
                HostName = _HostName
            };

            using (var connection = connectionFactory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    Dictionary<string, object> queueArgs = new Dictionary<string, object>();
                    queueArgs.Add("category", "animal");
                    queueArgs.Add("type", "mammal");

                    channel.QueueDeclare(queue: "queueTestForHeader",
                                            durable: true,
                                            autoDelete: false,
                                            exclusive: false,
                                            arguments: queueArgs);

                    Dictionary<string, object> messageHeader = new Dictionary<string, object>();
                    messageHeader.Add("category", "animal");
                    messageHeader.Add("type", "mammal");

                    var properties = channel.CreateBasicProperties();
                    properties.Persistent = true;
                    properties.Headers = messageHeader;

                    channel.ConfirmSelect();

                    Dictionary<string, object> voidExchangeArgs = new Dictionary<string, object>();

                    Dictionary<string, object> subSetExchangeArgs = new Dictionary<string, object>();
                    subSetExchangeArgs.Add("x-match", "all");
                    subSetExchangeArgs.Add("category", "animal");
                    subSetExchangeArgs.Add("type", "mammal");

                    channel.ExchangeDeclare(exchange: "VoidHeaderExchangeTest", type: "headers", durable: true, autoDelete: false, arguments: voidExchangeArgs);

                    channel.QueueBind(queue: "queueTestForHeader", exchange: "VoidHeaderExchangeTest", routingKey: "", arguments: subSetExchangeArgs);
                    //channel.ExchangeBind(destination: "HeaderExchangeTest", source: "HeaderExchangeTest", routingKey: "", arguments: bindingOneHeaders);

                    //read a string from CLI and send it to the exchange
                    do
                    {
                        message = Console.ReadLine();
                        if(message.ToUpper().Contains("STOP"))
                        {
                            break;
                        }
                        var body = Encoding.UTF8.GetBytes(message);

                        channel.BasicPublish(exchange: "VoidHeaderExchangeTest",
                                                routingKey: "",
                                                basicProperties: properties,
                                                body: body);

                        Console.WriteLine("[x] message sent: {0}", message);
                        Console.WriteLine("Enter another message...");
                    } while (!message.ToUpper().Contains("STOP"));
                }
            }
        }
    }
}
