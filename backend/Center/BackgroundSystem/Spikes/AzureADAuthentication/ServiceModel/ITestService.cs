// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITestService.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ITestService type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ServiceModel
{
    using System.ServiceModel;

    /// <summary>
    /// The TestService interface.
    /// </summary>
    [ServiceContract]
    public interface ITestService
    {
        /// <summary>
        /// The get test value.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        [OperationContract]
        string GetTestValue();
    }
}
