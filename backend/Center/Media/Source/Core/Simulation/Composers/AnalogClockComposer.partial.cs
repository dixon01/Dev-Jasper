// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AnalogClockComposer.partial.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AnalogClockComposer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Simulation.Composers
{
    using System.ComponentModel;

    using Gorba.Center.Media.Core.DataViewModels.Layout;
    using Gorba.Motion.Infomedia.Entities.Screen;

    /// <summary>
    /// Composer handling <see cref="AnalogClockElementDataViewModel"/>.
    /// </summary>
    public partial class AnalogClockComposer
    {
        partial void Initialize()
        {
            this.Item.Hour = this.CreateHand(this.ViewModel.Hour);
            this.Item.Minute = this.CreateHand(this.ViewModel.Minute);
            this.Item.Seconds = this.CreateHand(this.ViewModel.Seconds);

            this.ViewModel.Hour.PropertyChanged += this.HourOnPropertyChanged;
            this.ViewModel.Minute.PropertyChanged += this.MinuteOnPropertyChanged;
            this.ViewModel.Seconds.PropertyChanged += this.SecondsOnPropertyChanged;
        }

        partial void Deinitialize()
        {
            this.ViewModel.Hour.PropertyChanged -= this.HourOnPropertyChanged;
            this.ViewModel.Minute.PropertyChanged -= this.MinuteOnPropertyChanged;
            this.ViewModel.Seconds.PropertyChanged -= this.SecondsOnPropertyChanged;
        }

        private void HourOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.Item.Hour = this.CreateHand(this.ViewModel.Hour);
        }

        private void MinuteOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.Item.Minute = this.CreateHand(this.ViewModel.Minute);
        }

        private void SecondsOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.Item.Seconds = this.CreateHand(this.ViewModel.Seconds);
        }

        private AnalogClockHandItem CreateHand(AnalogClockHandElementDataViewModel handDataViewModel)
        {
            if (!handDataViewModel.Visible.Value)
            {
                return null;
            }

            using (var composer = new AnalogClockHandComposer(this.Context, this.Parent, handDataViewModel))
            {
                return composer.Item;
            }
        }
    }
}
