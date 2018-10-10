// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MouseHandlingExtensions.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The MouseHandlingExtensions.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Extensions
{
    using System;
    using System.Windows;
    using System.Windows.Input;

    /// <summary>
    /// Extensions for mouse handling
    /// </summary>
    public static class MouseHandlingExtensions
    {
        /// <summary>
        /// Extension Method to add a mouse up handler to an framework element which is automatically removed
        /// afterwards and calls an callback if the mouse up happened outside the element
        /// </summary>
        /// <param name="element">the element</param>
        /// <param name="mouseUpCallback">the callback to be called after mouse up</param>
        /// <returns>the registered mouse up handler</returns>
        public static MouseButtonEventHandler AddMouseUpHandler(this FrameworkElement element, Action mouseUpCallback)
        {
            MouseButtonEventHandler mouseUpHandler = null;
            mouseUpHandler = (o, e) =>
            {
                var pos = e.GetPosition(element);
                if (pos.X < 0 || pos.X >= element.ActualWidth || pos.Y < 0 || pos.Y >= element.ActualHeight)
                {
                    Mouse.RemoveMouseUpHandler((DependencyObject)o, mouseUpHandler);

                    mouseUpCallback();
                }
            };

            Mouse.AddMouseUpHandler(Window.GetWindow(element), mouseUpHandler);

            return mouseUpHandler;
        }
    }
}
