﻿using RabbitMQ.Client.Framing;

namespace Minor.Nein.TestBus
{
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
