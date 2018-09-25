// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GraphicalEditorViewBase.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The base editor class for editors which can draw elements, zoom or pan.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Views.Editors
{
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    using Gorba.Center.Common.Wpf.Core;
    using Gorba.Center.Media.Core.Controllers;
    using Gorba.Center.Media.Core.DataViewModels.Layout;
    using Gorba.Center.Media.Core.DataViewModels.Presentation;
    using Gorba.Center.Media.Core.Resources;
    using Gorba.Center.Media.Core.ViewModels;
    using Gorba.Center.Media.Core.ViewModels.CommandParameters;
    using Gorba.Common.Configuration.Infomedia.Presentation;

    /// <summary>
    /// The base editor class for editors which can draw elements, zoom or pan.
    /// </summary>
    public class GraphicalEditorViewBase : EditorViewBase, INotifyPropertyChanged
    {
        /// <summary>
        /// the width property
        /// </summary>
        public static readonly DependencyProperty LayoutResolutionWidthProperty =
            DependencyProperty.Register(
                "LayoutResolutionWidth",
                typeof(double),
                typeof(GraphicalEditorViewBase),
                new PropertyMetadata(default(double), OnLayoutResolutionWidthChanged));

        /// <summary>
        /// the height property
        /// </summary>
        public static readonly DependencyProperty LayoutResolutionHeightProperty =
            DependencyProperty.Register(
                "LayoutResolutionHeight",
                typeof(double),
                typeof(GraphicalEditorViewBase),
                new PropertyMetadata(default(double), OnLayoutResolutionHeightChanged));

        /// <summary>
        /// the position property
        /// </summary>
        public static readonly DependencyProperty LayoutPositionProperty = DependencyProperty.Register(
            "LayoutPosition",
            typeof(Point),
            typeof(GraphicalEditorViewBase),
            new PropertyMetadata(default(Point), OnLayoutPositionChanged));

        /// <summary>
        /// the left mouse button is pressed property
        /// </summary>
        public static readonly DependencyProperty LeftMouseButtonIsPressedProperty =
            DependencyProperty.Register(
                "LeftMouseButtonIsPressed",
                typeof(bool),
                typeof(GraphicalEditorViewBase),
                new PropertyMetadata(default(bool)));

        /// <summary>
        /// The current virtual display config property.
        /// </summary>
        public static readonly DependencyProperty CurrentVirtualDisplayConfigProperty =
            DependencyProperty.Register(
                "CurrentVirtualDisplayConfig",
                typeof(VirtualDisplayConfigDataViewModel),
                typeof(GraphicalEditorViewBase),
                new PropertyMetadata(default(VirtualDisplayConfigDataViewModel), OnCurrentVirtualDisplayChanged));

        /// <summary>
        /// The current physical screen config property.
        /// </summary>
        public static readonly DependencyProperty CurrentPhysicalScreenConfigProperty =
            DependencyProperty.Register(
                "CurrentPhysicalScreenConfig",
                typeof(PhysicalScreenConfigDataViewModel),
                typeof(GraphicalEditorViewBase),
                new PropertyMetadata(default(PhysicalScreenConfigDataViewModel)));

        private const double ZoomStep = 10;

        private const double MinZoom = 10;

        private const double MaxZoom = 1000;

        private double rendererResolutionWidth;

        private double rendererResolutionHeight;

        /// <summary>
        /// The property changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or sets the width of the layout resolution
        /// </summary>
        public double LayoutResolutionWidth
        {
            get
            {
                return (double)this.GetValue(LayoutResolutionWidthProperty);
            }

            set
            {
                this.SetValue(LayoutResolutionWidthProperty, value);
                this.OnPropertyChanged("RendererResolutionWidth");
            }
        }

        /// <summary>
        /// Gets or sets the renderer resolution width.
        /// </summary>
        public double RendererResolutionWidth
        {
            get
            {
                return this.rendererResolutionWidth;
            }

            set
            {
                this.rendererResolutionWidth = value;
                this.OnPropertyChanged("RendererResolutionWidth");
            }
        }

        /// <summary>
        /// Gets or sets the renderer resolution height.
        /// </summary>
        public double RendererResolutionHeight
        {
            get
            {
                return this.rendererResolutionHeight;
            }

            set
            {
                this.rendererResolutionHeight = value;
                this.OnPropertyChanged("RendererResolutionHeight");
            }
        }

        /// <summary>
        /// Gets or sets the height of the layout resolution
        /// </summary>
        public double LayoutResolutionHeight
        {
            get
            {
                return (double)this.GetValue(LayoutResolutionHeightProperty);
            }

            set
            {
                this.SetValue(LayoutResolutionHeightProperty, value);
                this.OnPropertyChanged("RendererResolutionHeight");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the left mouse button is pressed
        /// </summary>
        public bool LeftMouseButtonIsPressed
        {
            get
            {
                return (bool)this.GetValue(LeftMouseButtonIsPressedProperty);
            }

            set
            {
                this.SetValue(LeftMouseButtonIsPressedProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the current physical screen config.
        /// </summary>
        public VirtualDisplayConfigDataViewModel CurrentVirtualDisplayConfig
        {
            get
            {
                return (VirtualDisplayConfigDataViewModel)this.GetValue(CurrentVirtualDisplayConfigProperty);
            }

            set
            {
                this.SetValue(CurrentVirtualDisplayConfigProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the current physical screen config.
        /// </summary>
        public PhysicalScreenConfigDataViewModel CurrentPhysicalScreenConfig
        {
            get
            {
                return (PhysicalScreenConfigDataViewModel)this.GetValue(CurrentPhysicalScreenConfigProperty);
            }

            set
            {
                this.SetValue(CurrentPhysicalScreenConfigProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the position of the layout
        /// </summary>
        public Point LayoutPosition
        {
            get
            {
                return (Point)this.GetValue(LayoutPositionProperty);
            }

            set
            {
                this.SetValue(LayoutPositionProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the pan start offset.
        /// </summary>
        protected Point PanStartOffset { get; set; }

        /// <summary>
        /// Gets or sets the pan start position.
        /// </summary>
        protected Point? PanStartPosition { get; set; }

        /// <summary>
        /// Gets or sets the dot step.
        /// </summary>
        protected int DotStep { get; set; }

        /// <summary>
        /// Gets or sets the zoom context menu.
        /// </summary>
        protected ContextMenu ZoomContextMenu { get; set; }

        /// <summary>
        /// the method that handles the ongoing global interactions
        /// </summary>
        /// <param name="sender">the sender</param>
        /// <param name="e">the mouse parameters</param>
        public void MouseInteraction(object sender, MouseEventArgs e)
        {
            var mousePosition = e.GetPosition(this);
            if (this.PanStartPosition.HasValue)
            {
                if (this.SelectedTool == EditorToolType.Hand)
                {
                    var delta = mousePosition - this.PanStartPosition.Value;
                    this.LayoutPosition = this.PanStartOffset + delta;
                }
            }
        }

        /// <summary>
        /// This method can be overridden to do derived class specific stuff when the layout resolution width changed.
        /// </summary>
        /// <param name="d">
        /// The dependency object.
        /// </param>
        /// <param name="e">
        /// The property changed event args.
        /// </param>
        protected virtual void OnLayoutResolutionWidthChangedInternal(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
        }

        /// <summary>
        /// This method can be overridden to do derived class specific stuff when the layout resolution height changed.
        /// </summary>
        /// <param name="d">
        /// The dependency object.
        /// </param>
        /// <param name="e">
        /// The property changed event args.
        /// </param>
        protected virtual void OnLayoutResolutionHeightChangedInternal(
           DependencyObject d,
           DependencyPropertyChangedEventArgs e)
        {
        }

        /// <summary>
        /// This method can be overridden to do derived class specific stuff when the current virtual display changed.
        /// </summary>
        protected virtual void OnCurrentVirtualDisplayChangedInternal()
        {
        }

        /// <summary>
        /// This method can be overridden to do derived class specific stuff when the layout position changed.
        /// </summary>
        protected virtual void OnLayoutPositionChangedInternal()
        {
        }

        /// <summary>
        /// Creates the zoom context menu.
        /// </summary>
        protected void CreateZoomContextMenu()
        {
            this.ZoomContextMenu = new ContextMenu();
            this.ZoomContextMenu.Items.Add(
                new MenuItem
                {
                    Header = MediaStrings.GraphicalEditor_FitOnScreen,
                    Command = new RelayCommand(this.OnZoomFitOnScreen),
                    InputGestureText = MediaStrings.GraphicalEditor_FitOnScreenShortcut,
                });
            this.ZoomContextMenu.Items.Add(
                new MenuItem
                {
                    Header = MediaStrings.GraphicalEditor_100Percent,
                    Command = new RelayCommand(this.OnZoom100Percent),
                    InputGestureText = MediaStrings.GraphicalEditor_100PercentShortcut,
                });
            this.ZoomContextMenu.Items.Add(
                new MenuItem
                {
                    Header = MediaStrings.GraphicalEditor_200Percent,
                    Command = new RelayCommand(this.OnZoom200Percent)
                });
            this.ZoomContextMenu.Items.Add(new Separator());
            this.ZoomContextMenu.Items.Add(
                new MenuItem
                {
                    Header = MediaStrings.GraphicalEditor_ZoomIn,
                    Command = new RelayCommand(this.OnZoomIn),
                    InputGestureText = MediaStrings.GraphicalEditor_ZoomInShortcut,
                });
            this.ZoomContextMenu.Items.Add(
                new MenuItem
                {
                    Header = MediaStrings.GraphicalEditor_ZoomOut,
                    Command = new RelayCommand(this.OnZoomOut),
                    InputGestureText = MediaStrings.GraphicalEditor_ZoomOutShortcut,
                });
        }

        /// <summary>
        /// The zoom handling.
        /// </summary>
        /// <param name="button">
        /// The button.
        /// </param>
        /// <param name="isRect">
        /// A value indicating if the zoom is done by a rectangle.
        /// </param>
        protected void ZoomHandling(MouseButton button, bool isRect)
        {
            if (this.InteractionStartPosition.HasValue)
            {
                var interactionStartPosition = this.InteractionStartPosition.Value;

                var editor = (EditorViewModelBase)this.DataContext;
                var screen = editor.Parent.MediaApplicationState.CurrentVirtualDisplay;
                var modifiers = new ModifiersState();

                if (button == MouseButton.Left)
                {
                    if (!modifiers.IsControlPressed)
                    {
                        if (isRect)
                        {
                            var zoomCalculator = new ZoomCalculator(
                                this.Zoom,
                                this.LayoutPosition,
                                screen.Width.Value * this.DotStep,
                                screen.Height.Value * this.DotStep,
                                this.ActualWidth,
                                this.ActualHeight,
                                MinZoom,
                                MaxZoom);
                            zoomCalculator.SetRectangleZoom(this.InteractionRectangle);
                            this.LayoutPosition = zoomCalculator.GetLayoutPosition();
                            this.Zoom = zoomCalculator.GetZoom();
                        }
                        else
                        {
                            var zoomCalculator = new ZoomCalculator(
                                this.Zoom,
                                this.LayoutPosition,
                                screen.Width.Value * this.DotStep,
                                screen.Height.Value * this.DotStep,
                                this.ActualWidth,
                                this.ActualHeight,
                                MinZoom,
                                MaxZoom);
                            zoomCalculator.ZoomInAt(interactionStartPosition, ZoomStep);
                            this.LayoutPosition = zoomCalculator.GetLayoutPosition();
                            this.Zoom = zoomCalculator.GetZoom();
                        }
                    }
                    else
                    {
                        var zoomCalculator = new ZoomCalculator(
                            this.Zoom,
                            this.LayoutPosition,
                            screen.Width.Value * this.DotStep,
                            screen.Height.Value * this.DotStep,
                            this.ActualWidth,
                            this.ActualHeight,
                            MinZoom,
                            MaxZoom);
                        zoomCalculator.ZoomOutAt(interactionStartPosition, ZoomStep);
                        this.LayoutPosition = zoomCalculator.GetLayoutPosition();
                        this.Zoom = zoomCalculator.GetZoom();
                    }
                }
            }
        }

        /// <summary>
        /// The select elements handling.
        /// </summary>
        /// <param name="changedButton">
        /// The changed button.
        /// </param>
        /// <param name="isRect">
        /// A value indicating if it is a rectangle selection.
        /// </param>
        /// <param name="modifiers">
        /// The modifiers.
        /// </param>
        protected void SelectElementsHandling(MouseButton changedButton, bool isRect, ModifiersState modifiers)
        {
            if (this.InteractionStartPosition.HasValue && changedButton == MouseButton.Left)
            {
                var startPosition = new Point(
                 this.InteractionStartPosition.Value.X / this.DotStep,
                 this.InteractionStartPosition.Value.Y / this.DotStep);
                if (isRect)
                {
                    var bottomrightPosition = new Point(
                        (int)(this.MousePosition.X / this.DotStep) + 1,
                        (int)(this.MousePosition.Y / this.DotStep) + 1);
                    var parameters = new CreateElementParameters
                    {
                        Bounds =
                            new Rect(
                            startPosition,
                            bottomrightPosition),
                        Modifiers = modifiers,
                    };
                    ((EditorViewModelBase)this.DataContext).SelectLayoutElementsCommand.Execute(parameters);
                }
                else
                {
                    var parameter = new SelectElementParameters
                    {
                        Position = startPosition,
                        Modifiers = modifiers,
                    };
                    ((EditorViewModelBase)this.DataContext).SelectLayoutElementCommand.Execute(parameter);
                }
            }
        }

        /// <summary>
        /// The on mouse wheel.
        /// </summary>
        /// <param name="delta">
        /// The delta.
        /// </param>
        /// <param name="interactionStartPosition">
        /// The interaction start position.
        /// </param>
        protected void OnMouseWheel(double delta, Point interactionStartPosition)
        {
            if ((Keyboard.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt)
            {
                var editor = (EditorViewModelBase)this.DataContext;
                var screen = editor.Parent.MediaApplicationState.CurrentVirtualDisplay;

                var zoomCalculator = new ZoomCalculator(
                    this.Zoom,
                    this.LayoutPosition,
                    screen.Width.Value,
                    screen.Height.Value,
                    this.ActualWidth,
                    this.ActualHeight,
                    MinZoom,
                    MaxZoom);

                if (delta > 0)
                {
                    zoomCalculator.ZoomInAt(interactionStartPosition, delta);
                }
                else
                {
                    zoomCalculator.ZoomOutAt(interactionStartPosition, -delta);
                }

                this.Zoom = zoomCalculator.GetZoom();

                if (this.Zoom < ZoomStep)
                {
                    this.Zoom = ZoomStep;
                }
                else if (this.Zoom > MaxZoom)
                {
                    this.Zoom = MaxZoom;
                }
                else
                {
                    this.LayoutPosition = zoomCalculator.GetLayoutPosition();
                }
            }
            else if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                this.LayoutPosition = new Point(this.LayoutPosition.X - delta, this.LayoutPosition.Y);
            }
            else
            {
                this.LayoutPosition = new Point(this.LayoutPosition.X, this.LayoutPosition.Y - delta);
            }
        }

        /// <summary>
        /// The on zoom in.
        /// </summary>
        protected void OnZoomIn()
        {
            var editor = (EditorViewModelBase)this.DataContext;
            var screen = editor.Parent.MediaApplicationState.CurrentVirtualDisplay;

            var zoomCalculator = new ZoomCalculator(
                this.Zoom,
                this.LayoutPosition,
                screen.Width.Value,
                screen.Height.Value,
                this.ActualWidth,
                this.ActualHeight,
                MinZoom,
                MaxZoom);

            zoomCalculator.ZoomIn(ZoomStep);

            this.LayoutPosition = zoomCalculator.GetLayoutPosition();
            this.Zoom = zoomCalculator.GetZoom();
        }

        /// <summary>
        /// The on zoom out.
        /// </summary>
        protected void OnZoomOut()
        {
            var editor = (EditorViewModelBase)this.DataContext;
            var screen = editor.Parent.MediaApplicationState.CurrentVirtualDisplay;

            var zoomCalculator = new ZoomCalculator(
                this.Zoom,
                this.LayoutPosition,
                screen.Width.Value,
                screen.Height.Value,
                this.ActualWidth,
                this.ActualHeight,
                MinZoom,
                MaxZoom);

            zoomCalculator.ZoomOut(ZoomStep);

            this.LayoutPosition = zoomCalculator.GetLayoutPosition();
            this.Zoom = zoomCalculator.GetZoom();
        }

        /// <summary>
        /// The on key down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (!this.CanProcessKeyDown())
            {
                return;
            }

            switch (e.Key)
            {
                case Key.Space:
                    if (!this.LastTool.HasValue && this.SelectedTool != EditorToolType.Hand)
                    {
                        this.LastTool = this.SelectedTool;
                        this.SuspendToolChangeEvents = true;
                        this.SelectedTool = EditorToolType.Hand;
                        this.SuspendToolChangeEvents = false;
                    }

                    break;
            }
        }

        /// <summary>
        /// The on property changed.
        /// </summary>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Sets the zoom so that the editor fits the screen
        /// </summary>
        /// <param name="dotStep">
        /// The dot step.
        /// </param>
        protected void OnZoomFitOnScreen(int dotStep)
        {
            var editor = (EditorViewModelBase)this.DataContext;
            var screen = editor.Parent.MediaApplicationState.CurrentVirtualDisplay;

            this.LayoutPosition = new Point(0, 0);

            var widthRatio = this.ActualWidth / (screen.Width.Value * dotStep);
            var heightRatio = this.ActualHeight / (screen.Height.Value * dotStep);
            if (widthRatio < heightRatio)
            {
                this.Zoom = widthRatio * 100;
            }
            else
            {
                this.Zoom = heightRatio * 100;
            }
        }

        /// <summary>
        /// Handles the keys that are shared on all graphical editors.
        /// </summary>
        /// <param name="editor">
        /// The editor.
        /// </param>
        /// <param name="e">
        /// The key event args.
        /// </param>
        /// <param name="modifiers">
        /// The modifiers.
        /// </param>
        /// <returns>
        /// <c>true</c> if the key was handled; <c>false</c> otherwise.
        /// </returns>
        [SuppressMessage("StyleCopPlus.StyleCopPlusRules", "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here. It's a big switch statement.")]
        protected override bool HandleSharedKeyUp(EditorViewModelBase editor, KeyEventArgs e, ModifiersState modifiers)
        {
            if (base.HandleSharedKeyUp(editor, e, modifiers))
            {
                return true;
            }

            switch (e.Key)
            {
                case Key.H:
                    this.SelectedTool = EditorToolType.Hand;
                    break;

                case Key.Z:
                    if (!modifiers.IsControlPressed)
                    {
                        this.SelectedTool = EditorToolType.Zoom;
                    }

                    break;

                case Key.T:
                    this.SelectedTool = EditorToolType.StaticText;
                    break;
                case Key.D:
                    this.SelectedTool = EditorToolType.DynamicText;
                    break;

                case Key.I:
                    this.SelectedTool = EditorToolType.Image;
                    break;

                case Key.Delete:
                    editor.DeleteElementsCommand.Execute(editor.SelectedElements);
                    break;

                case Key.Home:
                    this.LayoutPosition = new Point(0, 0);
                    this.Zoom = 100;
                    break;

                case Key.OemPlus:
                    if (modifiers.IsControlPressed)
                    {
                        this.OnZoomIn();
                    }

                    break;

                case Key.Add:
                    if (modifiers.IsControlPressed)
                    {
                        this.OnZoomIn();
                    }

                    break;

                case Key.OemMinus:
                    if (modifiers.IsControlPressed)
                    {
                        this.OnZoomOut();
                    }

                    break;

                case Key.Subtract:
                    if (modifiers.IsControlPressed)
                    {
                        this.OnZoomOut();
                    }

                    break;

                case Key.Left:
                        editor.MoveSelectedElementsCommand.Execute(
                            new MoveElementsCommandParameters
                            {
                                Direction = MovementDirection.Left,
                                Modifiers = modifiers,
                            });
                        e.Handled = true;
                    break;

                case Key.Right:
                        editor.MoveSelectedElementsCommand.Execute(
                            new MoveElementsCommandParameters
                            {
                                Direction = MovementDirection.Right,
                                Modifiers = modifiers
                            });
                        e.Handled = true;
                    break;

                case Key.Up:
                        editor.MoveSelectedElementsCommand.Execute(
                            new MoveElementsCommandParameters
                            {
                                Direction = MovementDirection.Up,
                                Modifiers = modifiers
                            });
                        e.Handled = true;
                    break;

                case Key.Down:
                        editor.MoveSelectedElementsCommand.Execute(
                            new MoveElementsCommandParameters
                            {
                                Direction = MovementDirection.Down,
                                Modifiers = modifiers
                            });
                        e.Handled = true;

                    break;
                default:
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Returns a value indicating if the <paramref name="element"/> has the highest Z-Index.
        /// </summary>
        /// <param name="element">
        /// The element.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        protected bool HasHighestZIndex(DrawableElementDataViewModelBase element)
        {
            if (element == null)
            {
                return false;
            }

            return element.ZIndex.Value >= this.HighestElementUnderMouse.ZIndex.Value;
        }

        /// <summary>
        /// The deselect all elements.
        /// </summary>
        protected void DeselectAllElements()
        {
            var parameter = new SelectElementParameters { ClearSelection = true, Modifiers = new ModifiersState() };
            ((ViewModels.EditorViewModelBase)this.DataContext).SelectLayoutElementCommand.Execute(parameter);
        }

        private static void OnLayoutResolutionWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var editor = (GraphicalEditorViewBase)d;
            if (editor != null)
            {
                editor.OnLayoutResolutionWidthChangedInternal(d, e);
            }
        }

        private static void OnLayoutResolutionHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var editor = (GraphicalEditorViewBase)d;
            if (editor != null)
            {
                editor.OnLayoutResolutionHeightChangedInternal(d, e);
            }
        }

        private static void OnCurrentVirtualDisplayChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (GraphicalEditorViewBase)d;
            control.OnCurrentVirtualDisplayChangedInternal();
        }

        private static void OnLayoutPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (GraphicalEditorViewBase)d;
            if (control != null)
            {
                control.OnLayoutPositionChangedInternal();
            }
        }

        private void OnZoomFitOnScreen()
        {
            if (this.CurrentPhysicalScreenConfig.Type.Value != PhysicalScreenType.LED
                && this.CurrentPhysicalScreenConfig.Type.Value != PhysicalScreenType.TFT)
            {
                return;
            }

            this.OnZoomFitOnScreen(this.DotStep);
        }

        private void OnZoom100Percent()
        {
            this.LayoutPosition = new Point(0, 0);
            this.Zoom = 100;
        }

        private void OnZoom200Percent()
        {
            this.LayoutPosition = new Point(0, 0);
            this.Zoom = 200;
        }
    }
}
