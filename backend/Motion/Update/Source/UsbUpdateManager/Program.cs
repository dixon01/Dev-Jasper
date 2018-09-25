// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Program type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Update.UsbUpdateManager
{
    using System;
    using System.Windows.Forms;

    using Gorba.Common.Utility.Compatibility.Container;
    using Gorba.Motion.Update.UsbUpdateManager.Controls;

    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// The main application.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var serviceContainer = new ServiceContainer();
            ServiceLocator.SetLocatorProvider(() => new ServiceContainerLocator(serviceContainer));
            serviceContainer.RegisterInstance<IProjectManager>(new ProjectManager());

            Application.Run(new MainForm());
        }
    }
}
