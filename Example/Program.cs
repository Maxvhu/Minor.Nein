namespace VoorbeeldMicroservice
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Console;
    using Minor.Nein.RabbitMQBus;
    using Minor.Nein.WebScale;

    public class Program
    {
        private static void Main(string[] args)
        {
            ILoggerFactory loggerFactory = new LoggerFactory();
            loggerFactory.AddProvider(new ConsoleLoggerProvider((text, logLevel) => logLevel >= LogLevel.Debug, true));

            //192.168.99.100
            RabbitMQContextBuilder connectionBuilder = new RabbitMQContextBuilder()
                                                       .WithExchange("MVM.EventExchange")
                                                       .WithAddress("192.168.99.100", 5672)
                                                       .WithCredentials("guest", "guest");


            using (RabbitMQBusContext context = connectionBuilder.CreateContext())
            {
                MicroserviceHostBuilder builder = new MicroserviceHostBuilder()
                                                  .SetLoggerFactory(loggerFactory).RegisterDependencies(services =>
                                                  {
                                                      services.AddTransient<IDataMapper, SinaasAppelDataMapper>();
                                                  }).WithContext(context).UseConventions();

                using (MicroserviceHost host = builder.CreateHost())
                {
                    host.StartListening();

                    Console.WriteLine("ServiceHost is listening to incoming events...");
                    Console.WriteLine("Press any key to quit.");

                    var publisher = new EventPublisher(context);
                    publisher.Publish(new PolisToegevoegdEvent("MVM.Polisbeheer.PolisToegevoegd")
                                      {
                                              Message = "Hey"
                                      });
                    publisher.Publish(new HenkToegevoegdEvent("Test")
                                      {
                                              Test = "Oi"
                                      });

                    Test(context);
                    Console.ReadKey();
                }
            }
        }

        private static async Task Test(RabbitMQBusContext context)
        {
            var commandPublisher = new CommandPublisher(context, "Testje");
            var testcommand = new TestCommand
                              {
                                      i = 100
                              };

            object result = await commandPublisher.Publish(testcommand);
            Console.WriteLine("result:" + result);
        }
    }
}