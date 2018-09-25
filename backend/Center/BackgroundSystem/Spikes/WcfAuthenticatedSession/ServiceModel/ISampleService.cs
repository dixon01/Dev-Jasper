// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISampleService.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ISampleService type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WcfAuthenticatedSession.ServiceModel
{
    using System.Collections.Generic;
    using System.ServiceModel;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines a sample service.
    /// </summary>
    [ServiceContract]
    public interface ISampleService
    {
        /// <summary>
        /// Reads items.
        /// </summary>
        /// <returns>The items.</returns>
        [OperationContract]
        Task<IEnumerable<string>> Read();

        /// <summary>
        /// Writes a value for a specified tenant.
        /// </summary>
        /// <param name="tenantId">The identifier of the tenant.</param>
        /// <param name="value">The value.</param>
        /// <returns>A <see cref="Task"/> that can be awaited.</returns>
        [OperationContract]
        Task Write(int tenantId, string value);
    }
}
