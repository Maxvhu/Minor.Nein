namespace Minor.Nein.TestBus
{
    using System.Text.RegularExpressions;

    public class TestMessageSender : IMessageSender
    {
        public TestBusContext Context { get; }

        public TestMessageSender(TestBusContext context)
        {
            Context = context;
        }

        public void SendMessage(IEventMessage message)
        {
            //casmva.info.bla
            //casmva.#
            string senderExpression = message.RoutingKey ?? "";

            foreach (TestBusQueue testQueue in Context.TestQueues.Values)
            {
                foreach (string topicExpression in testQueue.TopicExpressions)
                {
                    if (IsTopicMatch(topicExpression, senderExpression))
                    {
                        testQueue.Queue.Enqueue(message);
                        break;
                    }
                }
            }
        }

        public void Dispose()
        {
        }

        public static bool IsTopicMatch(string s, string matchWith)
        {
            string regexString = s.Replace(".", @"\.").Replace("#", ".+").Replace("*", "[^.]*");
            regexString = "^" + regexString + "$";
            var regex = new Regex(regexString);

            return regex.IsMatch(matchWith);
        }
    }
}