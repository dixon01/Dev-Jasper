// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SplashScreenManager.CF20.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SplashScreenManager type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.SystemManager.Core.SplashScreen
{
    using System.Windows.Forms;

    /// <summary>
    /// Main class managing splash screens.
    /// </summary>
    public partial class SplashScreenManager
    {
        private void StartSplashScreenForm(ApplicationContext context)
        {
            this.StartFormThread();
        }

        private void RunFormThread()
        {
            var form = this.CreateSpashScreenForm();
            form.Load += (sender, args) => form.Visible = false;
            Application.Run(form);
        }
    }
}
