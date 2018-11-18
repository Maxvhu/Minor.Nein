﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Minor.Nijn
{
    public class CommandMessage
    {
        public string Message { get; }
        public string MessageType { get; }
        public string CorrelationId { get; }

        public byte[] EncodeMessage()
        {
            return Encoding.UTF8.GetBytes(Message);
        }



        public CommandMessage(string message, string messageType, string correlationId)
        {
            Message = message;
            MessageType = messageType;
            CorrelationId = correlationId;
        }
    }
}