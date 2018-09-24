namespace Luminator.PeripheralDimmer
{
    using System;

    using NLog;

    using Gorba.Common.Utility.Core;
    
    /// <summary>
    /// Main class of this library
    /// </summary>
    public class Dimmer : IDisposable
    {
        /// <summary>
         /// The management name.
         /// </summary>
        internal static readonly string ManagementName = "Dimmer";
        
        private static readonly Logger Logger = LogHelper.GetLogger<Dimmer>();

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Dimmer()
        {
            
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Implmenets IDisposable.
        /// </summary>
        public void Dispose()
        {
            
        }

        /// <summary>
        /// Starts this Dimmer
        /// </summary>
        public void Start()
        {
            lock (this)
            {
            }
        }

        #endregion
    }
}
