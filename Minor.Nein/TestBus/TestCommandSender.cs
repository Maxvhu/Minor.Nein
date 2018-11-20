namespace Minor.Nein.TestBus
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading;
    using System.Threading.Tasks;
    using RabbitMQ.Client.Framing;

    public class TestCommandSender : ICommandSender
    {
        public readonly ConcurrentDictionary<string, TaskCompletionSource<CommandMessage>> CallbackMapper =
                new ConcurrentDictionary<string, TaskCompletionSource<CommandMessage>>();

        private readonly string _replyQueueName;
        private TestBusContext Context { get; }

        public TestCommandSender(TestBusContext context)
        {
            Context = context;
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

                    TestBusCommandMessage response = queue.Dequeue();

                    if (!CallbackMapper.TryRemove(response.Props.CorrelationId, out var tcs))
                    {
                        return;
                    }

                    CommandMessage commandResponse = response.Message;
                    tcs.TrySetResult(commandResponse);
                }
            }).Start();
        }

        public Task<CommandMessage> SendCommandAsync(CommandMessage request, string queueName)
        {
            var props = new BasicProperties();
            string correlationId = Guid.NewGuid().ToString();
            props.CorrelationId = correlationId;
            props.Type = request.MessageType == null ? "" : request.MessageType;
            props.ReplyTo = _replyQueueName;

            var tcs = new TaskCompletionSource<CommandMessage>();
            CallbackMapper.TryAdd(correlationId, tcs);

            Context.CommandQueues[queueName].Enqueue(new TestBusCommandMessage(request, props));

            return tcs.Task;
        }

        public void Dispose()
        {
        }

        public string GenerateRandomQueueName()
        {
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[30];
            var random = new Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            return new string(stringChars);
        }
    }
}