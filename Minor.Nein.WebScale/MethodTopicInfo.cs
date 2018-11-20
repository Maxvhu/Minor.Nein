namespace Minor.Nein.WebScale
{
    using System;
    using System.Reflection;

    public class MethodTopicInfo
    {
        public Type ClassType { get; set; }
        public bool HasDefaultConstructor { get; set; }
        public MethodInfo MethodInfo { get; set; }
        public ParameterInfo MethodParameter { get; set; }

        public string TopicName { get; set; }

        public MethodTopicInfo(Type classType, bool hasDefaultConstructor, string topicName, MethodInfo methodInfo
                             , ParameterInfo methodParameter)
        {
            ClassType = classType;
            HasDefaultConstructor = hasDefaultConstructor;
            TopicName = topicName;
            MethodInfo = methodInfo;
            MethodParameter = methodParameter;
        }
    }
}