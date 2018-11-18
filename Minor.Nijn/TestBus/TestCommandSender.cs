﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Minor.Nijn.RabbitMQBus;
using RabbitMQ.Client;
using RabbitMQ.Client.Framing;

namespace Minor.Nijn.TestBus
{
    public class TestCommandSender : ICommandSender
    {
        private readonly TestBusContext _context;
        private readonly string _replyQueueName;

        public readonly ConcurrentDictionary<string, TaskCompletionSource<CommandMessage>> CallbackMapper =
            new ConcurrentDictionary<string, TaskCompletionSource<CommandMessage>>();

        public TestCommandSender(TestBusContext context)
        {
            _context = context;
            _replyQueueName = GenerateRandomQueueName();
            context.DeclareCommandQueue(_replyQueueName);

            new Task(() =>
            {
                var queue = context.CommandQueues[_replyQueueName];

                while (true)
                {
                    if (queue.Count <= 0)
                    {
                        Thread.Sleep(250);
                        continue;
                    }

                    var response = queue.Dequeue();

                    if (!CallbackMapper.TryRemove(response.Props.CorrelationId, out TaskCompletionSource<CommandMessage> tcs))
                        return;
                    var commandResponse = response.Message;
                    tcs.TrySetResult(commandResponse);

                }
            }).Start();

        }
        public Task<CommandMessage> SendCommandAsync(CommandMessage request, string queueName)
        {
            BasicProperties props = new BasicProperties();
            var correlationId = Guid.NewGuid().ToString();
            props.CorrelationId = correlationId;
            props.Type = request.MessageType == null ? "" : request.MessageType;
            props.ReplyTo = _replyQueueName;

            var tcs = new TaskCompletionSource<CommandMessage>();
            CallbackMapper.TryAdd(correlationId, tcs);

            _context.CommandQueues[queueName].Enqueue(new TestBusCommandMessage(request, props));

            return tcs.Task;
        }

        public void Dispose()
        {
        }

        public string GenerateRandomQueueName()
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[30];
            var random = new Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            return new String(stringChars);
        }
    }
}