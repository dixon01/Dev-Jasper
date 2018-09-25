// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestService.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The test service.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Host
{
    using System.ServiceModel;

    using ServiceModel;

    /// <summary>
    /// The test service.
    /// </summary>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class TestService : ITestService
    {
        /// <summary>
        /// The get test value.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string GetTestValue()
        {
            return "Access granted";
        }
    }
}
