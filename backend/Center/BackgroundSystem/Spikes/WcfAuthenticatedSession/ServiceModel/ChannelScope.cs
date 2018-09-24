// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChannelScope.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ChannelScope type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WcfAuthenticatedSession.ServiceModel
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines a channel scope.
    /// </summary>
    public class ChannelScope : ISampleService, IDisposable
    {
        private readonly ISampleService channel;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChannelScope"/> class.
        /// </summary>
        /// <param name="channel">
        /// The channel.
        /// </param>
        public ChannelScope(ISampleService channel)
        {
            this.channel = channel;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            // TODO: close/destroy channel
        }

        /// <summary>
        /// Reads items.
        /// </summary>
        /// <returns>
        /// A <see cref="Task"/> that can be awaited.
        /// </returns>
        public async Task<IEnumerable<string>> Read()
        {
            return await this.channel.Read();
        }

        /// <summary>
        /// Writes a value for the specified tenant.
        /// </summary>
        /// <param name="tenantId">
        /// The tenant id.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> that can be awaited.
        /// </returns>
        public async Task Write(int tenantId, string value)
        {
            await this.channel.Write(tenantId, value);
        }
    }
}
