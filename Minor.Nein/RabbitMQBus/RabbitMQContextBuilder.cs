namespace Minor.Nein.RabbitMQBus
{
    using System;
    using Microsoft.Extensions.Logging;
    using RabbitMQ.Client;
    using RabbitMQ.Client.Exceptions;

    public class RabbitMQContextBuilder
    {
        private readonly ILogger _log;
        private string _password;
        public string ExchangeName { get; private set; }
        public string HostName { get; private set; }
        public int Port { get; private set; }
        public string UserName { get; private set; }

        public RabbitMQContextBuilder()
        {
            _log = NeinLogger.CreateLogger<RabbitMQContextBuilder>();
        }

        private void ValidateContextVariables()
        {
            if (string.IsNullOrEmpty(HostName))
            {
                _log.LogError($"{NeinLogger.GetFunctionInformation()} hostname is not set!");
                throw new ArgumentNullException(nameof(HostName));
            }

            if (Port < 0 || Port > 65535)
            {
                _log.LogError($"{NeinLogger.GetFunctionInformation()} port out of range!");
                throw new ArgumentOutOfRangeException(nameof(Port));
            }
        }

        /// <summary>
        ///     Creates a context with
        ///     - an opened connection (based on HostName, Port, UserName and Password)
        ///     - a declared Topic-Exchange (based on ExchangeName)
        /// </summary>
        /// <returns></returns>
        public RabbitMQBusContext CreateContext(IConnectionFactory factory = null)
        {
            _log.LogInformation("Creating RabbitMQ Connection");
            ValidateContextVariables();

            factory = factory
                      ?? new ConnectionFactory
                         {
                                 HostName = HostName
                               , UserName = UserName
                               , Password = _password
                               , Port = Port
                         };

            try
            {
                IConnection connection = factory.CreateConnection();
                var context = new RabbitMQBusContext(connection, ExchangeName);

                using (IModel channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare(ExchangeName, "topic");
                }

                _log.LogInformation("RabbitMQ connection succesfully created");
                return context;
            }
            catch (BrokerUnreachableException e)
            {
                _log.LogError("Could not connect with the RabbitMQ Environment");
                throw;
            }
        }

        public RabbitMQContextBuilder ReadFromEnvironmentVariables()
        {
            if (TryGetFromEnvironmentVariable("ExchangeName", out string exchangeName))
            {
                ExchangeName = exchangeName;
            }

            if (TryGetFromEnvironmentVariable("HostName", out string hostName))
            {
                HostName = hostName;
            }

            if (TryGetFromEnvironmentVariable("Port", out string portString))
            {
                try
                {
                    Port = Convert.ToInt32(portString);
                }
                catch (Exception)
                {
                    throw new InvalidCastException("Could not convert PORT environment variable to a number");
                }
            }

            if (TryGetFromEnvironmentVariable("UserName", out string userName))
            {
                UserName = userName;
            }

            if (TryGetFromEnvironmentVariable("Password", out string password))
            {
                _password = password;
            }

            _log.LogTrace($"Creating Context with the use of environment variables. username {UserName}, hostname {HostName}, port {Port}, exchangeName {ExchangeName}");

            return this;
        }

        public RabbitMQContextBuilder WithAddress(string hostName, int port)
        {
            _log.LogTrace("Creating Context with hostname " + hostName + " And port " + port);
            HostName = hostName;
            Port = port;

            return this;
        }

        public RabbitMQContextBuilder WithCredentials(string userName, string password)
        {
            _log.LogTrace("Creating Context with username " + userName);
            UserName = userName;
            _password = password;

            return this;
        }

        public RabbitMQContextBuilder WithExchange(string exchangeName)
        {
            _log.LogTrace("Creating Context with exchangeName " + exchangeName);
            ExchangeName = exchangeName;

            return this;
        }

        private bool TryGetFromEnvironmentVariable(string key, out string variable)
        {
            variable = Environment.GetEnvironmentVariable(key);
            return variable != null;
        }
    }
}