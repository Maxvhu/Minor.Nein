namespace Minor.Nein.WebScale
{
    using System;
    using Newtonsoft.Json;
    using RabbitMQ.Client;

    public class CommandListener : IDisposable, ICommandListener
    {
        private readonly MethodCommandInfo _methodCommandInfo;
        private ICommandReceiver _receiver;
        public IMicroserviceHost Host { get; private set; }

        public CommandListener(MethodCommandInfo methodCommandInfo)
        {
            _methodCommandInfo = methodCommandInfo;
            QueueName = _methodCommandInfo.QueueName;
        }

        public string QueueName { get; }

        public void DeclareQueue(IBusContext<IConnection> context)
        {
            _receiver = context.CreateCommandReceiver(QueueName);
            _receiver.DeclareCommandQueue();
        }

        public void StartListening(IMicroserviceHost host)
        {
            Host = host;
            _receiver.StartReceivingCommands(Handle);
        }

        public CommandMessage Handle(CommandMessage commandMessage)
        {
            object instance = Host.CreateInstanceOfType(_methodCommandInfo.ClassType);

            object param = JsonConvert.DeserializeObject(commandMessage.Message
                  , _methodCommandInfo.MethodParameter.ParameterType);
            object result = _methodCommandInfo.MethodInfo.Invoke(instance, new[]
                                                                           {
                                                                                   param
                                                                           });
            string resultJson = JsonConvert.SerializeObject(result);
            return new CommandMessage(resultJson, _methodCommandInfo.MethodReturnType.ToString()
                  , commandMessage.CorrelationId);
        }

        public void Dispose()
        {
            _receiver?.Dispose();
        }
    }
}