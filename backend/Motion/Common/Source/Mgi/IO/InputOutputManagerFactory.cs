// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InputOutputManagerFactory.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the InputOutputManager type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Common.Mgi.IO
{
    using Kontron.Jida32;

    using NLog;

    /// <summary>
    /// Factory for <see cref="IInputOutputManager"/> instances.
    /// </summary>
    public static class InputOutputManagerFactory
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Creates a new instance of <see cref="IInputOutputManager"/>
        /// for the hardware this application is running on.
        /// </summary>
        /// <returns>
        /// The <see cref="IInputOutputManager"/>.
        /// </returns>
        public static IInputOutputManager Create()
        {
            InputOutputManagerBase manager = new Pc2InputOutputManager();
            try
            {
                manager.Initialize();

                // the Compact hardware doesn't have the button, so this will throw an exception
                // if we are on that hardware
                manager.Button.Read();
            }
            catch (JidaException ex)
            {
                Logger.Debug(ex, "Couldn't initialize PC2 hardware, trying Compact");
                manager.Dispose();

                manager = new CompactInputOutputManager();
                manager.Initialize();
            }

            Logger.Info("Using {0}", manager.GetType().Name);
            return manager;
        }
    }
}
