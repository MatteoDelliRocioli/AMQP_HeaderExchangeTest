using System;
using System.Collections.Generic;
using System.Text;
using RabbitMQ.Client;

namespace AMQP_HeaderExchangeTestPublisher
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var factory = new ConnectionFactory()
            {
                HostName = "localhost"
            };

            using (var connection = factory.CreateConnection())
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


                    Console.WriteLine("please enter the first message to send");
                    string message;
                    do
                    {
                        message = Console.ReadLine();
                        var body = Encoding.UTF8.GetBytes(message);

                        var properties = channel.CreateBasicProperties();
                        properties.Persistent = true;
                        properties.Headers = messageHeader;

                        channel.ConfirmSelect();
                        //declaring Exchange
                        Dictionary<string, object> exchangeArgs = new Dictionary<string, object>();
                        exchangeArgs.Add("x-match", "all");
                        exchangeArgs.Add("category", "animal");
                        exchangeArgs.Add("type", "mammal");
                        exchangeArgs.Add("style", "black");

                        Dictionary<string, object> voidExchangeArgs = new Dictionary<string, object>();

                        Dictionary<string, object> subSetExchangeArgs = new Dictionary<string, object>();
                        subSetExchangeArgs.Add("x-match", "all");
                        subSetExchangeArgs.Add("category", "animal");
                        subSetExchangeArgs.Add("type", "mammal");

                        channel.ExchangeDeclare(exchange: "VoidHeaderExchangeTest", type: "headers", durable: true, autoDelete: false, arguments: voidExchangeArgs);
                        //channel.ExchangeDeclare(exchange: "HeaderExchangeTest", type: "headers", durable: true, autoDelete: false, arguments: subSetExchangeArgs);
                        
                        channel.QueueBind(queue: "queueTestForHeader", exchange: "HeaderExchangeTest", routingKey:"", arguments: subSetExchangeArgs);
                        channel.QueueBind(queue: "queueTestForHeader", exchange: "VoidHeaderExchangeTest", routingKey:"", arguments: subSetExchangeArgs);
                        //channel.ExchangeBind(destination: "HeaderExchangeTest", source: "HeaderExchangeTest", routingKey: "", arguments: bindingOneHeaders);

                        channel.BasicPublish(exchange: "HeaderExchangeTest",
                                                routingKey: "",
                                                basicProperties: properties,
                                                body: body);

                        Console.WriteLine("[x] message sent: {0}", message);
                        Console.WriteLine("Enter another message...");
                    } while (!message.Contains("END"));
                }
            }
        }
    }
}
