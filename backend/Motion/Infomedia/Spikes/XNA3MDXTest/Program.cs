// ---------------------------public -----------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Program type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace XNA3MDXTest
{
    using Gorba.Common.Medi.Core;
    using Gorba.Common.Medi.Core.Config;

    /// <summary>
    /// The program.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// The main entry point for this application.
        /// </summary>
        /// <param name="args">
        /// The (eventual) arguments required by this application.
        /// </param>
        public static void Main(string[] args)
        {
            var configurator = new FileConfigurator("medi.config");
            MessageDispatcher.Instance.Configure(configurator);

            var renderer = new Renderer();
            renderer.Run();
        }
    }
}
