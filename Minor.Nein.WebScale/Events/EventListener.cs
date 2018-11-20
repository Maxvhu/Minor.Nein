namespace Minor.Nein.WebScale
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Newtonsoft.Json;

    public class EventListener : IDisposable
    {
        public Type Class { get; set; }
        public EventListenerAttribute EventListenerAttribute { get; set; }

        public MicroserviceHost Host { get; private set; }

        public IMessageReceiver MessageReceiver { get; set; }

        public Dictionary<TopicAttribute, List<MethodTopicInfo>> Topics { get; set; }

        public void Dispose()
        {
            MessageReceiver?.Dispose();
        }

        public void Handle(IEventMessage receivedMessage)
        {
            var methodList = Topics.Where(x => IsTopicMatching(x, receivedMessage.RoutingKey)).SelectMany(x => x.Value);

            foreach (MethodTopicInfo method in methodList)
            {
                object instance;

                if (Host != null)
                {
                    instance = Host.CreateInstanceOfType(method.ClassType);
                }
                else
                {
                    if (!method.HasDefaultConstructor)
                    {
                        throw new NoSuitableConstructorException(
                                "Eventlistener could not instantiate the class "
                                + Class
                                + " because no Dependency Injection has been set up and no default constructor was found."
                                + " Consider implementing a default constructor or add Dependency Injection in the MicroServiceHostBuilder by "
                                + "using  the RegisterDependencies((services) => {}) method.");
                    }

                    instance = Activator.CreateInstance(Class);
                }

                if (instance == null)
                {
                    throw new NullReferenceException("Could not make an instance of the class " + Class);
                }

                if (method.MethodParameter == null)
                {
                    StartMethod(instance, method.MethodInfo, null).Start();
                    continue;
                }

                if (method.TopicName == "#" && method.MethodParameter.GetType() == typeof(IEventMessage))
                {
                    StartMethod(instance, method.MethodInfo, new[]
                                                             {
                                                                     receivedMessage
                                                             }).Start();
                    continue;
                }

                object param =
                        JsonConvert.DeserializeObject(receivedMessage.Message, method.MethodParameter.ParameterType);
                StartMethod(instance, method.MethodInfo, new[]
                                                         {
                                                                 param
                                                         }).Start();
            }
        }

        public void StartListening(MicroserviceHost host)
        {
            Host = host;
        }

        public Task StartMethod(object classInstance, MethodInfo method, object[] parameters)
        {
            return new Task(() => method.Invoke(classInstance, parameters));
        }

        private bool IsTopicMatching(KeyValuePair<TopicAttribute, List<MethodTopicInfo>> arg
                                   , string receivedMessageRoutingKey)
        {
            string regexString = arg.Key.TopicPattern.Replace(".", @"\.").Replace("#", ".+").Replace("*", "[^.]*");
            regexString = "^" + regexString + "$";
            var regex = new Regex(regexString);

            return regex.IsMatch(receivedMessageRoutingKey);
        }
    }
}