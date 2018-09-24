using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library
{
    using Microsoft.ServiceBus.Messaging;

    public static class ServiceBusExtensions
    {
        public static async Task<TopicClient> GetOrCreateTopic(this ServiceBusConfiguration serviceBusConfiguration)
        {
            throw new NotImplementedException();
        }

        public static async Task<SubscriptionClient> GetOrCreateSubscription(this ServiceBusConfiguration serviceBusConfiguration)
        {
            throw new NotImplementedException();
        }

        public static async Task SendMessage(this ClientServiceBusConfiguration serviceBusConfiguration)
        {
            
        }

        public static async Task ReplayToMessage(this ServiceBusConfiguration serviceBusConfiguration)
        {
            
        }
    }
}
