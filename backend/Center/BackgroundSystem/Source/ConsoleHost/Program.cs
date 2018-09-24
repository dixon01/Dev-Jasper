// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Program type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.ConsoleHost
{
    using Gorba.Common.SystemManagement.Host;

    /// <summary>
    /// The program.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// The main entry.
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        internal static void Main(string[] args)
        {
            new ConsoleApplicationHost<HostApplication>().Run("BackgroundSystemConsoleHost");
        }
    }
}