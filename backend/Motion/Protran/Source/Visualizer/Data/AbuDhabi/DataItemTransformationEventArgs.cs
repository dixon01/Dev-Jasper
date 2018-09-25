// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataItemTransformationEventArgs.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DataItemTransformationEventArgs type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Visualizer.Data.AbuDhabi
{
    using Gorba.Common.Protocols.Isi.Messages;

    /// <summary>
    /// Event arguments for a full transformation (chain) of a
    /// <see cref="DataItem"/> for Abu Dhabi.
    /// </summary>
    public class DataItemTransformationEventArgs : TransformationChainEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataItemTransformationEventArgs"/> class.
        /// </summary>
        /// <param name="dataItem">
        /// The data item.
        /// </param>
        /// <param name="chainName">
        /// The chain name.
        /// </param>
        /// <param name="transformations">
        /// The transformations.
        /// </param>
        public DataItemTransformationEventArgs(
            DataItem dataItem, string chainName, params TransformationInfo[] transformations)
            : base(chainName, transformations)
        {
            this.DataItem = dataItem;
        }

        /// <summary>
        /// Gets the data item associated with this event.
        /// </summary>
        public DataItem DataItem { get; private set; }
    }
}