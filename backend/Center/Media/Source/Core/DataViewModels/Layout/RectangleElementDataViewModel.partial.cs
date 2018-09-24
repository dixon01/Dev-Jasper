// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RectangleElementDataViewModel.partial.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The pool config data view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.DataViewModels.Layout
{
    using System;
    using System.Windows.Media;

    using Gorba.Common.Formats.AlphaNT.Common;
    using Gorba.Motion.Infomedia.AhdlcRenderer.Engines;

    using Colors = System.Windows.Media.Colors;

    /// <summary>
    /// The pool config data view model.
    /// </summary>
    public partial class RectangleElementDataViewModel
    {
        /// <summary>
        /// Gets the component to be rendered.
        /// </summary>
        /// <returns>
        /// The <see cref="ComponentBase"/>.
        /// </returns>
        public override ComponentBase GetComponent()
        {
            var drawingColor = this.GetColor();
            var simpleColor = new SimpleColor(drawingColor.R, drawingColor.G, drawingColor.B);

            return new RectangleComponent
                   {
                       Color = simpleColor,
                       Height = this.Height.Value,
                       Width = this.Width.Value,
                       Visible = this.Visible.Value,
                       X = this.X.Value,
                       Y = this.Y.Value,
                       ZIndex = this.ZIndex.Value
                   };
        }

        /// <summary>
        /// The get color.
        /// </summary>
        /// <returns>
        /// The <see cref="Color"/>.
        /// </returns>
        public Color GetColor()
        {
            if (this.Color == null || string.IsNullOrEmpty(this.Color.Value))
            {
                return Colors.Black;
            }

            object colorObject;
            try
            {
                colorObject = ColorConverter.ConvertFromString(this.Color.Value);
            }
            catch (Exception)
            {
                return Colors.Black;
            }

            if (colorObject != null)
            {
                return (Color)colorObject;
            }

            return Colors.Black;
        }
    }
}
