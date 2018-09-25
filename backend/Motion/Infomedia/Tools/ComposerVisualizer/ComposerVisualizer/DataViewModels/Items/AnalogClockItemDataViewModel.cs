// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AnalogClockItemDataViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Tools.ComposerVisualizer.DataViewModels.Items
{
    using System.ComponentModel;

    using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

    /// <summary>
    /// Data view model of the analog clock item
    /// </summary>
    [DisplayName(@"Analog Clock Properties")]
    public class AnalogClockItemDataViewModel : DrawableItemBaseDataViewModel
    {
        /// <summary>
        /// Gets or sets the hour hand.
        /// </summary>
        [Category("Hands")]
        [ReadOnly(true)]
        [ExpandableObject]
        [DisplayName(@"Hour")]
        public AnalogClockHandItemDataViewModel Hour { get; set; }

        /// <summary>
        /// Gets or sets the minute hand.
        /// </summary>
        [Category("Hands")]
        [ReadOnly(true)]
        [ExpandableObject]
        [DisplayName(@"Minute")]
        public AnalogClockHandItemDataViewModel Minute { get; set; }

        /// <summary>
        /// Gets or sets the seconds hand.
        /// </summary>
        [Category("Hands")]
        [ReadOnly(true)]
        [ExpandableObject]
        [DisplayName(@"Seconds")]
        public AnalogClockHandItemDataViewModel Seconds { get; set; }
    }
}
