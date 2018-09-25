// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MembershipService.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MembershipService type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Core.Membership
{
    using System;
    using System.Data.Entity;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.Threading.Tasks;

    using Gorba.Center.BackgroundSystem.Data.Access;
    using Gorba.Center.BackgroundSystem.Data.Model;
    using Gorba.Center.Common.ServiceModel;
    using Gorba.Center.Common.ServiceModel.Exceptions;
    using Gorba.Center.Common.ServiceModel.Membership;
    using Gorba.Center.Common.ServiceModel.Security;

    using NLog;

    /// <summary>
    /// Implements the <see cref="IMembershipService"/> using the <see cref="ResourceRepository"/>.
    /// </summary>
    public class MembershipService : IMembershipService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Authenticates a user.
        /// </summary>
        /// <returns>The <see cref="User"/> corresponding to the given login.</returns>
        /// <exception cref="NotAuthenticatedException">The provided credentials are not valid.</exception>
        /// <remarks>The hashing function used for the password is MD5.</remarks>
        public async Task<User> AuthenticateUserAsync()
        {
            var userInfo = CurrentContextUserInfoProvider.Current.GetUserInfo();
            var requestAddress = GetRequestAddress();
            if (!userInfo.IsAuthenticated)
            {
                throw new NotAuthenticatedException(userInfo.Username, requestAddress);
            }

            User user = null;
            using (var repositoryContext = UserRepositoryFactory.Current.Create())
            {
                var databaseUser =
                    await repositoryContext.Query().SingleOrDefaultAsync(u => u.Username == userInfo.Username);
                if (databaseUser == null)
                {
                    Logger.Debug(
                        "Authentication failed (user not found). Login: '{0}', Request address: '{1}'",
                        userInfo,
                        requestAddress);
                }
                else
                {
                    databaseUser.ConsecutiveLoginFailures = 0;
                    databaseUser.LastLoginAttempt = DateTime.UtcNow;
                    databaseUser.LastSuccessfulLogin = DateTime.UtcNow;
                    databaseUser = await repositoryContext.UpdateAsync(databaseUser);
                    user = databaseUser.ToDto();
                }
            }

            if (user == null)
            {
                throw new NotAuthenticatedException(userInfo.Username, requestAddress);
            }

            Logger.Trace("User '{0}' successfully authenticated. Request address: '{1}'", userInfo, requestAddress);
            return user;
        }

        private static string GetRequestAddress()
        {
            if (OperationContext.Current == null)
            {
                return "local";
            }

            var properties = OperationContext.Current.IncomingMessageProperties;
            var endpoint = properties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
            if (endpoint == null)
            {
                return "Information not found";
            }

            return endpoint.Address;
        }
    }
}