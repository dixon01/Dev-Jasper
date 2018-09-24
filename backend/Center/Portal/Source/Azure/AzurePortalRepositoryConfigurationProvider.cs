// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AzurePortalRepositoryConfigurationProvider.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Provider for the Update repository configuration with values taken from Azure settings.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Portal.Azure
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Gorba.Center.Common.Azure;
    using Gorba.Center.Common.ServiceModel;
    using Gorba.Center.Portal.Host.Configuration;
    using Gorba.Common.Update.ServiceModel.Repository;
    using Gorba.Common.Utility.Core;

    using Microsoft.WindowsAzure.ServiceRuntime;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Blob;

    using NLog;

    /// <summary>
    /// Provider for the Update repository configuration with values taken from Azure settings.
    /// </summary>
    public class AzurePortalRepositoryConfigurationProvider : PortalRepositoryConfigurationProvider
    {
        private const string SharedLogsAccessPolicyFormat = "feedbackstoragepolicy{0}";

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly AsyncLock locker = new AsyncLock();

        private RepositoryConfig repositoryConfig;

        private DateTime expiryTime;

        private int policyCounter;

        /// <summary>
        /// Gets the Update repository configuration defined in the Azure settings.
        /// </summary>
        /// <returns>
        /// The task returning the <see cref="RepositoryConfig"/>.
        /// </returns>
        public override async Task<RepositoryConfig> GetRepositoryConfigurationAsync()
        {
            using (await this.locker.LockAsync())
            {
                if (this.repositoryConfig == null || this.expiryTime < TimeProvider.Current.UtcNow.AddHours(1))
                {
                    this.UpdateRepositoryXml();
                }
            }

            return this.repositoryConfig;
        }

        /// <summary>
        /// Updates/Renews the shared access policy on the logs container and creates a new repository configuration
        /// </summary>
        private void UpdateRepositoryXml()
        {
            Logger.Debug("Recreating repository configuration from role environment");
            var connectionString = RoleEnvironment.GetConfigurationSettingValue(
                    PredefinedAzureItems.Settings.StorageConnectionString);
            var storageAccount = CloudStorageAccount.Parse(connectionString);

            var blobClient = storageAccount.CreateCloudBlobClient();
            var resourcesContainer = blobClient.GetContainerReference("resources");
            resourcesContainer.CreateIfNotExists();

            var logsContainer = blobClient.GetContainerReference("logs");
            logsContainer.CreateIfNotExists();

            var policyName = this.CreateSharedAccessPolicy(logsContainer);

            // use the URI string for the container, including the SAS token
            var feedbackDirectory = logsContainer.Uri + logsContainer.GetSharedAccessSignature(null, policyName);
            var versionConfiguration = new RepositoryVersionConfig
                                           {
                                               CommandsDirectory = string.Empty,
                                               ResourceDirectory = resourcesContainer.Uri.AbsoluteUri,
                                               FeedbackDirectory = feedbackDirectory,
                                           };
            var configuration = new RepositoryConfig();
            configuration.Versions.Add(versionConfiguration);
            this.repositoryConfig = configuration;
        }

        private string CreateSharedAccessPolicy(CloudBlobContainer container)
        {
            var now = TimeProvider.Current.UtcNow;
            this.expiryTime = now.AddDays(1);

            var sharedPolicy = new SharedAccessBlobPolicy
                                   {
                                       SharedAccessStartTime = now,
                                       SharedAccessExpiryTime = this.expiryTime,
                                       Permissions =
                                           SharedAccessBlobPermissions.Write
                                           | SharedAccessBlobPermissions.List
                                           | SharedAccessBlobPermissions.Read
                                   };

            var permissions = container.GetPermissions();
            foreach (var policy in
                permissions.SharedAccessPolicies.Where(p => p.Value.SharedAccessExpiryTime <= now).ToList())
            {
                // Remove expired policies
                permissions.SharedAccessPolicies.Remove(policy.Key);
            }

            var policyName = string.Format(SharedLogsAccessPolicyFormat, this.policyCounter++);
            permissions.SharedAccessPolicies.Add(policyName, sharedPolicy);
            container.SetPermissions(permissions);
            return policyName;
        }
    }
}
