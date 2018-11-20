﻿namespace Minor.Nein.RabbitMQBus.Test
{
    using System;
    using System.Collections.Generic;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using Nein.Test;
    using RabbitMQ.Client;

    [TestClass]
    public class RabbitMQBusContextTest
    {
        #region CreateMessageSender

        [TestMethod]
        public void CreateMessageSender_ReturnsMessageSenderWithCorrectProperties()
        {
            // Arrange
            var channelMock = new Mock<IModel>(MockBehavior.Strict);
            var connectionMock = new Mock<IConnection>(MockBehavior.Strict);
            connectionMock.Setup(c => c.CreateModel()).Returns(channelMock.Object);

            var context = new RabbitMQBusContext(connectionMock.Object, "TestExchange3");

            // Act
            var sender = (RabbitMQMessageSender) context.CreateMessageSender();

            // Assert
            Assert.AreEqual("TestExchange3", sender.ExchangeName);
            var channel = TestHelper.GetPrivateProperty<IModel>(sender, "Channel");
            Assert.AreEqual(channelMock.Object, channel);
        }

        [TestMethod]
        public void CreateMessageSender_WithBusContextDisposed_ThrowsObjectDisposedException()
        {
            // Arrange
            var connectionMock = new Mock<IConnection>();

            var context = new RabbitMQBusContext(connectionMock.Object, "TestExchange3");
            context.Dispose();

            // Act & Assert
            Assert.ThrowsException<ObjectDisposedException>(() =>
            {
                context.CreateMessageSender();
            });
        }

        #endregion

        #region CreateMessageReceiver

        [TestMethod]
        public void CreateMessageReceiver_ReturnsMessageReceiverWithCorrectProperties()
        {
            // Arrange
            var channelMock = new Mock<IModel>(MockBehavior.Strict);
            var connectionMock = new Mock<IConnection>(MockBehavior.Strict);
            connectionMock.Setup(c => c.CreateModel()).Returns(channelMock.Object);

            var context = new RabbitMQBusContext(connectionMock.Object, "TestExchange4");
            var topicExpressions = new List<string>
                                   {
                                           "topic.expression.a"
                                         , "routing.key.b"
                                   };

            // Act
            var receiver = (RabbitMQMessageReceiver) context.CreateMessageReceiver("TestQueue", topicExpressions);

            // Assert
            Assert.AreEqual("TestExchange4", receiver.ExchangeName);
            Assert.AreEqual("TestQueue", receiver.QueueName);
            Assert.AreEqual(topicExpressions, receiver.TopicExpressions);
            var channel = TestHelper.GetPrivateProperty<IModel>(receiver, "Channel");
            Assert.AreEqual(channelMock.Object, channel);
        }

        [TestMethod]
        public void CreateMessageReceiver_WithBusContextDisposed_ThrowsObjectDisposedException()
        {
            // Arrange
            var connectionMock = new Mock<IConnection>();

            var context = new RabbitMQBusContext(connectionMock.Object, "TestExchange3");
            context.Dispose();

            // Act & Assert
            Assert.ThrowsException<ObjectDisposedException>(() =>
            {
                context.CreateMessageReceiver("TestQueue", new List<string>
                                                           {
                                                                   "topic.expression.a"
                                                                 , "routing.key.b"
                                                           });
            });
        }

        #endregion
    }
}