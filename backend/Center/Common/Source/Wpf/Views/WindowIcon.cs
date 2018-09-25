// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WindowIcon.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the WindowIcon type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Views
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    using SystemCommands = System.Windows.SystemCommands;

    /// <summary>
    /// Interaction logic for WindowIcon
    /// </summary>
    public class WindowIcon : ContentControl
    {
        /// <summary>
        /// Initializes static members of the <see cref="WindowIcon" /> class.
        /// </summary>
        static WindowIcon()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(WindowIcon),
                new FrameworkPropertyMetadata(typeof(WindowIcon)));
        }

        /// <summary>
        /// handles mouse interaction with the Window Icon
        /// </summary>
        /// <param name="e">the event parameter</param>
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            var w = Window.GetWindow(this);
            if (e.ClickCount == 1)
            {
                Point p;
                if (e.ChangedButton == MouseButton.Left)
                {
                    p = this.PointToScreen(e.GetPosition(this));
                    p.X += 1;
                    p.Y += 1;
                }
                else
                {
                    p = this.PointToScreen(e.GetPosition(this));
                    p.X += 1;
                    p.Y += 1;
                }

                SystemCommands.ShowSystemMenu(w, p);
            }

            if (e.ClickCount == 2 && e.ChangedButton == MouseButton.Left)
            {
                if (w != null)
                {
                    w.Close();
                }
            }
        }
    }
}
