// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EditPopupMarginConverter.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the EditPopupMarginConverter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.DataViewModels.Converters
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Input;

    using Gorba.Center.Media.Core.Properties;

    /// <summary>
    /// The EditPopupMarginConverter
    /// </summary>
    public class EditPopupMarginConverter : IMultiValueConverter
    {
        private const int TopBorderMargin = 50;

        private const int BottomBorderMargin = 43;

        private const int RightBorderMargin = 2;

        private double lastLeft;

        private double lastTop;

        private object lastPlacementObject;

        /// <summary>
        /// returns the element dimensions to a margin to but the popup next to the element
        /// </summary>
        /// <param name="values">the values</param>
        /// <param name="targetType">the target type</param>
        /// <param name="parameter">the parameter</param>
        /// <param name="culture">the culture</param>
        /// <returns>the result</returns>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var result = new Thickness();

            this.lastPlacementObject = null;

            if (values.Length > 13
                && TestParameterType(values))
            {
                var x = (int)values[0];
                var y = (int)values[1];
                var w = (int)values[2];
                var h = (int)values[3];
                var zoom = (double)values[4] / 100;
                var layoutOffset = (Point)values[5];
                var viewport = (FrameworkElement)values[6];
                var canvasWidth = (int)values[7] * zoom;
                var canvasHeight = (int)values[8] * zoom;
                var popupWidth = (double)values[9];
                var popupHeight = (double)values[10];
                var useMouseCoordinates = (bool)values[11];
                var isLedCanvas = (bool)values[12];

                if (isLedCanvas)
                {
                    x = ScaleToLedCanvas(x, ref y, ref w, ref canvasWidth, ref canvasHeight);
                }

                double left = this.lastLeft;
                double top = this.lastTop;

                if (!useMouseCoordinates)
                {
                    var margin = System.Convert.ToInt32(parameter);

                    left = ((x + w) * zoom) + margin;
                    top = y * zoom;

                    var viewportX = ((canvasWidth / 2) - (viewport.ActualWidth / 2)) - layoutOffset.X;
                    var viewportY = ((canvasHeight / 2) - (viewport.ActualHeight / 2)) - layoutOffset.Y;

                    if (left < viewportX + margin)
                    {
                        left = viewportX + margin;
                    }

                    if (top < viewportY + margin + TopBorderMargin)
                    {
                        top = viewportY + margin + TopBorderMargin;
                    }

                    if (left + popupWidth > viewportX + viewport.ActualWidth - margin - RightBorderMargin)
                    {
                        left = (viewportX + viewport.ActualWidth) - popupWidth - margin - RightBorderMargin;
                    }

                    if (top + popupHeight > viewportY + viewport.ActualHeight - margin - BottomBorderMargin)
                    {
                        top = (viewportY + viewport.ActualHeight) - popupHeight - margin - BottomBorderMargin;
                    }
                }
                else if (this.lastPlacementObject != values[13])
                {
                    var position = this.GetPopupPosition(popupHeight, popupWidth, viewport);
                    left = position.X;
                    top = position.Y;
                    this.lastPlacementObject = values[13];
                }

                result = new Thickness { Left = left, Top = top };

                this.lastLeft = left;
                this.lastTop = top;
            }

            return result;
        }

        /// <summary>
        /// not implemented
        /// </summary>
        /// <param name="value">the value</param>
        /// <param name="targetTypes">the target types</param>
        /// <param name="parameter">the parameter</param>
        /// <param name="culture">the culture</param>
        /// <returns>the result</returns>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private static bool TestParameterType(object[] values)
        {
            return values[0] is int
                   && values[1] is int
                   && values[2] is int
                   && values[3] is int
                   && values[4] is double
                   && values[5] is Point
                   && values[6] is FrameworkElement
                   && values[7] is int
                   && values[8] is int
                   && values[9] is double
                   && values[10] is double
                   && values[11] is bool
                   && values[12] is bool;
        }

        private static int ScaleToLedCanvas(
            int x,
            ref int y,
            ref int w,
            ref double canvasWidth,
            ref double canvasHeight)
        {
            var pixelSize = (Settings.Default.LedDotRadius * 2) + Settings.Default.LedDotSpace;
            x *= pixelSize;
            y *= pixelSize;
            w *= pixelSize;
            canvasWidth *= pixelSize;
            canvasHeight *= pixelSize;
            return x;
        }

        private Point GetPopupPosition(double popupHeight, double popupWidth, FrameworkElement viewport)
        {
            var mousePos = Mouse.GetPosition(viewport);
            if (popupHeight == 0)
            {
                popupHeight = 280;
            }

            if (popupWidth == 0)
            {
                popupWidth = 360;
            }

            var left = mousePos.X;
            var top = mousePos.Y;
            if (left + popupWidth > viewport.ActualWidth)
            {
                left = viewport.ActualWidth - popupWidth - RightBorderMargin;
            }

            if (top + popupHeight > viewport.ActualHeight)
            {
                top = viewport.ActualHeight - popupHeight;
            }

            return new Point(left, top);
        }
    }
}
