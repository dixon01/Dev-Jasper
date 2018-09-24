// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Main class for listening IBIS telegram from serial port
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace IbisListenerTest
{
    using System;
    using System.Windows.Forms;

    /// <summary>
    /// The program.
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
            Application.Run(new FormMain());
        }
    }
}
