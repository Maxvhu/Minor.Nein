namespace Minor.Nein.TestBus.Test
{
    using System.Collections.Generic;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class TestMessageSender_Test
    {
        [TestMethod]
        public void IsTopicMatchTest()
        {
            Assert.IsTrue(TestMessageSender.IsTopicMatch("Minor.Nein.BerichtVerstuurd", "Minor.Nein.BerichtVerstuurd"));
            Assert.IsTrue(TestMessageSender.IsTopicMatch("Minor.Nein.*", "Minor.Nein.BerichtVerstuurd"));
            Assert.IsTrue(TestMessageSender.IsTopicMatch("Minor.*.*", "Minor.Nein.BerichtVerstuurd"));
            Assert.IsTrue(TestMessageSender.IsTopicMatch("*.*.*", "Minor.Nein.BerichtVerstuurd"));
            Assert.IsTrue(TestMessageSender.IsTopicMatch("Minor.Nein.#", "Minor.Nein.BerichtVerstuurd"));
            Assert.IsTrue(TestMessageSender.IsTopicMatch("Minor.#", "Minor.Nein.BerichtVerstuurd"));
            Assert.IsTrue(TestMessageSender.IsTopicMatch("#", "Minor.Nein.BerichtVerstuurd"));
            Assert.IsTrue(TestMessageSender.IsTopicMatch("#.BerichtVerstuurd", "Minor.Nein.BerichtVerstuurd"));
            Assert.IsTrue(TestMessageSender.IsTopicMatch("Minor.*.BerichtVerstuurd", "Minor.Nein.BerichtVerstuurd"));

            Assert.IsFalse(TestMessageSender.IsTopicMatch("Minor.Nein.BerichtVerstuurd.2"
                  , "Minor.Nein.BerichtVerstuurd"));
            Assert.IsFalse(TestMessageSender.IsTopicMatch("Minor.Nein", "Minor.Nein.BerichtVerstuurd"));
            Assert.IsFalse(TestMessageSender.IsTopicMatch("Minor", "Minor.Nein.BerichtVerstuurd"));
            Assert.IsFalse(TestMessageSender.IsTopicMatch("Mva.Minor.Nein.BerichtVerstuurd"
                  , "Minor.Nein.BerichtVerstuurd"));
        }

        [TestMethod]
        public void MultipleMessagesAddToQueue()
        {
            var context = new TestBusContext();

            IMessageSender sender = context.CreateMessageSender();
            context.DeclareQueue("receiver1", new List<string>
                                              {
                                                      "receiver.info"
                                              });
            var message = new EventMessage("receiver.info", "receiver");

            sender.SendMessage(message);
            sender.SendMessage(message);
            sender.SendMessage(message);

            Assert.AreEqual(3, context.TestQueues["receiver1"].Queue.Count);
        }

        [TestMethod]
        public void SendMessageWithCorrectTopicAddsToQueue()
        {
            var context = new TestBusContext();

            IMessageSender sender = context.CreateMessageSender();
            context.DeclareQueue("receiver1", new List<string>
                                              {
                                                      "receiver.info"
                                              });
            context.DeclareQueue("receiver2", new List<string>
                                              {
                                                      "receiver.*.info"
                                              });

            var message = new EventMessage("receiver.info", "receiver");
            sender.SendMessage(message);

            Assert.AreEqual(1, context.TestQueues["receiver1"].Queue.Count);
            Assert.AreEqual(0, context.TestQueues["receiver2"].Queue.Count);
        }
    }
}