// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ShutdownCatcher.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ShutdownCatcher type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.SystemManagement.Client
{
    using System;

    using Gorba.Common.Utility.Core;

    using NLog;

    /// <summary>
    /// Class that catches the shutdown event from Windows (using a dedicated form).
    /// </summary>
    public partial class ShutdownCatcher
    {
        private static readonly Logger Logger = LogHelper.GetLogger<ShutdownCatcher>();

        /// <summary>
        /// Event that is fired when Windows is shutting down.
        /// </summary>
        public event EventHandler ShuttingDown;

        private void RaiseShuttingDown(EventArgs e)
        {
            var handler = this.ShuttingDown;
            if (handler != null)
            {
                try
                {
                    handler(this, e);
                }
                catch (Exception ex)
                {
                    Logger.Warn("Couldn't raise ShuttingDown event {0}", ex.Message);
                }
            }
        }
    }
}