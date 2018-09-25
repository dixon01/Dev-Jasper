// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RemoteMembershipService.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the RemoteMembershipService type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Core.Membership
{
    using System;
    using System.ServiceModel;
    using System.Threading.Tasks;

    using Gorba.Center.BackgroundSystem.Core.Utility;
    using Gorba.Center.Common.ServiceModel;
    using Gorba.Center.Common.ServiceModel.Exceptions;
    using Gorba.Center.Common.ServiceModel.Membership;

    using NLog;

    /// <summary>
    /// Wrapper for an <see cref="IMembershipService"/> designed for remote services.
    /// </summary>
    [ErrorHandler]
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.Single)]
    public class RemoteMembershipService : IMembershipService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IMembershipService membershipService;

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteMembershipService"/> class.
        /// </summary>
        /// <param name="membershipService">The membership service.</param>
        /// <exception cref="ArgumentNullException">The input service is null.</exception>
        public RemoteMembershipService(IMembershipService membershipService)
        {
            if (membershipService == null)
            {
                throw new ArgumentNullException("membershipService");
            }

            this.membershipService = membershipService;
        }

        /// <summary>
        /// Authenticates a user.
        /// </summary>
        /// <returns>The <see cref="User"/> corresponding to the given login.</returns>
        /// <exception cref="NotAuthenticatedException">The provided credentials are not valid.</exception>
        /// <remarks>The hashing function used for the password is MD5.</remarks>
        public async Task<User> AuthenticateUserAsync()
        {
            try
            {
                return await this.membershipService.AuthenticateUserAsync();
            }
            catch (NotAuthenticatedException)
            {
                throw new FaultException("User not authenticated");
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Error while authenticating user");
                throw new FaultException("User not authenticated");
            }
        }
    }
}