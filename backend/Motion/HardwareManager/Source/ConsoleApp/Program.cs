// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Program type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.HardwareManager.ConsoleApp
{
    using Gorba.Common.SystemManagement.Host;
    using Gorba.Motion.HardwareManager.Core;

    /// <summary>
    /// The main application of Hardware Manager.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// The main function.
        /// </summary>
        /// <param name="args">
        /// The command line arguments.
        /// </param>
        public static void Main(string[] args)
        {
            new ConsoleApplicationHost<HardwareManagerApplication>().Run("HardwareManager");
        }
    }
}
