namespace Minor.Nein.TestBus.Test
{
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class TestCommandSenderTest
    {
        [TestMethod]
        public void CreateTestCommandSenderDeclaresNewQueue()
        {
            var context = new TestBusContext();
            ICommandSender sender = context.CreateCommandSender();

            Assert.IsInstanceOfType(typeof(TestCommandSender), sender.GetType().BaseType);
            Assert.AreEqual(1, context.CommandQueues.Count);
        }

        [TestMethod]
        public void GenerateRandomQueueNameTest()
        {
            var context = new TestBusContext();
            var sender = (TestCommandSender) context.CreateCommandSender();

            string result = sender.GenerateRandomQueueName();
            Assert.AreEqual(30, result.Length);
        }

        [TestMethod]
        public async Task SendCommandAsync()
        {
            var context = new TestBusContext();
            var sender = (TestCommandSender) context.CreateCommandSender();

            var generatedQueue = context.CommandQueues.First().Value;

            context.DeclareCommandQueue("queue");

            var message = new CommandMessage("message", null);
            var task = sender.SendCommandAsync(message, "queue");
            Assert.AreEqual(1, sender.CallbackMapper.Count);

            generatedQueue.Enqueue(context.CommandQueues["queue"].Dequeue());

            CommandMessage result = await task;

            Assert.AreEqual("message", result.Message);
        }
    }
}