// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AnalogClockHandItem.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AnalogClockHandItem type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Entities.Screen
{
    /// <summary>
    /// Description of a hand of an analog clock.
    /// The <see cref="DrawableItemBase.X"/> and <see cref="DrawableItemBase.Y"/>
    /// properties are relative to the parent.
    /// The <see cref="DrawableItemBase.ZIndex"/> is only relative
    /// to the parent, i.e. defines the order in which the hands are drawn.
    /// </summary>
    public partial class AnalogClockHandItem
    {
        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return string.Format("AnalogClockItem.Hand: \"{0}\"", this.Filename);
        }
    }
}