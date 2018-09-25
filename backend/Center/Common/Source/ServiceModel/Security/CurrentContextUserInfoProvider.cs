// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CurrentContextUserInfoProvider.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the CurrentContextUserInfoProvider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.ServiceModel.Security
{
    using System;
    using System.ServiceModel;

    /// <summary>
    /// Defines a component to get the <see cref="UserInfo"/> for the current operation context.
    /// By default, it uses the information provided by the <see cref="OperationContext.Current"/>.
    /// </summary>
    public abstract class CurrentContextUserInfoProvider
    {
        static CurrentContextUserInfoProvider()
        {
            ResetCurrent();
        }

        /// <summary>
        /// Gets the current provider.
        /// </summary>
        public static CurrentContextUserInfoProvider Current { get; private set; }

        /// <summary>
        /// Resets the current provider to the default one.
        /// </summary>
        public static void ResetCurrent()
        {
            SetCurrent(DefaultCurrentContextUserInfoProvider.Instance);
        }

        /// <summary>
        /// Sets the current provider to the provided <paramref name="instance"/>.
        /// </summary>
        /// <param name="instance">The instance to be set as current one.</param>
        public static void SetCurrent(CurrentContextUserInfoProvider instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }

            Current = instance;
        }

        /// <summary>
        /// Gets the <see cref="UserInfo"/> for the current context.
        /// </summary>
        /// <returns>
        /// The <see cref="UserInfo"/> for the current context.
        /// </returns>
        public abstract UserInfo GetUserInfo();

        private sealed class DefaultCurrentContextUserInfoProvider : CurrentContextUserInfoProvider
        {
            static DefaultCurrentContextUserInfoProvider()
            {
                Instance = new DefaultCurrentContextUserInfoProvider();
            }

            public static DefaultCurrentContextUserInfoProvider Instance { get; private set; }

            public override UserInfo GetUserInfo()
            {
                if (OperationContext.Current == null)
                {
                    return new UserInfo();
                }

                var username = OperationContext.Current.ServiceSecurityContext.PrimaryIdentity.Name;
                return new UserInfo(
                    OperationContext.Current.ServiceSecurityContext.PrimaryIdentity.IsAuthenticated,
                    username);
            }
        }
    }
}