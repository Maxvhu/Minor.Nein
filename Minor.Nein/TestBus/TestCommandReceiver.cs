namespace Minor.Nein.TestBus
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using RabbitMQ.Client.Framing;

    public class TestCommandReceiver : ICommandReceiver
    {
        private bool _isDeclared;
        private bool _isListening;
        private TestBusContext _context { get; }
        private readonly ILogger _log;
        public string QueueName { get; }

        public TestCommandReceiver(TestBusContext context, string queueName)
        {
            _log = NeinLogger.CreateLogger<TestCommandReceiver>();
            QueueName = queueName;
            _context = context;
        }


        public void DeclareCommandQueue()
        {
            if (_isDeclared)
            {
                _log.LogWarning("[TestCommandReceiver]DeclareCommandQueue: Queue {QueueName} is already declared", QueueName);

                throw new BusConfigurationException("Already declared queue " + QueueName);
            }

            _context.DeclareCommandQueue(QueueName);
            _isDeclared = true;
        }

        public void StartReceivingCommands(CommandReceivedCallback callback)
        {
            if (_isListening)
            {                
                _log.LogWarning("[TestCommandReceiver]StartReceivingCommands: Already listening to queue {QueueName}", QueueName);

                throw new BusConfigurationException("Already listening to queuename " + QueueName);
            }

            new Task(() =>
            {
                var queue = _context.CommandQueues[QueueName];

                while (true)
                {
                    if (queue.Count == 0)
                    {
                        Thread.Sleep(250);
                        continue;
                    }

                    CommandMessage response = null;
                    TestBusCommandMessage command = queue.Dequeue();

                    try
                    {
                        response = callback.Invoke(command.Message);
                    }
                    catch (Exception e)
                    {
                        response = new CommandMessage(e.Message, command.Props.Type, command.Props.CorrelationId);
                    }
                    finally
                    {
                        _context.CommandQueues[command.Props.ReplyTo].Enqueue(new TestBusCommandMessage(response
                              , new BasicProperties
                                {
                                        CorrelationId = command.Props.CorrelationId
                                      , Type = command.Props.Type
                                }));
                    }
                }
            }).Start();
            _isListening = true;
        }

        public void Dispose()
        {
        }
    }
}