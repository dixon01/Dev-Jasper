// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ControlExtensions.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Declare some extensions for Form class
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Utils
{
    using System.Windows.Forms;

    /// <summary>
    /// Declare some extended control methods
    /// </summary>
    public static class ControlExtensions
    {
        /// <summary>
        /// Enables to change UI oriented properties on the specified control when the method is
        /// called in other thread than the Main UI thread.
        /// Avoid to call InvokedRequires in the code and to setup SyncContext.
        /// Usage :
        /// If you have a method DoSomeThing(int a) {} on a form, call it like this :
        ///     this.UIThread( () => DoSomeThing(int a)); That's it !
        /// </summary>
        /// <param name="control">
        /// The control on which one the method is called.
        /// </param>
        /// <param name="code">
        /// The code to be executed in safe-thread context.
        /// </param>
        public static void UIThread(this Control control, MethodInvoker code)
        {
            if (control.InvokeRequired)
            {
                control.Invoke(code);
                return;
            }

            code.Invoke();
        }
    }
}
