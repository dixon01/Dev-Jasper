namespace Library
{
    public class ServiceBusConfiguration
    {
        public string TopicName { get; set; }

        public string SubscriptionName { get; set; }
    }

    public class ClientServiceBusConfiguration : ServiceBusConfiguration
    {
        public string SessionId { get; set; }
    }
}