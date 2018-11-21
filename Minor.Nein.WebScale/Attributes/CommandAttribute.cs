namespace Minor.Nein.WebScale.Attributes
{
    using System;

    [AttributeUsage(AttributeTargets.Method)]
    public class CommandAttribute : Attribute
    {
        public string Queuename { get; }

        public CommandAttribute(string queuename)
        {
            Queuename = queuename;
        }
    }
}