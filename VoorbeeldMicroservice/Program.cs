﻿using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Minor.Nijn;
using Minor.Nijn.RabbitMQBus;
using Minor.Nijn.WebScale;
using RabbitMQ.Client;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Minor.Nijn.TestBus;
using Newtonsoft.Json;

namespace VoorbeeldMicroservice
{
    public class Program
    {
        static void Main(string[] args)
        {
            ILoggerFactory loggerFactory = new LoggerFactory();
            loggerFactory.AddProvider(
                new ConsoleLoggerProvider(
                    (text, logLevel) => logLevel >= LogLevel.Debug, true));

            //192.168.99.100
            var connectionBuilder = new RabbitMQContextBuilder()
                    .WithExchange("MVM.EventExchange")
                    .WithAddress("192.168.99.100", 5672)
                    .WithCredentials(userName: "guest", password: "guest");

            
            using (var context = connectionBuilder.CreateContext())
            {
                var builder = new MicroserviceHostBuilder()
                    .SetLoggerFactory(loggerFactory)
                    .RegisterDependencies((services) =>
                        {
                            services.AddTransient<IDataMapper, SinaasAppelDataMapper>();
                        })
                        .WithContext(context)
                        .UseConventions();


                using (var host = builder.CreateHost())
                {

                    host.StartListening();

                    Console.WriteLine("ServiceHost is listening to incoming events...");
                    Console.WriteLine("Press any key to quit.");

                    var publisher = new EventPublisher(context);
                    publisher.Publish(new PolisToegevoegdEvent("MVM.Polisbeheer.PolisToegevoegd") { Message = "Hey" });
                    publisher.Publish(new HenkToegevoegdEvent("Test") { Test = "Oi" });

                    Test(context);
                    Console.ReadKey();
                }
            }
        }

        private static async Task Test(IBusContext<IConnection> context)
        {
            CommandPublisher commandPublisher = new CommandPublisher(context, "Testje");
            var testcommand = new TestCommand(){i = 100, ConvertResponseToType = typeof(int)};

            try
            {
                var result = await commandPublisher.Publish(testcommand);
            Console.WriteLine("result:" + result.ToString());

            }
            catch(ArgumentException e )
            {
                Console.WriteLine("Argumentexceptie");
            }
            
        }
    }
}