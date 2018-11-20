namespace Minor.Nein.TestBus
{
    using System.Collections.Generic;
    using RabbitMQ.Client;

    public class TestBusContext : IBusContext<IConnection>
    {
        public Dictionary<string, Queue<TestBusCommandMessage>> CommandQueues { get; set; }
        public Dictionary<string, TestBusQueue> TestQueues { get; set; }

        public TestBusContext()
        {
            TestQueues = new Dictionary<string, TestBusQueue>();
            CommandQueues = new Dictionary<string, Queue<TestBusCommandMessage>>();
        }

        public IConnection Connection { get; }
        public string ExchangeName { get; }

        public IMessageSender CreateMessageSender()
        {
            return new TestMessageSender(this);
        }

        public IMessageReceiver CreateMessageReceiver(string queueName, IEnumerable<string> topicExpressions)
        {
            return new TestMessageReceiver(this, queueName, topicExpressions);
        }

        public ICommandSender CreateCommandSender()
        {
            return new TestCommandSender(this);
        }

        public ICommandReceiver CreateCommandReceiver(string queueName)
        {
            return new TestCommandReceiver(this, queueName);
        }

        public void Dispose()
        {
        }

        public void DeclareCommandQueue(string queueName)
        {
            if (!CommandQueues.ContainsKey(queueName))
            {
                CommandQueues[queueName] = new Queue<TestBusCommandMessage>();
            }
        }

        public void DeclareQueue(string queueName, IEnumerable<string> topics)
        {
            if (!TestQueues.ContainsKey(queueName))
            {
                TestQueues[queueName] = new TestBusQueue(topics);
            }
        }
    }
}