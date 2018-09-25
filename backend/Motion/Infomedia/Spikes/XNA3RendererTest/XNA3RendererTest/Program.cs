// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Program type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace XNA3RendererTest
{
    using Gorba.Common.Medi.Core;
    using Gorba.Common.Medi.Core.Config;

    /// <summary>
    /// Main program
    /// </summary>
    public class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        public static void Main()
        {
            var configurator = new FileConfigurator("medi.config");
            MessageDispatcher.Instance.Configure(configurator);

            var sample = new Renderer();
            sample.Run();
        }
    }
}
