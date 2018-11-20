namespace Minor.Nein.TestBus
{
    using System.Collections.Generic;

    public class TestBusQueue
    {
        public IEnumerable<string> TopicExpressions;
        public Queue<IEventMessage> Queue { get; }

        public TestBusQueue(IEnumerable<string> topicExpressions)
        {
            Queue = new Queue<IEventMessage>();
            TopicExpressions = topicExpressions;
        }
    }
}