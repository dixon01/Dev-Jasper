// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LayoutEditor.xaml.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the LayoutEditor type.
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
    using Gorba.Center.Media.Core.ViewModels;
    using Gorba.Center.Media.Core.ViewModels.CommandParameters;
    using Gorba.Common.Configuration.Infomedia.Presentation;

    /// <summary>
    /// Interaction logic for LayoutEditor.xaml
    /// </summary>
    public partial class LayoutEditor
    {
        private readonly ResizeController resizeController;

        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutEditor"/> class.
        /// </summary>
        public LayoutEditor()
        {
            this.InitializeComponent();
            this.DotStep = 1;
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

                    var editor = (EditorViewModelBase)this.DataContext;
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
            this.InteractionStartPosition = e.GetPosition(this.Renderer);

            if (e.ChangedButton == MouseButton.Left)
            {
                if (this.SelectedTool == EditorToolType.Move)
                {
                    var layoutEditor = (EditorViewModelBase)this.DataContext;
                    this.CurrentElementUnderMouse =
                        layoutEditor.SelectedElements.OfType<GraphicalElementDataViewModelBase>()
                            .GetElementAt(this.InteractionStartPosition.Value);
                    this.HighestElementUnderMouse =
                        layoutEditor.Elements.GetElementAt(this.InteractionStartPosition.Value);
                    this.LastMousePosition = this.InteractionStartPosition.Value;
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
                        this.InteractionRectangle = new Rect(this.InteractionStartPosition.Value, this.MousePosition);
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
            var interationEndpoint = e.GetPosition(this.Renderer);
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
                case EditorToolType.Video:
                    this.AddElementHandling(e.ChangedButton, isRect, LayoutElementType.Video);
                    break;
                case EditorToolType.Frame:
                    this.AddElementHandling(e.ChangedButton, isRect, LayoutElementType.Frame);
                    break;
                case EditorToolType.AnalogClock:
                    this.AddElementHandling(e.ChangedButton, isRect, LayoutElementType.AnalogClock);
                    break;
                case EditorToolType.ImageList:
                    this.AddElementHandling(e.ChangedButton, isRect, LayoutElementType.ImageList);
                    break;
                case EditorToolType.RssTicker:
                    this.AddElementHandling(e.ChangedButton, isRect, LayoutElementType.RssTicker);
                    break;
                case EditorToolType.LiveStream:
                    this.AddElementHandling(e.ChangedButton, isRect, LayoutElementType.LiveStream);
                    break;
                case EditorToolType.Template:
                    this.AddElementHandling(e.ChangedButton, isRect, LayoutElementType.Template);
                    break;
            }

            this.InteractionStartPosition = null;
            this.PanStartPosition = null;
            this.InteractionRectangle = new Rect(0, 0, 0, 0);
        }

        /// <summary>
        /// The on current virtual display changed internal.
        /// </summary>
        protected override void OnCurrentVirtualDisplayChangedInternal()
        {
            var currentVirtualDisplayConfig = this.CurrentVirtualDisplayConfig;
            var currentPhysicalScreenConfig = this.CurrentPhysicalScreenConfig;
            if (currentVirtualDisplayConfig != null
                && currentPhysicalScreenConfig != null
            && currentPhysicalScreenConfig.Type.Value == PhysicalScreenType.TFT)
            {
                if (Math.Abs(currentVirtualDisplayConfig.CurrentZoomLevel) < double.Epsilon)
                {
                    this.OnZoomFitOnScreen(1);
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
        /// The on zoom changed.
        /// </summary>
        protected override void OnZoomChangedInternal()
        {
            var currentVirtualDisplayConfig = this.CurrentVirtualDisplayConfig;
            var currentPhysicalScreenConfig = this.CurrentPhysicalScreenConfig;
            if (currentVirtualDisplayConfig != null
                && currentPhysicalScreenConfig != null
            && currentPhysicalScreenConfig.Type.Value == PhysicalScreenType.TFT)
            {
                if (Math.Abs(this.Zoom) > double.Epsilon)
                {
                    currentVirtualDisplayConfig.CurrentZoomLevel = this.Zoom;
                }
            }
        }

        /// <summary>
        /// The on layout position changed internal.
        /// </summary>
        protected override void OnLayoutPositionChangedInternal()
        {
            var currentVirtualDisplayConfig = this.CurrentVirtualDisplayConfig;
            var currentPhysicalScreenConfig = this.CurrentPhysicalScreenConfig;
            if (currentVirtualDisplayConfig != null
                && currentPhysicalScreenConfig != null
                && currentPhysicalScreenConfig.Type.Value == PhysicalScreenType.TFT)
            {
                currentVirtualDisplayConfig.CurrentLayoutPosition = this.LayoutPosition;
            }
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
            if (!this.IsStandardtToolbarActive() || !this.CanProcessKeyUp(e, out modifiers))
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
                case Key.M:
                    if (this.IsStandardtToolbarActive())
                    {
                        this.SelectedTool = EditorToolType.Video;
                    }

                    break;

                case Key.F:
                    if (this.IsStandardtToolbarActive())
                    {
                        this.SelectedTool = EditorToolType.Frame;
                    }

                    break;

                case Key.L:
                    if (this.IsStandardtToolbarActive())
                    {
                        this.SelectedTool = EditorToolType.ImageList;
                    }

                    break;

                case Key.R:
                    if (this.IsStandardtToolbarActive())
                    {
                        this.SelectedTool = EditorToolType.RssTicker;
                    }

                    break;

                case Key.J:
                    if (this.IsStandardtToolbarActive())
                    {
                        this.SelectedTool = EditorToolType.LiveStream;
                    }

                    break;

                case Key.C:
                    if (this.IsStandardtToolbarActive() && !modifiers.IsControlPressed)
                    {
                        this.SelectedTool = EditorToolType.AnalogClock;
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

            var deltaX = this.MousePosition.X - this.LastMousePosition.X;
            var deltaY = this.MousePosition.Y - this.LastMousePosition.Y;
            var deltaRoundX = Math.Round(deltaX);
            var deltaRoundY = Math.Round(deltaY);

            if (deltaRoundX == 0.0 && deltaRoundY == 0.0)
            {
                return;
            }

            // address rounding errors which would build up
            var deltaMissingX = deltaX - deltaRoundX;
            var deltaMissingY = deltaY - deltaRoundY;

            this.LastMousePosition = new Point(
                this.MousePosition.X - deltaMissingX,
                this.MousePosition.Y - deltaMissingY);

            var delta = new Vector(deltaRoundX, deltaRoundY);
            var parameters = new MoveElementsCommandParameters
                                 {
                                     Delta = delta,
                                     Modifiers = modifiers,
                                 };
            ((EditorViewModelBase)this.DataContext).MoveSelectedElementsCommand.Execute(parameters);
        }

        private void AddElementHandling(MouseButton button, bool isRect, LayoutElementType layoutElementType)
        {
            if (this.InteractionStartPosition.HasValue && button == MouseButton.Left && isRect)
            {
                var parameters = new CreateElementParameters
                                 {
                                     Type = layoutElementType,
                                     Bounds =
                                         new Rect(
                                         this.InteractionStartPosition.Value,
                                         this.MousePosition)
                                 };
                ((EditorViewModelBase)this.DataContext).CreateLayoutElementCommand.Execute(parameters);
            }
        }

        private double Distance(Point a, Point b)
        {
            var dir = b - a;
            return dir.Length;
        }

        private void LayoutEditor_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            this.LeftMouseButtonIsPressed = true;
        }

        private void LayoutEditor_OnMouseUp(object sender, MouseButtonEventArgs e)
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

        private bool IsStandardtToolbarActive()
        {
            var layoutEditor = (EditorViewModelBase)this.DataContext;
            if (layoutEditor == null || layoutEditor.Parent.MediaApplicationState.CurrentPhysicalScreen == null)
            {
                return false;
            }

            return layoutEditor.Parent.MediaApplicationState.CurrentPhysicalScreen.Type.Value == PhysicalScreenType.TFT;
        }

        private void OnRightClickRenderer(object sender, MouseButtonEventArgs e)
        {
            var currentTool = ((EditorViewModelBase)this.DataContext).Parent.SelectedEditorTool;
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
            if (thumb == null)
            {
                return;
            }

            var viewModel = (LayoutElementDataViewModelBase)thumb.Tag;
            if (viewModel == null || !viewModel.IsResizable.Value)
            {
                return;
            }

            var startPosition = e.GetPosition(this);
            this.resizeController.ActivateResizeThumb(thumb, startPosition);
        }
    }
}
