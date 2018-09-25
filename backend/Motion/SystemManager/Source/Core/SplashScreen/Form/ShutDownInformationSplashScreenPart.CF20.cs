// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ShutDownInformationSplashScreenPart.CF20.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ShutDownInformationSplashScreenPart type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.SystemManager.Core.SplashScreen.Form
{
    using System;

    using OpenNETCF.IO;

    /// <summary>
    /// Splash screen part that shows information that system is shutting down
    /// and about USB stick if it is connected.
    /// </summary>
    public partial class ShutDownInformationSplashScreenPart
    {
        private bool CheckUsbStickInserted()
        {
            foreach (var drive in DriveInfo.GetDrives())
            {
                // TODO: ugly, but it works in many cases
                if (drive.RootDirectory.FullName.IndexOf("USB", StringComparison.InvariantCultureIgnoreCase) > 0)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
