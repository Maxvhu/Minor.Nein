namespace Minor.Nein.TestBus
{
    using RabbitMQ.Client.Framing;

    public class TestBusCommandMessage
    {
        public CommandMessage Message { get; set; }
        public BasicProperties Props { get; set; }

        public TestBusCommandMessage(CommandMessage message, BasicProperties props)
        {
            Message = message;
            Props = props;
        }
    }
}