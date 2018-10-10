// --------------------------------------------------------------------------------------------------------------------
// <copyright company="" file="">
//   Copyright © 2011-2017 LTG. All rights reserved.
// </copyright>
// <summary>
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Gorba.Motion.SystemManager.Core.Hal
{
    using NLog;

    /// <summary>The Luminator hardware abstraction.</summary>
    public class LuminatorHardwareAbstraction : HardwareAbstractionBase
    {
        #region Static Fields

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the serial number or null if it is unknown.
        /// </summary>
        public override string SerialNumber
        {
            get
            {
                return "1.0.0.0";
            }
        }

        #endregion

        #region Methods

        protected override void DoStart()
        {
            Logger.Debug("DoStart() Enter");           
        }

        protected override void DoStop()
        {
            Logger.Debug("DoStop() Enter");       
        }

        #endregion
    }
}