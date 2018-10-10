using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Microsoft.WindowsAzure;

namespace Library
{
    public class Bootstrapper
    {
        public async Task Bootstrap()
        {
            await this.EnsureTopic("Tenants");
        }

        private async Task EnsureTopic(string topicName)
        {
            var connectionString = CloudConfigurationManager.GetSetting("ServiceBus");
            var namespaceManager = NamespaceManager.CreateFromConnectionString(connectionString);

            if (!(await namespaceManager.TopicExistsAsync(topicName)))
            {
                await namespaceManager.CreateTopicAsync(topicName);
            }

            await Task.FromResult(0);
        }
    }
}