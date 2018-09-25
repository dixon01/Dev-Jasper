// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BusStopList.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the BusStopList type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Edi.Core
{
    using System;
    using System.Collections;

    /// <summary>
    /// The bus stop list.
    /// </summary>
    public class BusStopList : CollectionBase
    {
        /// <summary>
        /// The this.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="BusStop"/>.
        /// </returns>
        public BusStop this[int index]
        {
            get
            {
                return (BusStop)this.List[index];
            }

            set
            {
                this.List[index] = value;
            }
        }

        /// <summary>
        /// Add a bus stop.
        /// </summary>
        /// <param name="value">
        /// The stop.
        /// </param>
        /// <returns>
        /// The index at which the stop was added.
        /// </returns>
        public int Add(BusStop value)
        {
            return this.List.Add(value);
        }

        /// <summary>
        /// Gets the bus stop at a given index.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="BusStop"/>.
        /// </returns>
        public BusStop Get(int index)
        {
            try
            {
                if (List.Count > index)
                {
                    return (BusStop)List[index];
                }
            }
            catch (Exception ex)
            {
            }

            return new BusStop(); // (BusStop)List[List.Count - 1];
        }
    }
}