// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Luminator Technology Group">
//   Copyright © 2011-2015 LTG. All rights reserved.
// </copyright>
// <summary>
//   The program
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Luminator.PeripheralDimmer.Console
{
    using Gorba.Common.SystemManagement.Host;

    /// <summary>
    /// The program
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// Main entry point of executable program.
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            new ConsoleApplicationHost<DimmerApplication>(args).Run("Dimmer");
        }
    }    
}
