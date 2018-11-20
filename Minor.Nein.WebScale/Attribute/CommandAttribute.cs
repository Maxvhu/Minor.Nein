using System;

namespace Minor.Nein.WebScale
{
    [AttributeUsage(AttributeTargets.Method)]
    public class CommandAttribute : Attribute
    {
        public String Queuename { get; }

        public CommandAttribute(string queuename)
        {
            Queuename = queuename;
        }
    }
}
