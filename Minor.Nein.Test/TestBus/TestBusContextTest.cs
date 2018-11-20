namespace Minor.Nein.TestBus.Test
{
    using System.Collections.Generic;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class TestBusContextTest
    {
        #region CreateCommandReceiver

        [TestMethod]
        public void CreateCommandReceiver_ReturnsTestCommandReceiver()
        {
            // Arrange
            var context = new TestBusContext();

            // Act
            ICommandReceiver receiver = context.CreateCommandReceiver("TestQueue");

            // Assert
            Assert.IsInstanceOfType(receiver, typeof(TestCommandReceiver));
        }

        #endregion

        #region CreateCommandSender

        [TestMethod]
        public void CreateCommandSender_ReturnsTestCommandSender()
        {
            // Arrange
            var context = new TestBusContext();

            // Act
            ICommandSender sender = context.CreateCommandSender();

            // Assert
            Assert.IsInstanceOfType(sender, typeof(TestCommandSender));
        }

        #endregion

        #region CreateMessageReceiver

        [TestMethod]
        public void CreateMessageReceiver_ReturnsTestMessageReceiver()
        {
            // Arrange
            var context = new TestBusContext();

            // Act
            IMessageReceiver receiver = context.CreateMessageReceiver("TestQueue", new List<string>
                                                                                   {
                                                                                           "test.routing.key"
                                                                                   });

            // Assert
            Assert.IsInstanceOfType(receiver, typeof(TestMessageReceiver));
        }

        #endregion

        #region CreateMessageSender

        [TestMethod]
        public void CreateMessageSender_ReturnsTestMessageSender()
        {
            // Arrange
            var context = new TestBusContext();

            // Act
            IMessageSender sender = context.CreateMessageSender();

            // Assert
            Assert.IsInstanceOfType(sender, typeof(TestMessageSender));
        }

        #endregion

        #region DeclareCommandQueue

        [TestMethod]
        public void DeclareCommandQueue_AddsQueue()
        {
            // Arrange
            var context = new TestBusContext();

            // Act
            context.DeclareCommandQueue("TestQueue");

            // Assert
            Assert.AreEqual(1, context.CommandQueues.Count);
            Assert.IsNotNull(context.CommandQueues["TestQueue"]);
        }

        #endregion

        #region DeclareQueue

        [TestMethod]
        public void DeclareQueue_AddsQueue()
        {
            // Arrange
            var context = new TestBusContext();

            // Act
            context.DeclareQueue("TestQueue", new List<string>
                                              {
                                                      "test.routing.key"
                                              });

            // Assert
            Assert.AreEqual(1, context.TestQueues.Count);
            Assert.IsNotNull(context.TestQueues["TestQueue"]);
        }

        #endregion
    }
}