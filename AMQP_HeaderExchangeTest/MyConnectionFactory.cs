using RabbitMQ.Client;

namespace AMQP_HeaderExchangeTestPublisher
{
    public class MyConnectionFactory : ConnectionFactory
    {
        private const string _UserName = "guest";
        private const string _Password = "guest";
        private const string _HostName = "localhost";

        public MyConnectionFactory()
            :base ()
        {
            base.UserName = _UserName;
            base.Password = _Password;
            base.HostName = _HostName;
        }
    }
}
