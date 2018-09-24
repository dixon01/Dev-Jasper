// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Program type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Update.ConsoleApp
{
    using Gorba.Common.SystemManagement.Host;
    using Gorba.Motion.Update.Core;

    /// <summary>
    /// The main application of Update.
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
            new ConsoleApplicationHost<UpdateApplication>(args).Run("Update");
        }
    }
}
