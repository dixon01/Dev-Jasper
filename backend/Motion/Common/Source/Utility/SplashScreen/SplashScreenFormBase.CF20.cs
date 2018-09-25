// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SplashScreenFormBase.CF20.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Common.Utility.SplashScreen
{
    /// <summary>
    /// The splash screen form base.
    /// </summary>
    public partial class SplashScreenFormBase
    {
        /// <summary>
        /// Shows the form.
        /// </summary>
        public virtual void ShowForm()
        {
            this.Show();
        }

        /// <summary>
        /// Hides the form.
        /// </summary>
        public virtual void HideForm()
        {
            this.Hide();
        }

        /// <summary>
        /// Brings this window forcefully to focus.
        /// </summary>
        public virtual void ForceFocus()
        {
        }
    }
}
