// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LedGridDisplay.xaml.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Interaction logic for LedGridDisplay.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Views.Controls
{
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Threading;

    using Gorba.Center.Media.Core.DataViewModels.Presentation;
    using Gorba.Common.Configuration.Infomedia.Presentation;
    using Gorba.Common.Formats.AlphaNT.Bitmaps;
    using Gorba.Common.Formats.AlphaNT.Common;
    using Gorba.Motion.Infomedia.AhdlcRenderer.Renderer;

    using Color = System.Windows.Media.Color;
    using Point = System.Windows.Point;

    /// <summary>
    /// The led grid display.
    /// </summary>
    public partial class LedGridDisplay : IGraphicsContext
    {
        /// <summary>
        /// The resolution config property.
        /// </summary>
        public static readonly DependencyProperty ResolutionConfigProperty = DependencyProperty.Register(
            "ResolutionConfig",
            typeof(VirtualDisplayConfigDataViewModel),
            typeof(LedGridDisplay),
            new PropertyMetadata(null, OnResolutionConfigChanged));

        /// <summary>
        /// The visual height property.
        /// </summary>
        public static readonly DependencyProperty VisualHeightProperty = DependencyProperty.Register(
            "VisualHeight",
            typeof(int),
            typeof(LedGridDisplay),
            new PropertyMetadata(default(int)));

        /// <summary>
        /// The visual width property.
        /// </summary>
        public static readonly DependencyProperty VisualWidthProperty = DependencyProperty.Register(
            "VisualWidth",
            typeof(int),
            typeof(LedGridDisplay),
            new PropertyMetadata(default(int)));

        /// <summary>
        /// The visual width property.
        /// </summary>
        public static readonly DependencyProperty LedRadiusProperty = DependencyProperty.Register(
            "LedRadius",
            typeof(int),
            typeof(LedGridDisplay),
            new PropertyMetadata(5, OnLedRadiusChanged));

        /// <summary>
        /// The visual width property.
        /// </summary>
        public static readonly DependencyProperty DistanceBetweenLedProperty = DependencyProperty.Register(
            "DistanceBetweenLed",
            typeof(int),
            typeof(LedGridDisplay),
            new PropertyMetadata(2, OnDistanceBetweenLedChanged));

        private SimpleBitmap blackDisplay;
        private int currentLedHeight;
        private int currentLedWidth;
        private bool isInverted;

        private LedRenderContainer ledRenderContainer;

        /// <summary>
        /// Initializes a new instance of the <see cref="LedGridDisplay"/> class.
        /// </summary>
        public LedGridDisplay()
        {
            this.InitializeComponent();
            this.Loaded += this.OnLoaded;
        }

        /// <summary>
        /// Gets or sets the color pixel source.
        /// </summary>
        public VirtualDisplayConfigDataViewModel ResolutionConfig
        {
            get
            {
                return (VirtualDisplayConfigDataViewModel)this.GetValue(ResolutionConfigProperty);
            }

            set
            {
                this.SetValue(ResolutionConfigProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the visual height.
        /// </summary>
        public int VisualHeight
        {
            get
            {
                return (int)this.GetValue(VisualHeightProperty);
            }

            set
            {
                this.SetValue(VisualHeightProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the visual width.
        /// </summary>
        public int VisualWidth
        {
            get
            {
                return (int)this.GetValue(VisualWidthProperty);
            }

            set
            {
                this.SetValue(VisualWidthProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the led radius.
        /// </summary>
        public int LedRadius
        {
            get
            {
                return (int)this.GetValue(LedRadiusProperty);
            }

            set
            {
                this.SetValue(LedRadiusProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the distance between led.
        /// </summary>
        public int DistanceBetweenLed
        {
            get
            {
                return (int)this.GetValue(DistanceBetweenLedProperty);
            }

            set
            {
                this.SetValue(DistanceBetweenLedProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the color is inverted.
        /// </summary>
        public bool IsInverted
        {
            get
            {
                return this.isInverted;
            }

            set
            {
                this.isInverted = value;
                if (this.ledRenderContainer != null)
                {
                    this.ledRenderContainer.IsInverted = value;
                }
            }
        }

        /// <summary>
        /// The draw bitmap.
        /// </summary>
        /// <param name="offsetX">
        /// The offset x.
        /// </param>
        /// <param name="offsetY">
        /// The offset y.
        /// </param>
        /// <param name="bitmap">
        /// The bitmap.
        /// </param>
        public void DrawBitmap(int offsetX, int offsetY, IBitmap bitmap)
        {
            if (this.ledRenderContainer != null)
            {
                this.ledRenderContainer.DrawLeds(offsetX, offsetY, bitmap);
            }
        }

        /// <summary>
        /// Clears the display.
        /// </summary>
        public void ClearDisplay()
        {
            this.DrawBitmap(0, 0, this.blackDisplay);
        }

        private static void OnDistanceBetweenLedChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var control = (LedGridDisplay)obj;
            control.SetupResolution(control.currentLedWidth, control.currentLedHeight);
        }

        private static void OnLedRadiusChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var control = (LedGridDisplay)obj;
            control.SetupResolution(control.currentLedWidth, control.currentLedHeight);
        }

        private static void OnResolutionConfigChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var control = (LedGridDisplay)obj;

            var resolutionHasChanged = true;
            var newResolutinConfig = (VirtualDisplayConfigDataViewModel)e.NewValue;
            if (newResolutinConfig == null)
            {
                return;
            }

            var context = (ViewModels.LedPreviewRenderer)control.DataContext;
            if (context.Parent.Parent.MediaApplicationState.CurrentPhysicalScreen == null
                || context.Parent.Parent.MediaApplicationState.CurrentPhysicalScreen.Type.Value
                != PhysicalScreenType.LED)
            {
                return;
            }

            if (control.currentLedWidth == newResolutinConfig.Width.Value
                && control.currentLedHeight == newResolutinConfig.Height.Value)
            {
                resolutionHasChanged = false;
            }

            if (resolutionHasChanged)
            {
                control.SetupResolution(newResolutinConfig.Width.Value, newResolutinConfig.Height.Value);
                control.ClearDisplay();
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (this.ResolutionConfig != null)
            {
                this.SetupResolution(this.ResolutionConfig.Width.Value, this.ResolutionConfig.Height.Value);
            }

            this.ClearDisplay();
        }

        private void SetupResolution(int width, int height)
        {
            if (this.ledRenderContainer != null)
            {
                RenderSurface.Children.Remove(this.ledRenderContainer);
            }

            if (width < 1 || height < 1)
            {
                return;
            }

            var diameter = this.LedRadius * 2;
            var diameterWithSpacing = diameter + this.DistanceBetweenLed;

            this.currentLedHeight = height;
            this.currentLedWidth = width;
            this.VisualWidth = (width * diameter) + ((width - 1) * this.DistanceBetweenLed);
            this.VisualHeight = (height * diameter) + ((height - 1) * this.DistanceBetweenLed);
            this.Width = this.VisualWidth;
            this.Height = this.VisualHeight;

            var leds = new EllipseGeometry[width, height];

            var radius = this.LedRadius;
            this.blackDisplay = new SimpleBitmap(this.currentLedWidth, this.currentLedHeight);

            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    var pos = new Point((x * diameterWithSpacing) + radius, (y * diameterWithSpacing) + radius);

                    leds[x, y] = new EllipseGeometry(pos, radius, radius);
                    this.blackDisplay.SetPixel(x, y, Gorba.Common.Formats.AlphaNT.Common.Colors.Black);
                }
            }

            var isMonochrome =
                ((ViewModels.LedPreviewRenderer)this.DataContext).Parent.Parent.MediaApplicationState
                    .CurrentPhysicalScreen.IsMonochromeScreen;
            this.ledRenderContainer = new LedRenderContainer(leds, isMonochrome);
            RenderSurface.Children.Add(this.ledRenderContainer);
        }

        /// <summary>
        /// The led render container.
        /// </summary>
        private class LedRenderContainer : FrameworkElement
        {
            private readonly GeometryDrawing[,] geometryDrawings;

            private readonly DrawingGroup ledDrawingGroup;

            private readonly ColorComparer colorComparer;

            private readonly SolidColorBrush monochromeBrush;

            private readonly SolidColorBrush blackBrush;

            private readonly bool isMonochrome;

            private Dictionary<IColor, SolidColorBrush> brushCache;

            public LedRenderContainer(EllipseGeometry[,] leds, bool isMonochrome)
            {
                this.colorComparer = new ColorComparer();
                this.isMonochrome = isMonochrome;
                this.SetupBrushCache();
                this.monochromeBrush = (SolidColorBrush)this.FindResource("LedGridDisplay_LedOnDefaultColorBrush");
                this.monochromeBrush.Freeze();
                this.blackBrush = new SolidColorBrush(Color.FromRgb(0, 0, 0));
                this.blackBrush.Freeze();
                this.ledDrawingGroup = new DrawingGroup();
                this.geometryDrawings = new GeometryDrawing[leds.GetLength(0), leds.GetLength(1)];
                for (var x = 0; x < leds.GetLength(0); x++)
                {
                    for (var y = 0; y < leds.GetLength(1); y++)
                    {
                        this.geometryDrawings[x, y] = new GeometryDrawing(null, null, leds[x, y]);
                        this.ledDrawingGroup.Children.Add(this.geometryDrawings[x, y]);
                    }
                }
            }

            public bool IsInverted { get; set; }

            public void DrawLeds(int offsetX, int offsetY, IBitmap bitmap)
            {
                var dpd = Dispatcher.CurrentDispatcher.DisableProcessing();

                for (var x = 0; x < bitmap.Width; x++)
                {
                    if ((x + offsetX > this.geometryDrawings.GetLength(0) - 1) || (x + offsetX < 0))
                    {
                        continue;
                    }

                    for (var y = 0; y < bitmap.Height; y++)
                    {
                        if ((y + offsetY > this.geometryDrawings.GetLength(1) - 1) || (y + offsetY < 0))
                        {
                            continue;
                        }

                        var color = bitmap.GetPixel(x, y);
                        if (color == Gorba.Common.Formats.AlphaNT.Common.Colors.Transparent)
                        {
                            continue;
                        }

                        SolidColorBrush brush;
                        if (this.isMonochrome)
                        {
                            if (color.R == 0 && color.G == 0 && color.B == 0)
                            {
                                brush = this.GetBrushFromCache(color);
                            }
                            else
                            {
                                if (this.IsInverted)
                                {
                                    brush = this.blackBrush;
                                }
                                else
                                {
                                    brush = this.monochromeBrush;
                                }
                            }
                        }
                        else
                        {
                            brush = this.GetBrushFromCache(color);
                        }

                        this.geometryDrawings[x + offsetX, y + offsetY].Brush = brush;
                    }
                }

                dpd.Dispose();
            }

            /// <summary>
            /// The on render.
            /// </summary>
            /// <param name="drawingContext">
            /// The drawing context.
            /// </param>
            protected override void OnRender(DrawingContext drawingContext)
            {
                base.OnRender(drawingContext);

                drawingContext.DrawDrawing(this.ledDrawingGroup);
            }

            private void SetupBrushCache()
            {
                this.brushCache = new Dictionary<IColor, SolidColorBrush>(this.colorComparer);
                var color = new SimpleColor(0, 0, 0);
                var black = new SolidColorBrush(Color.FromRgb(0, 0, 0));
                black.Freeze();
                this.brushCache.Add(color, black);
            }

            private SolidColorBrush GetBrushFromCache(IColor color)
            {
                SolidColorBrush brush;
                if (!this.brushCache.TryGetValue(color, out brush))
                {
                    brush = new SolidColorBrush(Color.FromRgb(color.R, color.G, color.B));
                    brush.Freeze();
                    this.brushCache.Add(color, brush);
                }

                return brush;
            }

            private class ColorComparer : IEqualityComparer<IColor>
            {
                public bool Equals(IColor a, IColor b)
                {
                    return a.R == b.R
                           && a.G == b.G
                           && a.B == b.B;
                }

                public int GetHashCode(IColor color)
                {
                    var hashCode = color.R.GetHashCode();
                    hashCode = (hashCode * 397) ^ color.G.GetHashCode();
                    hashCode = (hashCode * 397) ^ color.B.GetHashCode();
                    return hashCode;
                }
            }
        }
    }
}
