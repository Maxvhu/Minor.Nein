namespace Minor.Nein.TestBus.Test
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using RabbitMQ.Client.Framing;

    [TestClass]
    public class TestCommandReceiverTest
    {
        [TestMethod]
        public void DeclareCommandQueueTest()
        {
            var context = new TestBusContext();
            ICommandReceiver target = context.CreateCommandReceiver("queue");
            target.DeclareCommandQueue();

            Assert.AreEqual(1, context.CommandQueues.Count);
            Assert.AreEqual("queue", context.CommandQueues.First().Key);
        }

        [TestMethod]
        public void DeclaringQueueTwiceThrowsException()
        {
            var context = new TestBusContext();
            ICommandReceiver receiver = context.CreateCommandReceiver("queue");
            receiver.DeclareCommandQueue();

            Assert.ThrowsException<BusConfigurationException>(() => receiver.DeclareCommandQueue());
        }

        [TestMethod]
        public void StartListeningTwiceThrowsException()
        {
            var context = new TestBusContext();
            ICommandReceiver receiver = context.CreateCommandReceiver("queue");
            receiver.DeclareCommandQueue();
            receiver.StartReceivingCommands(cm => cm);
            Assert.ThrowsException<BusConfigurationException>(() => receiver.StartReceivingCommands(cm => cm));
        }

        [TestMethod]
        public void StartReceivingCommandsTest()
        {
            var context = new TestBusContext();
            ICommandReceiver target = context.CreateCommandReceiver("queue");
            target.DeclareCommandQueue();
            context.DeclareCommandQueue("responseQueue");
            var autoReset = new AutoResetEvent(false);

            target.StartReceivingCommands(cm =>
            {
                autoReset.Set();
                return cm;
            });

            context.CommandQueues["queue"].Enqueue(new TestBusCommandMessage(new CommandMessage("message", null)
                  , new BasicProperties
                    {
                            ReplyTo = "responseQueue"
                    }));

            bool succes = autoReset.WaitOne(5000);
            Assert.IsTrue(succes);
            Assert.AreEqual(0, context.CommandQueues["queue"].Count);
            Thread.Sleep(100);
            Assert.AreEqual(1, context.CommandQueues["responseQueue"].Count);
        }

        [TestMethod]
        public async Task TestBusIntegrationTest()
        {
            var context = new TestBusContext();
            ICommandSender sender = context.CreateCommandSender();
            ICommandReceiver receiver = context.CreateCommandReceiver("queue");
            receiver.DeclareCommandQueue();
            receiver.StartReceivingCommands(cm =>
            {
                string message = "message2";
                return new CommandMessage(message, cm.MessageType, cm.CorrelationId);
            });

            var mess = new CommandMessage("message", null);
            CommandMessage result = await sender.SendCommandAsync(mess, "queue");

            Assert.AreEqual("message2", result.Message);
        }

        [TestMethod]
        public void TestCommandReceiverCreateTest()
        {
            var context = new TestBusContext();
            ICommandReceiver target = context.CreateCommandReceiver("queue");

            Assert.AreEqual("queue", target.QueueName);
        }
    }
}