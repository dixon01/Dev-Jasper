// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ShutDownInformationSplashScreenPart.FX20.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ShutDownInformationSplashScreenPart type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.SystemManager.Core.SplashScreen.Form
{
    using System.IO;

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
                if (drive.DriveType == DriveType.Removable)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
