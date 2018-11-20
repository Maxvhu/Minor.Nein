using System.Collections.Generic;

namespace Minor.Nein.TestBus
{
    public class TestBusQueue
    {
        public Queue<IEventMessage> Queue { get; }
        public IEnumerable<string> TopicExpressions;

        public TestBusQueue(IEnumerable<string> topicExpressions)
        {
            Queue = new Queue<IEventMessage>();
            TopicExpressions = topicExpressions;
        }
    }
}
