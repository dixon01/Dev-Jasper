// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EditableLayoutElementBase.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Views.LayoutElements
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    using Gorba.Center.Media.Core.ViewModels;

    /// <summary>
    /// Defines the base properties of an editable layout element
    /// </summary>
    public class EditableLayoutElementBase : UserControl
    {
        /// <summary>
        /// The Dependency Property to the ViewMode Property defining the view mode of the inner component
        /// </summary>
        public static readonly DependencyProperty ViewModeProperty = DependencyProperty.Register(
            "ViewMode",
            typeof(ViewModeType),
            typeof(EditableLayoutElementBase),
            new PropertyMetadata(default(ViewModeType)));

        /// <summary>
        /// The Dependency Property to the ShowAdorner Property defining if the resizing adorners should be shown.
        /// </summary>
        public static readonly DependencyProperty ShowAdornerProperty = DependencyProperty.Register(
            "ShowAdorner",
            typeof(bool),
            typeof(EditableLayoutElementBase),
            new PropertyMetadata(default(bool)));

        /// <summary>
        /// Initializes a new instance of the <see cref="EditableLayoutElementBase"/> class.
        /// </summary>
        public EditableLayoutElementBase()
        {
            this.MouseDoubleClick += this.DoubleClick;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to show the adorners.
        /// </summary>
        public bool ShowAdorner
        {
            get
            {
                return (bool)this.GetValue(ShowAdornerProperty);
            }

            set
            {
                this.SetValue(ShowAdornerProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the view mode.
        /// </summary>
        public ViewModeType ViewMode
        {
            get
            {
                return (ViewModeType)this.GetValue(ViewModeProperty);
            }

            set
            {
                this.SetValue(ViewModeProperty, value);
            }
        }

        /// <summary>
        /// Handler for the double click event.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The event args.
        /// </param>
        protected virtual void DoubleClick(object sender, RoutedEventArgs e)
        {
            this.Dispatcher.BeginInvoke((Action)this.EnterEditMode);
        }

        /// <summary>
        /// on key up
        /// </summary>
        /// <param name="e">
        /// the arguments
        /// </param>
        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);

            if (e.Key == Key.Return)
            {
                this.ExitEditMode();
            }
        }

        /// <summary>
        /// the method to enter the edit mode
        /// </summary>
        protected virtual void EnterEditMode()
        {
            Mouse.AddPreviewMouseUpHandler(Window.GetWindow(this), handler: this.OnMouseUp);
            this.ViewMode = ViewModeType.Edit;
        }

        /// <summary>
        /// The exit edit mode.
        /// </summary>
        protected virtual void ExitEditMode()
        {
            Mouse.RemovePreviewMouseUpHandler(Window.GetWindow(this), this.OnMouseUp);

            this.ViewMode = ViewModeType.View;
        }

        private void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            var pos = e.GetPosition(this);
            if (pos.X < 0 || pos.X >= this.Width || pos.Y < 0 || pos.Y >= this.Height)
            {
                this.ExitEditMode();
            }
        }
    }
}
