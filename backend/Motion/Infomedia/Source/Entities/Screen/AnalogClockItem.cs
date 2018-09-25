// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AnalogClockItem.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   An analog clock with configurable an hour, a minute and an optional seconds hand.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Entities.Screen
{
    using System.Collections.Generic;

    using Gorba.Common.Medi.Core.Management;

    /// <summary>
    /// An analog clock with configurable an hour, a minute and an optional seconds hand.
    /// </summary>
    public partial class AnalogClockItem : IManageable
    {
        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public override object Clone()
        {
            var clone = (AnalogClockItem)base.Clone();
            clone.Hour = this.Hour == null ? null : (AnalogClockHandItem)this.Hour.Clone();
            clone.Minute = this.Minute == null ? null : (AnalogClockHandItem)this.Minute.Clone();
            clone.Seconds = this.Seconds == null ? null : (AnalogClockHandItem)this.Seconds.Clone();
            return clone;
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return string.Format(
                "AnalogClockItem: H:{0}, M:{1}, S:{2} @ [{3},{4}]",
                this.Hour,
                this.Minute,
                this.Seconds,
                this.X,
                this.Y);
        }

        IEnumerable<IManagementProvider> IManageable.GetChildren(IManagementProvider parent)
        {
            yield return parent.Factory.CreateManagementProvider("Hour", parent, this.Hour);
            yield return parent.Factory.CreateManagementProvider("Minute", parent, this.Minute);
            yield return parent.Factory.CreateManagementProvider("Seconds", parent, this.Seconds);
        }
    }
}