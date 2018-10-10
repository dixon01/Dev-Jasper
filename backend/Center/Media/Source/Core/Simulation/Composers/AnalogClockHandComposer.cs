// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AnalogClockHandComposer.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AnalogClockHandComposer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Simulation.Composers
{
    using Gorba.Center.Media.Core.DataViewModels.Layout;
    using Gorba.Motion.Infomedia.Entities.Screen;

    /// <summary>
    /// Composer handling <see cref="AnalogClockHandElementDataViewModel"/>.
    /// </summary>
    public class AnalogClockHandComposer :
        DrawableComposerBase<AnalogClockHandElementDataViewModel, AnalogClockHandItem>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AnalogClockHandComposer"/> class.
        /// </summary>
        /// <param name="context">
        /// The composer context.
        /// </param>
        /// <param name="parent">
        /// The parent composer.
        /// </param>
        /// <param name="viewModel">
        /// The data view model.
        /// </param>
        public AnalogClockHandComposer(
            IComposerContext context, IComposer parent, AnalogClockHandElementDataViewModel viewModel)
            : base(context, parent, viewModel)
        {
            this.UpdateFilename();
            this.Item.Scaling = this.ViewModel.Scaling.Value;
            this.Item.Mode = this.ViewModel.Mode.Value;
            this.Item.CenterX = this.ViewModel.CenterX.Value;
            this.Item.CenterY = this.ViewModel.CenterY.Value;
        }

        /// <summary>
        /// Method to be overridden by subclasses to handle the change of a property of the data view model.
        /// </summary>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        protected override void HandlePropertyChange(string propertyName)
        {
            switch (propertyName)
            {
                case "Filename":
                    this.UpdateFilename();
                    break;
                case "Scaling":
                    this.Item.Scaling = this.ViewModel.Scaling.Value;
                    break;
                case "Mode":
                    this.Item.Mode = this.ViewModel.Mode.Value;
                    break;
                case "CenterX":
                    this.Item.CenterX = this.ViewModel.CenterX.Value;
                    break;
                case "CenterY":
                    this.Item.CenterY = this.ViewModel.CenterY.Value;
                    break;
                default:
                    base.HandlePropertyChange(propertyName);
                    break;
            }
        }

        private void UpdateFilename()
        {
            var image = this.ViewModel.Image;
            this.Item.Filename = this.Context.GetImageFileName(image == null ? null : image.Hash);
        }
    }
}
