// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SampleService.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SampleService type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WcfAuthenticatedSession.TestServer
{
    using System;
    using System.Collections.Generic;
    using System.ServiceModel;
    using System.Threading.Tasks;

    using WcfAuthenticatedSession.ServiceModel;

    /// <summary>
    /// Defines the sample service.
    /// </summary>
    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    public class SampleService : ISampleService
    {
        private readonly List<Tuple<int, string>> values = new List<Tuple<int, string>>();

        /// <summary>
        /// Reads the values.
        /// </summary>
        /// <returns>The values.</returns>
        public async Task<IEnumerable<string>> Read()
        {
            await Task.FromResult(0);
            throw new NotImplementedException();
        }

        /// <summary>
        /// Writes a value.
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
            await Task.FromResult(0);
            this.values.Add(new Tuple<int, string>(tenantId, value));
            Console.WriteLine("Added {0}, '{1}'", tenantId, value);
        }
    }
}
