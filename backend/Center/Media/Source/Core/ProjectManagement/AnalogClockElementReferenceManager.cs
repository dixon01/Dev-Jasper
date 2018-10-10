// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AnalogClockElementReferenceManager.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Gorba.Center.Media.Core.ProjectManagement
{
    using Gorba.Center.Media.Core.DataViewModels.Layout;

    /// <summary>
    /// The analog clock element reference manager.
    /// </summary>
    public class AnalogClockElementReferenceManager : ElementReferenceManagerBase<AnalogClockElementDataViewModel>
    {
        /// <summary>
        /// Sets the references for the given item.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        public override void SetReferences(AnalogClockElementDataViewModel item)
        {
            if (!item.Visible.Value && item.Visible.Formula == null)
            {
                this.UnsetReferences(item);
                return;
            }

            this.SetReferenceHourFilename(item);
            this.SetReferenceMinuteFilename(item);
            this.SetReferenceSecondsFilename(item);
        }

        /// <summary>
        /// Unsets the references for the given item.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        public override void UnsetReferences(AnalogClockElementDataViewModel item)
        {
            this.UnsetReferenceSecondsFilename(item);
            this.UnsetReferenceMinuteFilename(item);
            this.UnsetReferenceHourFilename(item);
        }

        /// <summary>
        /// Adds the reference for the HourFilename property.
        /// </summary>
        /// <param name="analogClock">
        /// The analog clock element.
        /// </param>
        private void SetReferenceHourFilename(AnalogClockElementDataViewModel analogClock)
        {
            if (analogClock.HourFilename == null || string.IsNullOrEmpty(analogClock.HourFilename.Value))
            {
                return;
            }

            var resource = this.GetResource(analogClock.HourFilename.Value);
            if (resource == null)
            {
                return;
            }

            resource.SetReference(analogClock, "HourFilename");
            resource.UpdateIsUsedVisible();
        }

        /// <summary>
        /// Removes the reference for the HourFilename property.
        /// </summary>
        /// <param name="analogClock">
        /// The analog clock element.
        /// </param>
        private void UnsetReferenceHourFilename(AnalogClockElementDataViewModel analogClock)
        {
            if (analogClock.HourFilename == null || string.IsNullOrEmpty(analogClock.HourFilename.Value))
            {
                return;
            }

            var resource = this.GetResource(analogClock.HourFilename.Value);
            if (resource == null)
            {
                return;
            }

            resource.UnsetReference(analogClock, "HourFilename");
            resource.UpdateIsUsedVisible();
        }

        /// <summary>
        /// Adds the reference for the MinuteFilename property.
        /// </summary>
        /// <param name="analogClock">
        /// The analog clock element.
        /// </param>
        private void SetReferenceMinuteFilename(AnalogClockElementDataViewModel analogClock)
        {
            if (analogClock.MinuteFilename == null || string.IsNullOrEmpty(analogClock.MinuteFilename.Value))
            {
                return;
            }

            var resource = this.GetResource(analogClock.MinuteFilename.Value);
            if (resource == null)
            {
                return;
            }

            resource.SetReference(analogClock, "MinuteFilename");
            resource.UpdateIsUsedVisible();
        }

        /// <summary>
        /// Removes the reference for the MinuteFilename property.
        /// </summary>
        /// <param name="audio">
        /// The analog clock element.
        /// </param>
        private void UnsetReferenceMinuteFilename(AnalogClockElementDataViewModel audio)
        {
            if (audio.MinuteFilename == null || string.IsNullOrEmpty(audio.MinuteFilename.Value))
            {
                return;
            }

            var resource = this.GetResource(audio.MinuteFilename.Value);
            if (resource == null)
            {
                return;
            }

            resource.UnsetReference(audio, "MinuteFilename");
            resource.UpdateIsUsedVisible();
        }

        /// <summary>
        /// Adds the reference for the SecondsFilename property.
        /// </summary>
        /// <param name="audio">
        /// The analog clock element.
        /// </param>
        private void SetReferenceSecondsFilename(AnalogClockElementDataViewModel audio)
        {
            if (audio.SecondsFilename == null || string.IsNullOrEmpty(audio.SecondsFilename.Value))
            {
                return;
            }

            var resource = this.GetResource(audio.SecondsFilename.Value);
            if (resource == null)
            {
                return;
            }

            resource.SetReference(audio, "SecondsFilename");
            resource.UpdateIsUsedVisible();
        }

        /// <summary>
        /// Adds the reference for the SecondsFilename property.
        /// </summary>
        /// <param name="audio">
        /// The analog clock element.
        /// </param>
        private void UnsetReferenceSecondsFilename(AnalogClockElementDataViewModel audio)
        {
            if (audio.SecondsFilename == null || string.IsNullOrEmpty(audio.SecondsFilename.Value))
            {
                return;
            }

            var resource = this.GetResource(audio.SecondsFilename.Value);
            if (resource == null)
            {
                return;
            }

            resource.UnsetReference(audio, "SecondsFilename");
            resource.UpdateIsUsedVisible();
        }
    }
}