// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Luminator Technology Group" file="XimpleProtocolFactory.cs">
//   Copyright © 2011-2015 LTG. All rights reserved.
// </copyright>
// <author>Kevin Hartman</author>
// --------------------------------------------------------------------------------------------------------------------
namespace Luminator.Motion.Protran.XimpleProtocol
{
    using System;

    using Gorba.Common.Gioom.Core;
    using Gorba.Common.Gioom.Core.Values;

    /// <summary>The ximple protocol factory.</summary>
    public static class XimpleProtocolFactory
    {
        #region Constants

        /// <summary>The simple port network connection name.</summary>
        public const string SimplePortNetworkConnectionName = "NetworkConnection";

        #endregion

        #region Static Fields

        private static readonly object SimplePortLock = new Object();

        private static SimplePort simplePort;

        #endregion

        #region Public Methods and Operators

        /// <summary>Create a GIOOM SimplePort for use in network connections.</summary>
        /// <param name="name">The name, default sto SimplePortNetworkConnectionName</param>
        /// <returns>The <see cref="SimplePort"/>.</returns>
        public static SimplePort CreateNetworkConnectionSimplePort(string name = SimplePortNetworkConnectionName)
        {
            lock (SimplePortLock)
            {
                if (simplePort == null)
                {
                    simplePort = new SimplePort(name, false, true, new FlagValues(), 0);
                }

                return simplePort;
            }
        }

        #endregion
    }
}