// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LedEditor.xaml.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Interaction logic for LedEditor.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Views.Editors
{
    using System;
    using System.Linq;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Threading;

    using Gorba.Center.Media.Core.Controllers;
    using Gorba.Center.Media.Core.DataViewModels.Layout;
    using Gorba.Center.Media.Core.Extensions;
    using Gorba.Center.Media.Core.Properties;
    using Gorba.Center.Media.Core.ViewModels;
    using Gorba.Center.Media.Core.ViewModels.CommandParameters;
    using Gorba.Common.Configuration.Infomedia.Presentation;

    /// <summary>
    /// Interaction logic for LedEditor.xaml
    /// </summary>
    public partial class LedEditor
    {
        private readonly ResizeController resizeController;

        private int dotSpacing;

        /// <summary>
        /// Initializes a new instance of the <see cref="LedEditor"/> class.
        /// </summary>
        public LedEditor()
        {
            this.InitializeComponent();

            this.dotSpacing = Settings.Default.LedDotSpace;
            this.DotStep = (Settings.Default.LedDotRadius * 2) + Settings.Default.LedDotSpace;
            Mouse.AddPreviewMouseUpHandler(this, this.StopEditorInteraction);
            Mouse.AddPreviewMouseMoveHandler(this, this.MouseInteraction);
            Mouse.AddMouseWheelHandler(this, this.OnMouseWheel);
            this.Loaded += (sender, args) =>
            {
                var window = Window.GetWindow(this);
                if (window != null)
                {
                    Keyboard.AddPreviewKeyDownHandler(window, this.OnKeyDown);
                    Keyboard.AddPreviewKeyUpHandler(window, this.OnKeyUp);
                }

                var editor = (ViewModels.LedEditorViewModel)this.DataContext;
                editor.ScreenshotTaken += this.TakeScreenshot;

                this.OnCurrentVirtualDisplayChangedInternal();
            };

            this.CreateZoomContextMenu();

            this.resizeController = new ResizeController(this);
        }

        /// <summary>
        /// the method that handles the start of editor interactions
        /// </summary>
        /// <param name="sender">the sender</param>
        /// <param name="e">the mouse parameters</param>
        public void StartEditorInteraction(object sender, MouseButtonEventArgs e)
        {
            this.Focus();
            this.InteractionStartPosition = this.FitPointToDot(e.GetPosition(this.Renderer));
            if (e.ChangedButton == MouseButton.Left)
            {
                if (this.SelectedTool == EditorToolType.Move)
                {
                    var ledEditor = (ViewModels.LedEditorViewModel)this.DataContext;
                    var startPosition = new Point(
                                                  this.InteractionStartPosition.Value.X / this.DotStep,
                                                  this.InteractionStartPosition.Value.Y / this.DotStep);
                    this.CurrentElementUnderMouse =
                        ledEditor.SelectedElements.OfType<GraphicalElementDataViewModelBase>()
                            .GetElementAt(startPosition);
                    this.HighestElementUnderMouse = ledEditor.Elements.GetElementAt(startPosition);
                    this.LastMousePosition = e.GetPosition(this.Renderer);
                }
                else if (this.SelectedTool == EditorToolType.Hand)
                {
                    this.PanStartOffset = new Point(this.LayoutPosition.X, this.LayoutPosition.Y);
                    this.PanStartPosition = e.GetPosition(this);
                }
            }
        }

        /// <summary>
        /// the method that handles the ongoing editor interactions
        /// </summary>
        /// <param name="sender">the sender</param>
        /// <param name="e">the mouse parameters</param>
        public void EditorInteraction(object sender, MouseEventArgs e)
        {
            this.MousePosition = e.GetPosition(this.Renderer);
            if (this.InteractionStartPosition.HasValue)
            {
                if (this.SelectedTool != EditorToolType.Hand)
                {
                    if (this.SelectedTool == EditorToolType.Move && this.CurrentElementUnderMouse != null
                        && e.LeftButton == MouseButtonState.Pressed)
                    {
                        var modifiers = new ModifiersState();
                        this.MoveElementsHandling(MouseButton.Left, modifiers);
                    }
                    else
                    {
                        this.InteractionRectangle = new Rect(
                                this.InteractionStartPosition.Value,
                                this.FitPointToDot(this.MousePosition, true));
                    }
                }
            }
        }

        /// <summary>
        /// the method that handles the stop of editor interactions
        /// </summary>
        /// <param name="sender">the sender</param>
        /// <param name="e">the mouse parameters</param>
        public void StopEditorInteraction(object sender, MouseButtonEventArgs e)
        {
            var interationEndpoint = this.FitPointToDot(e.GetPosition(this.Renderer));
            var isRect = this.InteractionStartPosition.HasValue
                         && this.Distance(this.InteractionStartPosition.Value, interationEndpoint)
                         > this.MinDragDistance.Length;

            var modifiers = new ModifiersState();

            switch (this.SelectedTool)
            {
                case EditorToolType.Move:
                    if (this.CurrentElementUnderMouse == null || modifiers.IsControlPressed
                        || !this.HasHighestZIndex(this.CurrentElementUnderMouse))
                    {
                        this.SelectElementsHandling(e.ChangedButton, isRect, modifiers);
                    }

                    break;
                case EditorToolType.Zoom:
                    this.ZoomHandling(e.ChangedButton, isRect);
                    break;
                case EditorToolType.Hand:
                    break;
                case EditorToolType.StaticText:
                    this.AddElementHandling(e.ChangedButton, isRect, LayoutElementType.StaticText);
                    break;
                case EditorToolType.Image:
                    this.AddElementHandling(e.ChangedButton, isRect, LayoutElementType.Image);
                    break;
                case EditorToolType.DynamicText:
                    this.AddElementHandling(e.ChangedButton, isRect, LayoutElementType.DynamicText);
                    break;
                case EditorToolType.Rectangle:
                    this.AddElementHandling(e.ChangedButton, isRect, LayoutElementType.Rectangle);
                    break;
            }

            this.InteractionStartPosition = null;
            this.PanStartPosition = null;
            this.InteractionRectangle = new Rect(0, 0, 0, 0);
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
        protected override void OnLayoutResolutionHeightChangedInternal(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            var control = (LedEditor)d;
            if (Math.Abs(control.LayoutResolutionHeight) < double.Epsilon)
            {
                return;
            }

            var diameter = Settings.Default.LedDotRadius * 2;
            var distanceBetweenLed = Settings.Default.LedDotSpace;
            var height = (control.LayoutResolutionHeight * diameter)
                         + ((control.LayoutResolutionHeight - 1) * distanceBetweenLed);
            control.RendererResolutionHeight = height;
        }

        /// <summary>
        /// This method can be overridden to do derived class specific stuff when the dependency property
        /// "LayoutResolutionWidthW changed.
        /// </summary>
        /// <param name="d">
        /// The dependency object.
        /// </param>
        /// <param name="e">
        /// The property changed event args.
        /// </param>
        protected override void OnLayoutResolutionWidthChangedInternal(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            var control = (LedEditor)d;
            if (Math.Abs(control.LayoutResolutionWidth) < double.Epsilon)
            {
                return;
            }

            var diameter = Settings.Default.LedDotRadius * 2;
            var distanceBetweenLed = Settings.Default.LedDotSpace;
            var width = (control.LayoutResolutionWidth * diameter)
                        + ((control.LayoutResolutionWidth - 1) * distanceBetweenLed);
            control.RendererResolutionWidth = width;
        }

        /// <summary>
        /// This method can be overridden to do derived class specific stuff when the dependency property
        /// "LayoutPosition" changed.
        /// </summary>
        protected override void OnLayoutPositionChangedInternal()
        {
            var currentVirtualDisplayConfig = this.CurrentVirtualDisplayConfig;
            var currentPhysicalScreenConfig = this.CurrentPhysicalScreenConfig;
            if (currentVirtualDisplayConfig != null
                && currentPhysicalScreenConfig != null
                && currentPhysicalScreenConfig.Type.Value == PhysicalScreenType.LED)
            {
                currentVirtualDisplayConfig.CurrentLayoutPosition = this.LayoutPosition;
            }
        }

        /// <summary>
        /// This method can be overridden to do derived class specific stuff when the dependency property
        /// "CurrentVirtualDisplay" changed.
        /// </summary>
        protected override void OnCurrentVirtualDisplayChangedInternal()
        {
            var currentVirtualDisplayConfig = this.CurrentVirtualDisplayConfig;
            var currentPhysicalScreenConfig = this.CurrentPhysicalScreenConfig;
            if (currentVirtualDisplayConfig != null
                && currentPhysicalScreenConfig != null
            && currentPhysicalScreenConfig.Type.Value == PhysicalScreenType.LED)
            {
                if (Math.Abs(currentVirtualDisplayConfig.CurrentZoomLevel) < double.Epsilon)
                {
                    this.OnZoomFitOnScreen(this.DotStep);
                    currentVirtualDisplayConfig.CurrentZoomLevel = this.Zoom;
                    currentVirtualDisplayConfig.CurrentLayoutPosition = this.LayoutPosition;
                }
                else
                {
                    this.Zoom = currentVirtualDisplayConfig.CurrentZoomLevel;
                    this.LayoutPosition = currentVirtualDisplayConfig.CurrentLayoutPosition;
                }
            }
        }

        /// <summary>
        /// This method can be overridden to do derived class specific stuff when the zoom dependency property changed.
        /// </summary>
        protected override void OnZoomChangedInternal()
        {
            var currentVirtualDisplayConfig = this.CurrentVirtualDisplayConfig;
            var currentPhysicalScreenConfig = this.CurrentPhysicalScreenConfig;
            if (currentVirtualDisplayConfig != null
                && currentPhysicalScreenConfig != null
            && currentPhysicalScreenConfig.Type.Value == PhysicalScreenType.LED)
            {
                if (Math.Abs(this.Zoom) > double.Epsilon)
                {
                    currentVirtualDisplayConfig.CurrentZoomLevel = this.Zoom;
                }
            }
        }

        private Point FitPointToDot(Point point, bool isCursorDragPosition = false)
        {
            var moduloX = point.X % this.DotStep;
            var moduloY = point.Y % this.DotStep;

            if (this.InteractionStartPosition == null)
            {
                return isCursorDragPosition
                    ? new Point(
                        point.X + (this.DotStep - moduloX - this.dotSpacing),
                        point.Y + (this.DotStep - moduloY - this.dotSpacing))
                    : new Point(
                        point.X - moduloX,
                        point.Y - moduloY);
            }

            var x = point.X - moduloX;
            if (this.InteractionStartPosition.Value.X <= point.X)
            {
                if (isCursorDragPosition)
                {
                    x += this.DotStep - this.dotSpacing;
                }
            }

            var y = point.Y - moduloY;
            if (this.InteractionStartPosition.Value.Y <= point.Y)
            {
                if (isCursorDragPosition)
                {
                    y += this.DotStep - this.dotSpacing;
                }
            }

            return new Point(x, y);
        }

        private void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var delta = e.Delta / 10.0;
            var interactionStartPosition = e.GetPosition(this.Renderer);
            this.OnMouseWheel(delta, interactionStartPosition);
        }

        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            ModifiersState modifiers;
            if (!this.IsLedToolbarActive() || !this.CanProcessKeyUp(e, out modifiers))
            {
                return;
            }

            var editor = (EditorViewModelBase)this.DataContext;
            if (this.HandleSharedKeyUp(editor, e, modifiers))
            {
                return;
            }

            switch (e.Key)
            {
                case Key.R:
                    if (this.IsLedToolbarActive())
                    {
                        this.SelectedTool = EditorToolType.Rectangle;
                    }

                    break;
            }
        }

        private void MoveElementsHandling(MouseButton changedButton, ModifiersState modifiers)
        {
            if (!this.InteractionStartPosition.HasValue || changedButton != MouseButton.Left)
            {
                return;
            }

            var dotWidth = (Settings.Default.LedDotRadius * 2) + Settings.Default.LedDotSpace;
            var deltaX = (this.MousePosition.X - this.LastMousePosition.X) / dotWidth;
            var deltaY = (this.MousePosition.Y - this.LastMousePosition.Y) / dotWidth;

            var deltaRoundX = Math.Round(deltaX);
            var deltaRoundY = Math.Round(deltaY);

            if (deltaRoundX == 0.0 && deltaRoundY == 0.0)
            {
                return;
            }

            // address rounding errors which would build up
            var deltaMissingX = (deltaX - deltaRoundX) * dotWidth;
            var deltaMissingY = (deltaY - deltaRoundY) * dotWidth;

            this.LastMousePosition = new Point(
                this.MousePosition.X - deltaMissingX,
                this.MousePosition.Y - deltaMissingY);

            var delta = new Vector(deltaRoundX, deltaRoundY);
            var parameters = new MoveElementsCommandParameters
                                 {
                                     Delta = delta,
                                     Modifiers = modifiers,
                                 };
            ((ViewModels.LedEditorViewModel)this.DataContext).MoveSelectedElementsCommand.Execute(parameters);
        }

        private void AddElementHandling(MouseButton button, bool isRect, LayoutElementType layoutElementType)
        {
            if (this.InteractionStartPosition.HasValue && button == MouseButton.Left && isRect)
            {
                var startPosition = new Point(
                    (int)(this.InteractionStartPosition.Value.X / this.DotStep),
                    (int)(this.InteractionStartPosition.Value.Y / this.DotStep));
                var bottomrightPosition = new Point(
                    (int)(this.MousePosition.X / this.DotStep) + 1,
                    (int)(this.MousePosition.Y / this.DotStep) + 1);
                if ((int)(startPosition.X - bottomrightPosition.X) == 0
                    || (int)(startPosition.Y - bottomrightPosition.Y) == 0)
                {
                    return;
                }

                var parameters = new CreateElementParameters
                {
                    Type = layoutElementType,
                    Bounds =
                        new Rect(
                        startPosition,
                        bottomrightPosition)
                };
                ((ViewModels.LedEditorViewModel)this.DataContext).CreateLayoutElementCommand.Execute(parameters);
            }
        }

        private double Distance(Point a, Point b)
        {
            var dir = b - a;
            return dir.Length;
        }

        private void LedEditor_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            this.LeftMouseButtonIsPressed = true;
        }

        private void LedEditor_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            this.LeftMouseButtonIsPressed = false;

            var currentTool = ((EditorViewModelBase)this.DataContext).Parent.SelectedEditorTool;
            if (e.ChangedButton == MouseButton.Right
                && e.RightButton == MouseButtonState.Released
                && currentTool == EditorToolType.Move)
            {
                this.DeselectAllElements();
            }
        }

        private bool IsLedToolbarActive()
        {
            var ledEditor = (ViewModels.LedEditorViewModel)this.DataContext;
            if (ledEditor == null || ledEditor.Parent.MediaApplicationState.CurrentPhysicalScreen == null)
            {
                return false;
            }

            return ledEditor.Parent.MediaApplicationState.CurrentPhysicalScreen.Type.Value == PhysicalScreenType.LED;
        }

        private void OnRightClickRenderer(object sender, MouseButtonEventArgs e)
        {
            var currentTool = ((ViewModels.LedEditorViewModel)this.DataContext).Parent.SelectedEditorTool;
            if (currentTool == EditorToolType.Zoom)
            {
                this.ZoomContextMenu.IsOpen = true;
                e.Handled = true;
            }
        }

        private void TakeScreenshot(object sender, ScreenshotEventArgs args)
        {
            // This line is needed to be sure that the renderer is completely loaded before taking the screenshot.
            this.Dispatcher.Invoke(DispatcherPriority.Loaded, new Action(() => { }));

            var actualHeight = this.Renderer.RenderSize.Height;
            var actualWidth = this.Renderer.RenderSize.Width;
            var renderHeight = actualHeight * this.Zoom / 100;
            var renderWidth = actualWidth * this.Zoom / 100;

            // Put visuals in a new DrawingVisual to be able to take a screenshot of the entire layout
            // even it is zoomed in
            var rendererBrush = new VisualBrush(this.Renderer);
            var drawingVisual = new DrawingVisual();
            var drawingContext = drawingVisual.RenderOpen();
            var scale = this.Zoom / 100;
            using (drawingContext)
            {
                drawingContext.PushTransform(new ScaleTransform(scale, scale));
                drawingContext.DrawRectangle(
                    rendererBrush, null, new Rect(new Point(0, 0), new Point(actualWidth, actualHeight)));
            }

            var renderTarget = new RenderTargetBitmap(
                (int)renderWidth, (int)renderHeight, 96, 96, PixelFormats.Pbgra32);
            renderTarget.Render(drawingVisual);
            var encoder = args.Encoder;
            encoder.Frames.Add(BitmapFrame.Create(renderTarget));
        }

        private void HandleResizeThumbMouseDown(object sender, MouseButtonEventArgs e)
        {
            var thumb = sender as FrameworkElement;
            if (thumb != null && !(thumb.Tag is ImageElementDataViewModel))
            {
                var startPosition = e.GetPosition(this);
                this.resizeController.ActivateResizeThumb(thumb, startPosition, true);
            }
        }
    }
}
