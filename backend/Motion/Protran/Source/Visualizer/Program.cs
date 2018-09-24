// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the main program.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Visualizer
{
    using System;
    using System.Windows.Forms;
    using System.Windows.Forms.VisualStyles;

    using Gorba.Common.SystemManagement.Host;

    /// <summary>
    /// Main class for this application
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            // I apply the visual style only if supported.
            if (VisualStyleRenderer.IsSupported)
            {
                Application.EnableVisualStyles();
            }

            Application.SetCompatibleTextRenderingDefault(false);

            new ApplicationHost<ProtranVisualizerApplication>().Run("ProtranVisualizer");
        }
    }
}
