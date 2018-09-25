// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace TestDirectXRenderer
{
    using System;
    using System.Windows.Forms;

    using Gorba.Common.Medi.Core;
    using Gorba.Common.Medi.Core.Config;

    /// <summary>
    /// The program.
    /// </summary>
    internal static class Program
    {
        #region Methods

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var configurator = new FileConfigurator("medi.config");
            MessageDispatcher.Instance.Configure(configurator);

            using (var form = new Form1())
            {
                form.Show();

                // While the form is still valid, render and process messages
                while (form.Created)
                {
                    form.Render();
                    Application.DoEvents();
                }
            }
        }

        #endregion
    }
}