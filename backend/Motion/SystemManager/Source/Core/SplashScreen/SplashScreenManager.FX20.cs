// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SplashScreenManager.FX20.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
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
            if (context != null && context.MainForm != null)
            {
                Logger.Debug("Starting splash screen on the main UI thread");
                context.MainForm.BeginInvoke(new MethodInvoker(() => this.CreateSpashScreenForm()));
            }
            else
            {
                this.StartFormThread();
            }
        }

        private void RunFormThread()
        {
            this.CreateSpashScreenForm();
            Application.Run();
        }
    }
}
