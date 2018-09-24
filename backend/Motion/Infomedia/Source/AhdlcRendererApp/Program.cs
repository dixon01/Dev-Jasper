// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Program type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.AhdlcRendererApp
{
    using Gorba.Common.SystemManagement.Host;
    using Gorba.Motion.Infomedia.AhdlcRenderer;

    /// <summary>
    /// The main program.
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
            new ConsoleApplicationHost<AhdlcRendererApplication>().Run("AhdlcRenderer");
        }
    }
}
