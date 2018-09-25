

namespace Luminator.DimmerPeripheral.Core
{
    using System;

    using Luminator.DimmerPeripheral.Core.Interfaces;
    
    [Obsolete("See Luminator.PeripheralDimmer")]
    public class DimmerPeripheralDataReadyEventArg
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DimmerPeripheralDataReadyEventArg"/> class.
        /// </summary>
        /// <param name="data">The data.</param>
        public DimmerPeripheralDataReadyEventArg(byte[] data)
        {
            this.Data = data;
        }

        #endregion

        #region Public Properties

        public DimmerPeripheralDataReadyEventArg(byte[] data, IDimmerPeripheralBaseMessage message)
        {
            this.Data = data;
            this.Message = message;
        }

        public IDimmerPeripheralBaseMessage Message { get; set; }

        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        public byte[] Data { get; set; }

        #endregion
    }
}