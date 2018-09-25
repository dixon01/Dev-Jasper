// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JidaApi.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the JidaApi type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kontron.Jida32
{
    using System;

    /// <summary>
    /// Main access to the Kontron JIDA32 API.
    /// </summary>
    public class JidaApi : IDisposable
    {
        /// <summary>
        /// The CPU board class.
        /// </summary>
        public static readonly string BoardClassCpu = "CPU";

        /// <summary>
        /// The VGA board class.
        /// </summary>
        public static readonly string BoardClassVga = "VGA";

        /// <summary>
        /// The I/O board class.
        /// </summary>
        public static readonly string BoardClassIO = "IO";

        private bool initialized;

        /// <summary>
        /// Gets the JIDA API version.
        /// </summary>
        /// <exception cref="JidaException">
        /// if <see cref="Initialize"/> was not successfully called before.
        /// </exception>
        public Version Version
        {
            get
            {
                this.CheckInitialized();
                var version = NativeMethods.JidaDllGetVersion();
                return new Version((version >> 16) & 0xFFFF, version & 0xFFFF);
            }
        }

        /// <summary>
        /// Initializes this API. This method has to be called before any other method.
        /// Don't forget to call <see cref="Dispose"/> once you are finished using this object.
        /// </summary>
        /// <returns>
        /// A flag indicating if the board was successfully initialized.
        /// </returns>
        public bool Initialize()
        {
            var init = NativeMethods.JidaDllInitialize();
            if (!init)
            {
                if (NativeMethods.JidaDllInstall(true))
                {
                    init = NativeMethods.JidaDllInitialize();
                }
            }

            this.initialized = init;
            return this.initialized;
        }

        /// <summary>
        /// Gets the board count for a given class.
        /// </summary>
        /// <param name="boardClass">
        /// The board class.
        /// </param>
        /// <returns>
        /// The number of boards that implement the given class.
        /// </returns>
        /// <exception cref="JidaException">
        /// if <see cref="Initialize"/> was not successfully called before.
        /// </exception>
        public int GetBoardCount(string boardClass)
        {
            this.CheckInitialized();
            return NativeMethods.JidaBoardCount(boardClass, 0);
        }

        /// <summary>
        /// Opens a board with its class and index.
        /// </summary>
        /// <param name="boardClass">
        /// The board class.
        /// </param>
        /// <param name="index">
        /// The index (value between 0 and <see cref="GetBoardCount"/> - 1).
        /// </param>
        /// <returns>
        /// The <see cref="JidaBoard"/>. Please dispose of the returned object
        /// once it's not used anymore.
        /// </returns>
        /// <exception cref="JidaException">
        /// if <see cref="Initialize"/> was not successfully called before.
        /// </exception>
        public JidaBoard OpenBoard(string boardClass, int index)
        {
            this.CheckInitialized();
            var handle = IntPtr.Zero;
            if (!NativeMethods.JidaBoardOpen(boardClass, index, 0, ref handle))
            {
                return null;
            }

            return new JidaBoard(handle);
        }

        /// <summary>
        /// Uninitializes this API.
        /// </summary>
        public void Uninitialize()
        {
            if (this.initialized)
            {
                this.initialized = !NativeMethods.JidaDllUninitialize();
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Uninitialize();
        }

        private void CheckInitialized()
        {
            if (!this.initialized)
            {
                throw new JidaException("DLL was not initialized before calling any other method");
            }
        }
    }
}
