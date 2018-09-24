// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AnalogClockElementDataViewModel.partial.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.DataViewModels.Layout
{
    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Center.Media.Core.Models.Layout;
    using Gorba.Center.Media.Core.Properties;

    /// <summary>
    /// Defines the properties of an image layout element.
    /// </summary>
    public partial class AnalogClockElementDataViewModel
    {
        /// <summary>
        /// The unset media reference.
        /// </summary>
        /// <param name="commandRegistry">
        /// The command registry.
        /// </param>
        public override void UnsetMediaReference(ICommandRegistry commandRegistry)
        {
            if (this.HourFilename != null && this.HourFilename.Value != string.Empty)
            {
                var hash = this.Shell.MediaApplicationState.CurrentProject.GetMediaHash(this.HourFilename.Value);
                this.DecreaseMediaReferenceByHash(hash, commandRegistry);
            }

            if (this.MinuteFilename != null && this.MinuteFilename.Value != string.Empty)
            {
                var hash = this.Shell.MediaApplicationState.CurrentProject.GetMediaHash(this.MinuteFilename.Value);
                this.DecreaseMediaReferenceByHash(hash, commandRegistry);
            }

            if (this.SecondsFilename != null && this.SecondsFilename.Value != string.Empty)
            {
                var hash = this.Shell.MediaApplicationState.CurrentProject.GetMediaHash(this.SecondsFilename.Value);
                this.DecreaseMediaReferenceByHash(hash, commandRegistry);
            }

            this.ResourceManager.AnalogClockElementManager.UnsetReferences(this);
        }

        /// <summary>
        /// The set media reference.
        /// </summary>
        /// <param name="commandRegistry">
        /// The command registry.
        /// </param>
        public override void SetMediaReference(ICommandRegistry commandRegistry)
        {
            if (this.HourFilename != null && this.HourFilename.Value != string.Empty)
            {
                var hash = this.Shell.MediaApplicationState.CurrentProject.GetMediaHash(this.HourFilename.Value);
                this.IncreaseMediaReferenceByHash(hash, commandRegistry);
            }

            if (this.MinuteFilename != null && this.MinuteFilename.Value != string.Empty)
            {
                var hash = this.Shell.MediaApplicationState.CurrentProject.GetMediaHash(this.MinuteFilename.Value);
                this.IncreaseMediaReferenceByHash(hash, commandRegistry);
            }

            if (this.SecondsFilename != null && this.SecondsFilename.Value != string.Empty)
            {
                var hash = this.Shell.MediaApplicationState.CurrentProject.GetMediaHash(this.SecondsFilename.Value);
                this.IncreaseMediaReferenceByHash(hash, commandRegistry);
            }

            this.ResourceManager.AnalogClockElementManager.SetReferences(this);
        }

        partial void Initialize(AnalogClockElementDataModel dataModel)
        {
            if (dataModel == null)
            {
                this.SetDefaultValues();
            }
        }

        private void SetDefaultValues()
        {
            this.Hour.CenterX.Value = Settings.Default.HourCenterX;
            this.Hour.CenterY.Value = Settings.Default.HourCenterY;
            this.Hour.Height.Value = Settings.Default.HourHeight;
            this.Hour.Width.Value = Settings.Default.HourWidth;
            this.Hour.X.Value = Settings.Default.HourX;
            this.Hour.Y.Value = Settings.Default.HourY;
            this.Hour.ZIndex.Value = Settings.Default.HourZIndex;
            this.Hour.Filename.Value = Settings.Default.HourFilename;

            this.Minute.CenterX.Value = Settings.Default.MinuteCenterX;
            this.Minute.CenterY.Value = Settings.Default.MinuteCenterY;
            this.Minute.Height.Value = Settings.Default.MinuteHeight;
            this.Minute.Width.Value = Settings.Default.MinuteWidth;
            this.Minute.X.Value = Settings.Default.MinuteX;
            this.Minute.Y.Value = Settings.Default.MinuteY;
            this.Minute.ZIndex.Value = Settings.Default.MinuteZIndex;
            this.Minute.Filename.Value = Settings.Default.MinuteFilename;

            this.Seconds.CenterX.Value = Settings.Default.SecondsCenterX;
            this.Seconds.CenterY.Value = Settings.Default.SecondsCenterY;
            this.Seconds.Height.Value = Settings.Default.SecondsHeight;
            this.Seconds.Width.Value = Settings.Default.SecondsWidth;
            this.Seconds.X.Value = Settings.Default.SecondsX;
            this.Seconds.Y.Value = Settings.Default.SecondsY;
            this.Seconds.ZIndex.Value = Settings.Default.SecondsZIndex;
            this.Seconds.Filename.Value = Settings.Default.SecondsFilename;
        }
    }
}
