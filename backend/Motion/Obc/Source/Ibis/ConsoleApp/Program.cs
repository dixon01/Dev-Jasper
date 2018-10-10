// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoleApp
{
    using Gorba.Common.SystemManagement.Host;
    using Gorba.Motion.Obc.Ibis.Core;

    /// <summary>
    /// The main program.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// The main entry point.
        /// </summary>
        /// <param name="args">
        /// The arguments.
        /// </param>
        public static void Main(string[] args)
        {
            new ConsoleApplicationHost<IbisApplication>().Run(IbisApplication.ManagementName);
        }
    }
}
