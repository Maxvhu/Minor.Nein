namespace Minor.Nein.RabbitMQTest
{
    using System.Collections.Generic;
    using System.Threading;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using RabbitMQBus;

    [TestClass]
    public class RabbitMqIntegration_Test
    {
        private RabbitMQBusContext _context;
        private readonly string _hostname = "192.168.99.100";

        [TestMethod]
        public void SendAndReceiveWithTopicsCorrectly()
        {
            //Arrange
            IEventMessage receivedMessage = null;
            var flag = new AutoResetEvent(false);

            IMessageSender sender = _context.CreateMessageSender();
            IMessageReceiver receiver = _context.CreateMessageReceiver("TestQueue", new List<string>
                                                                                    {
                                                                                            "test"
                                                                                    });
            receiver.DeclareQueue();

            var message = new EventMessage("test", "TestMessage");
            //Act
            sender.SendMessage(message);

            receiver.StartReceivingMessages(eventMessage =>
            {
                receivedMessage = eventMessage;
                flag.Set();
            });

            flag.WaitOne();

            //Assert
            Assert.IsNotNull(receivedMessage);
            Assert.AreEqual("TestMessage", receivedMessage.Message);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _context.Dispose();
        }

        [TestInitialize]
        public void TestInitialize()
        {
            var contextBuilder = new RabbitMQContextBuilder();
            _context = contextBuilder.WithCredentials("guest", "guest").WithAddress(_hostname, 5672)
                                     .WithExchange("TestExchange1").CreateContext();
        }

        [TestMethod]
        public void ThrowsExceptionWhenListeningMultipleTimes()
        {
            //Arrange
            IMessageReceiver receiver = _context.CreateMessageReceiver("TestQueue", new List<string>
                                                                                    {
                                                                                            "test"
                                                                                    });
            receiver.DeclareQueue();

            //Act & Assert
            var exception = Assert.ThrowsException<BusConfigurationException>(() =>
            {
                receiver.StartReceivingMessages(message =>
                {
                });
                receiver.StartReceivingMessages(message =>
                {
                });
            });
            Assert.AreEqual("Can't start listening multiple times", exception.Message);
        }

        [TestMethod]
        public void ThrowsExceptionWithMultipleQueueDeclare()
        {
            //Arrange
            IMessageReceiver receiver = _context.CreateMessageReceiver("TestQueue", new List<string>
                                                                                    {
                                                                                            "test"
                                                                                    });

            //Act & Assert
            var exception = Assert.ThrowsException<BusConfigurationException>(() =>
            {
                receiver.DeclareQueue();
                receiver.DeclareQueue();
            });
            Assert.AreEqual("Can't declare the queue multiple times", exception.Message);
        }
    }
}