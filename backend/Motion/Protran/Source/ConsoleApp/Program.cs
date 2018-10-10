// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   This class is the loader of Protran.dll.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.ConsoleApp
{
    using Gorba.Common.SystemManagement.Host;
    using Gorba.Motion.Protran.Core;

    /// <summary>
    /// This class is the loader of Protran.dll.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// The main application's method.
        /// This starts everything.
        /// </summary>
        /// <param name="args">List of all the required arguments
        /// coming from the command line.</param>
        public static void Main(string[] args)
        {
            new ConsoleApplicationHost<ProtranApplication>().Run("Protran");
        }
    }
}
