// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMembershipService.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IMembershipService type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.ServiceModel
{
    using System.ServiceModel;
    using System.Threading.Tasks;

    using Gorba.Center.Common.ServiceModel.Exceptions;
    using Gorba.Center.Common.ServiceModel.Faults;
    using Gorba.Center.Common.ServiceModel.Membership;

    /// <summary>
    /// Defines the service for authentication and authorization.
    /// </summary>
    [ServiceContract]
    public interface IMembershipService
    {
        /// <summary>
        /// Authenticates a user.
        /// </summary>
        /// <returns>
        /// The <see cref="User"/> corresponding to login information provided through the WCF call.
        /// </returns>
        /// <exception cref="NotAuthenticatedException">The provided credentials are not valid.</exception>
        /// <remarks>The hashing function used for the password is MD5.</remarks>
        [OperationContract]
        [FaultContract(typeof(NotAuthenticatedFaultDetails))]
        Task<User> AuthenticateUserAsync();
    }
}