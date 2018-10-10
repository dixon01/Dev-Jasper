// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Tools.ComposerVisualizer.Controllers
{
    /// <summary>
    /// Interface to handle control of a controller
    /// </summary>
    public interface IController
    {
        /// <summary>
        /// Starts the controller
        /// </summary>
        void Run();

        /// <summary>
        /// Stops the controller
        /// </summary>
        void Stop();
    }
}
