namespace VoorbeeldMicroservice
{
    using System;
    using System.Threading;
    using Minor.Nein;
    using Minor.Nein.WebScale;
    using Newtonsoft.Json;

    [EventListener("MVM.TestService.PolisEventListenerQueue")]
    public class PolisEventListener
    {
        private readonly IDataMapper mapper;

        //private readonly IDbContextOptions<PolisContext> _context;
        public PolisEventListener(IDataMapper mapper)
        {
            this.mapper = mapper;
            mapper.Print();
            //_context = context;
        }

        [Command("Testje")]
        public int CommandListner(TestCommand evt)
        {
            Thread.Sleep(5000);
            Console.WriteLine("TestCommand ontvangen:");
            return evt.i * evt.i;
        }

        [Command("Testje2")]
        public int CommandListner2(TestCommand evt)
        {
            Thread.Sleep(5000);
            Console.WriteLine("TestCommand ontvangen:");
            return evt.i * evt.i * evt.i;
        }

        [Topic("MVM.Polisbeheer.PolisToegevoegd")]
        public void Handles(PolisToegevoegdEvent evt)
        {
            Console.WriteLine("Werkt dit?????????");
        }

        [Topic("Test")]
        public void Handles(HenkToegevoegdEvent evt)
        {
            Console.WriteLine("Test Message ontvangen:");
            Console.WriteLine(evt.Test);
        }

        [Topic("#")]
        public void HandlesNew(EventMessage evt)
        {
            Console.WriteLine("Eventmessage voor auditlog Bericht ontvangen met #:");
            Console.WriteLine(evt.Message);
        }

        [Topic("Polisbeheer.*")]
        public void HandlesNot(PolisToegevoegdEvent evt)
        {
            Console.WriteLine("Zou niet ontvangen moeten worden:");
            Console.WriteLine(evt.Message);
        }

        [Topic("MVM.Polisbeheer.*")]
        public void HandlesOld(PolisToegevoegdEvent evt)
        {
            Console.WriteLine("Bericht ontvangen met *:");
            Console.WriteLine(evt.Message);
        }
    }

    [EventListener("MVM.TestService.PolisEventListenerQueue")]
    public class Test
    {
        [Topic("MVM.Polisbeheer.PolisToegevoegd")]
        public void Handles(PolisToegevoegdEvent evt)
        {
            Console.WriteLine("Werkt dit?");
        }
    }

    public class PolisToegevoegdEvent : DomainEvent
    {
        [JsonProperty]
        public string Message { get; set; }

        public PolisToegevoegdEvent(string routingKey) : base(routingKey)
        {
        }
    }

    public class HenkToegevoegdEvent : DomainEvent
    {
        [JsonProperty]
        public string Test { get; set; }

        public HenkToegevoegdEvent(string routingKey) : base(routingKey)
        {
        }
    }
}