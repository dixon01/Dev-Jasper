// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Luminator Technology Group" file="PeripheralImageUpdateInfoEventArgs.cs">
//   Copyright © 2011-2016 LTG. All rights reserved.
// </copyright>
// <author>Kevin Hartman</author>
// --------------------------------------------------------------------------------------------------------------------
namespace Luminator.PeripheralProtocol.Core.AudioSwitchPeripheral
{
    using System;

    /// <summary>The peripheral image update info event args.</summary>
    public class PeripheralImageUpdateInfoEventArgs : EventArgs
    {
        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="PeripheralImageUpdateInfoEventArgs"/> class.</summary>
        public PeripheralImageUpdateInfoEventArgs()
        {
            this.Status = string.Empty;
            this.TotalRecords = 0;
            this.Record = 0;
        }

        /// <summary>Initializes a new instance of the <see cref="PeripheralImageUpdateInfoEventArgs"/> class.</summary>
        /// <param name="totalRecords">The total records.</param>
        /// <param name="record">The record.</param>
        public PeripheralImageUpdateInfoEventArgs(int totalRecords, int record)
            : this(string.Empty, totalRecords, record)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="PeripheralImageUpdateInfoEventArgs"/> class.</summary>
        /// <param name="status">The status.</param>
        /// <param name="totalRecords">The total records.</param>
        /// <param name="record">The record.</param>
        public PeripheralImageUpdateInfoEventArgs(string status, int totalRecords, int record)
        {
            this.Status = status;
            this.TotalRecords = totalRecords;
            this.Record = record;
        }

        #endregion

        #region Properties

        private int Record { get; set; }

        private string Status { get; set; }

        private int TotalRecords { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>The to string.</summary>
        /// <returns>The <see cref="string"/>.</returns>
        public override string ToString()
        {
            return string.Format("{0} {1} of {1}", this.Status, this.Record, this.TotalRecords);
        }

        #endregion
    }
}