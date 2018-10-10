// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoginValidatorBase.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Base class for login validators. It provides system specific properties.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Core.Security
{
    using System;
    using System.ComponentModel;
    using System.IdentityModel.Selectors;
    using System.Linq;
    using System.Security;
    using System.ServiceModel;

    using Gorba.Center.BackgroundSystem.Data.Access;
    using Gorba.Center.BackgroundSystem.Data.Model.Membership;
    using Gorba.Center.Common.ServiceModel;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Meta;
    using Gorba.Center.Common.ServiceModel.Faults;
    using Gorba.Center.Common.ServiceModel.Filters.Meta;
    using Gorba.Center.Common.ServiceModel.Settings;

    using NLog;

    /// <summary>
    /// Base class for login validators. It provides system specific properties.
    /// </summary>
    public abstract class LoginValidatorBase : UserNamePasswordValidator
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private MaintenanceModeSettings maintenanceMode;

        private SystemConfigReadableModel systemConfigReadableModel;

        private ISystemConfigChangeTrackingManager systemConfigChangeTrackingManager;

        /// <summary>
        /// When overridden in a derived class, validates the specified username and password.
        /// </summary>
        /// <param name="userName">The username to validate.</param>
        /// <param name="password">The password to validate.</param>
        public sealed override void Validate(string userName, string password)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                throw new SecurityException("Login validation failed");
            }

            // in maintenance mode only the user "admin" can login
            this.CheckMaintenanceMode(userName);

            var databaseUser = this.GetUser(userName);

            try
            {
                this.Validate(databaseUser, password);
            }
            catch (SecurityException e)
            {
                this.UpdateFailedLoginByUser(databaseUser);
                throw e;
            }
        }

        /// <summary>
        /// Validates the specified user with the given password.
        /// </summary>
        /// <param name="databaseUser">
        /// The user from the database.
        /// </param>
        /// <param name="password">
        /// The password.
        /// </param>
        protected abstract void Validate(User databaseUser, string password);

        private void CheckMaintenanceMode(string userName)
        {
            if (this.systemConfigReadableModel == null)
            {
                this.GetInitialSystemConfiguration();
            }

            if (this.maintenanceMode.IsEnabled && !userName.Equals(CommonNames.AdminUsername))
            {
                // see http://stackoverflow.com/questions/1322743/wcf-username-authentication-and-fault-contracts
                var reason = this.maintenanceMode.Reason ?? string.Empty;
                throw new FaultException(reason, new FaultCode(FaultCodes.MaintenanceMode));
            }
        }

        private User GetUser(string userName)
        {
            Logger.Trace("Getting user '{0}' from database", userName);
            using (var userRepository = UserRepositoryFactory.Current.Create())
            {
                var lowerUsername = userName.ToLower();
                var user = userRepository.Query()
                    .SingleOrDefault(
                        u =>
                        u.Username.ToLower() == lowerUsername
                        && u.IsEnabled);
                if (user == null)
                {
                    var securityMessage = string.Format("User {0} not found or not enabled.", userName);
                    throw new SecurityException(securityMessage);
                }

                return user;
            }
        }

        private void UpdateFailedLoginByUser(User databaseUser)
        {
            databaseUser.LastLoginAttempt = DateTime.UtcNow;
            databaseUser.ConsecutiveLoginFailures++;
            this.SetUser(databaseUser);
        }

        private void SetUser(User user)
        {
            Logger.Trace("Setting updateing login information for user '{0}' to database", user.Username);
            using (var userRepository = UserRepositoryFactory.Current.Create())
            {
                try
                {
                    var updatedUser = userRepository.UpdateAsync(user).Result;
                }
                catch (Exception)
                {
                    Logger.Error("Could not update user failed login information.");
                }
            }
        }

        private void GetInitialSystemConfiguration()
        {
            this.systemConfigChangeTrackingManager =
                DependencyResolver.Current.Get<ISystemConfigChangeTrackingManager>();
            if (this.systemConfigChangeTrackingManager == null)
            {
                Logger.Error("Could not get system configuration data service. Maybe still starting.");
                throw new FaultException("System not available.", new FaultCode(FaultCodes.SystemNotAvailable));
            }

            var systemConfigQuery = SystemConfigQuery.Create().IncludeSettings();
            var query = this.systemConfigChangeTrackingManager.QueryAsync(systemConfigQuery).Result;

            this.systemConfigReadableModel = query.LastOrDefault();
            if (this.systemConfigReadableModel == null)
            {
                Logger.Fatal("Could not get system configuration.");
                throw new SecurityException("Login validation failed");
            }

            this.systemConfigReadableModel.PropertyChanged += this.OnSystemConfigurationChanged;
            this.OnSystemConfigurationChanged(null, new PropertyChangedEventArgs("Settings"));
        }

        private void OnSystemConfigurationChanged(
            object sender,
            PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (propertyChangedEventArgs.PropertyName == "Settings")
            {
                var settings = this.systemConfigReadableModel.Settings.Deserialize() as BackgroundSystemSettings;
                if (settings == null)
                {
                    Logger.Fatal("Could not deserialize system configuration.");
                    throw new SecurityException("Login validation failed");
                }

                this.maintenanceMode = settings.MaintenanceMode;
            }
        }
    }
}
