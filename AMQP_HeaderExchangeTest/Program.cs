using System;
using AMQP_HeaderExchangeTestPublisher.models;

namespace AMQP_HeaderExchangeTestPublisher
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Please enter the first message to send\nType STOP to exit");

            HeadersMessages message = new HeadersMessages();
            message.SendMessage();

            Console.ReadLine();
        }
    }
}
