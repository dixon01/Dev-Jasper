// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Program type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace XNA2._0RendererTest
{
    /// <summary>
    /// Enty point
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        public static void Main(string[] args)
        {
            using (var game = new Renderer())
            {
                game.Run();
            }
        }
    }
}
