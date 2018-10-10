// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Luminator Technology Group" file="PeripheralDataReadyEventArg.cs">
//   Copyright © 2011-2015 LTG. All rights reserved.
// </copyright>
// <author>Kevin Hartman</author>
// <summary>
//   The peripheral data ready event arg.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Luminator.PeripheralDimmer.Models
{
    using System;

    using Luminator.PeripheralDimmer.Interfaces;

    /// <summary>The peripheral data ready event arg.</summary>
    public class PeripheralDataReadyEventArg : EventArgs
    {
        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="PeripheralDataReadyEventArg"/> class.</summary>
        /// <param name="data">The data.</param>
        public PeripheralDataReadyEventArg(byte[] data)
        {
            this.Data = data;
        }

        #endregion

        #region Public Properties

        public PeripheralDataReadyEventArg(byte[] data, IPeripheralBaseMessage message)
        {
            this.Data = data;
            this.Message = message;
        }

        public IPeripheralBaseMessage Message { get; set; }

        /// <summary>Gets or sets the data.</summary>
        public byte[] Data { get; set; }

        #endregion
    }
}