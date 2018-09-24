// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResizeController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The ResizeController.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Controllers
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    using Gorba.Center.Media.Core.DataViewModels.Layout;
    using Gorba.Center.Media.Core.Properties;
    using Gorba.Center.Media.Core.ViewModels;
    using Gorba.Center.Media.Core.ViewModels.CommandParameters;

    /// <summary>
    /// The ResizeController.
    /// </summary>
    public class ResizeController
    {
        private readonly UserControl viewParent;

        private readonly int ledDotStep;

        private double resizeStartX;

        private double resizeStartY;

        private double resizeStartWidth;

        private double resizeStartHeight;

        private Point? resizeStartPosition;

        private FrameworkElement activeThumb;

        private FrameworkElement currentThumb;

        private bool ledDisplay;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResizeController"/> class.
        /// </summary>
        /// <param name="viewParent">the view Parent</param>
        public ResizeController(UserControl viewParent)
        {
            this.viewParent = viewParent;
            this.ledDotStep = (Settings.Default.LedDotRadius * 2) + Settings.Default.LedDotSpace;
            Mouse.AddPreviewMouseUpHandler(this.viewParent, this.MouseUp);
            Mouse.AddPreviewMouseMoveHandler(this.viewParent, this.MouseMove);
        }

        /// <summary>
        /// Activated a thumb for resizing
        /// </summary>
        /// <param name="thumb">the thumb</param>
        /// <param name="startPosition">the start position</param>
        /// <param name="isLedDisplay">a value indicating if the resize happens on the LED editor</param>
        public void ActivateResizeThumb(FrameworkElement thumb, Point startPosition, bool isLedDisplay = false)
        {
            this.activeThumb = thumb;
            this.ledDisplay = isLedDisplay;
            var element = thumb.Tag as GraphicalElementDataViewModelBase;

            if (element != null
                && element.Parent.Parent.SelectedEditorTool != EditorToolType.Zoom
                && element.Parent.Parent.SelectedEditorTool != EditorToolType.Hand)
            {
                this.currentThumb = thumb;

                this.resizeStartX = element.X.Value;
                this.resizeStartY = element.Y.Value;
                this.resizeStartWidth = element.Width.Value;
                this.resizeStartHeight = element.Height.Value;
                this.resizeStartPosition = startPosition;
            }
        }

        private void MouseMove(object sender, MouseEventArgs e)
        {
            if (this.activeThumb == null)
            {
                return;
            }

            if (!this.resizeStartPosition.HasValue)
            {
                return;
            }

            var element = this.activeThumb.Tag as GraphicalElementDataViewModelBase;
            if (element == null)
            {
                return;
            }

            var currentPosition = e.GetPosition(this.viewParent);

            var delta = currentPosition - this.resizeStartPosition.Value;
            if (this.ledDisplay)
            {
                delta = new Vector(delta.X / this.ledDotStep, delta.Y / this.ledDotStep);
            }

            delta /= element.Parent.Parent.Zoom / 100.0;
            delta.X = Math.Round(delta.X);
            delta.Y = Math.Round(delta.Y);

            switch (this.activeThumb.Name)
            {
                case "ResizeS":
                    this.AdjustHeight(element, delta.Y);
                    break;

                case "ResizeE":
                    this.AdjustWidth(element, delta.X);
                    break;

                case "ResizeN":
                    this.AdjustYPosition(element, delta.Y);
                    this.AdjustHeight(element, -delta.Y);
                    break;

                case "ResizeW":
                    this.AdjustXPosition(element, delta.X);
                    this.AdjustWidth(element, -delta.X);
                    break;

                case "ResizeSE":
                    this.AdjustWidth(element, delta.X);
                    this.AdjustHeight(element, delta.Y);
                    break;

                case "ResizeNE":
                    this.AdjustWidth(element, delta.X);
                    this.AdjustYPosition(element, delta.Y);
                    this.AdjustHeight(element, -delta.Y);
                    break;

                case "ResizeSW":
                    this.AdjustXPosition(element, delta.X);
                    this.AdjustWidth(element, -delta.X);
                    this.AdjustHeight(element, delta.Y);
                    break;

                case "ResizeNW":
                    this.AdjustXPosition(element, delta.X);
                    this.AdjustWidth(element, -delta.X);
                    this.AdjustYPosition(element, delta.Y);
                    this.AdjustHeight(element, -delta.Y);
                    break;
            }
        }

        private void MouseUp(object sender, MouseEventArgs e)
        {
            if (this.currentThumb != null)
            {
                this.resizeStartPosition = null;
                this.activeThumb = null;

                var element = this.currentThumb.Tag as GraphicalElementDataViewModelBase;

                if (element != null)
                {
                    var parameters = new ResizeElementParameters(element)
                    {
                        OldBounds =
                            new Rect(
                            this.resizeStartX,
                            this.resizeStartY,
                            this.resizeStartWidth,
                            this.resizeStartHeight),
                        NewBounds =
                            new Rect(
                            element.X.Value,
                            element.Y.Value,
                            element.Width.Value,
                            element.Height.Value)
                    };

                    var editorViewModel = element.Parent as EditorViewModelBase;
                    if (editorViewModel != null)
                    {
                        editorViewModel.ResizeElementCommand.Execute(parameters);
                    }

                    this.currentThumb = null;
                }
            }
        }

        private void AdjustXPosition(GraphicalElementDataViewModelBase element, double amount)
        {
            element.X.Value = (int)(this.resizeStartX + amount);
        }

        private void AdjustYPosition(GraphicalElementDataViewModelBase element, double amount)
        {
            element.Y.Value = (int)(this.resizeStartY + amount);
        }

        private void AdjustHeight(GraphicalElementDataViewModelBase element, double amount)
        {
            element.Height.Value =
                (int)Math.Max(
                    this.resizeStartHeight + amount, this.ledDisplay ? 1 : this.activeThumb.DesiredSize.Height);
        }

        private void AdjustWidth(GraphicalElementDataViewModelBase element, double amount)
        {
            element.Width.Value =
                (int)Math.Max(this.resizeStartWidth + amount, this.ledDisplay ? 1 : this.activeThumb.DesiredSize.Width);
        }
    }
}