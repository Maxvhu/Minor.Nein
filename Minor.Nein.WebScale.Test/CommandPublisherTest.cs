namespace Minor.Nein.WebScale.Test
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using Newtonsoft.Json;
    using RabbitMQ.Client;

    [TestClass]
    public class CommandPublisherTest
    {
        [TestMethod]
        public async Task CommandPublisherPublishExceptionTest()
        {
            var sender = new Mock<ICommandSender>(MockBehavior.Strict);
            var responseCommand = new TestCommand
                                  {
                                          Message = "message2"
                                  };

            sender.Setup(m => m.SendCommandAsync(It.IsAny<CommandMessage>(), "queue")).Returns(
                    Task.FromResult(new CommandMessage("error", typeof(ArgumentException).FullName, null)));

            var iBusContextMock = new Mock<IBusContext<IConnection>>(MockBehavior.Strict);
            iBusContextMock.Setup(m => m.CreateCommandSender()).Returns(sender.Object);

            var target = new CommandPublisher(iBusContextMock.Object, "queue");
            var testCommand = new TestCommand();

            ArgumentException exception = await Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
            {
                await target.Publish<TestCommand>(testCommand);
            });
            Assert.AreEqual("error", exception.Message);
        }

        [TestMethod]
        public async Task CommandPublisherPublishTest()
        {
            var sender = new Mock<ICommandSender>(MockBehavior.Strict);
            var responseCommand = new TestCommand
                                  {
                                          Message = "message2"
                                  };

            sender.Setup(m => m.SendCommandAsync(It.IsAny<CommandMessage>(), "queue")).Returns(
                    Task.FromResult(new CommandMessage(JsonConvert.SerializeObject(responseCommand)
                          , typeof(TestCommand).FullName, null)));

            var iBusContextMock = new Mock<IBusContext<IConnection>>(MockBehavior.Strict);
            iBusContextMock.Setup(m => m.CreateCommandSender()).Returns(sender.Object);

            var target = new CommandPublisher(iBusContextMock.Object, "queue");
            var testCommand = new TestCommand();

            TestCommand result = await target.Publish<TestCommand>(testCommand);

            Assert.IsInstanceOfType(result, typeof(TestCommand));
            Assert.AreEqual("message2", result.Message);
        }

        [TestMethod]
        public async Task CommandPublisherPublishUnknownExceptionThrowsInvalidCastException()
        {
            var sender = new Mock<ICommandSender>(MockBehavior.Strict);
            var responseCommand = new TestCommand
                                  {
                                          Message = "message2"
                                  };

            sender.Setup(m => m.SendCommandAsync(It.IsAny<CommandMessage>(), "queue")).Returns(
                    Task.FromResult(new CommandMessage("error", "RandomException", null)));

            var iBusContextMock = new Mock<IBusContext<IConnection>>(MockBehavior.Strict);
            iBusContextMock.Setup(m => m.CreateCommandSender()).Returns(sender.Object);

            var target = new CommandPublisher(iBusContextMock.Object, "queue");
            var testCommand = new TestCommand();

            InvalidCastException exception = await Assert.ThrowsExceptionAsync<InvalidCastException>(async () =>
            {
                await target.Publish<TestCommand>(testCommand);
            });
            Assert.AreEqual("an unknown exception occured (message error), exception type was RandomException"
                  , exception.Message);
        }
    }
}