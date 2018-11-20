namespace Minor.Nein.WebScale
{
    using System;
    using System.Reflection;

    public class MethodCommandInfo
    {
        public Type ClassType { get; set; }
        public MethodInfo MethodInfo { get; set; }
        public ParameterInfo MethodParameter { get; set; }
        public Type MethodReturnType { get; set; }

        public string QueueName { get; set; }

        public MethodCommandInfo(Type classType, MethodInfo methodInfo, ParameterInfo methodParameter
                               , Type methodReturnType, string queueName)
        {
            ClassType = classType;
            MethodInfo = methodInfo;
            MethodParameter = methodParameter;
            QueueName = queueName;
            MethodReturnType = methodReturnType;
        }
    }
}