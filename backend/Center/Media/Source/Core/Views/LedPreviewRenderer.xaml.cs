// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LedPreviewRenderer.xaml.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the LayoutPreviewRenderer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Views
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Threading;

    using Gorba.Center.Common.Wpf.Client.Interaction;
    using Gorba.Center.Common.Wpf.Core;
    using Gorba.Center.Common.Wpf.Framework.Interaction;
    using Gorba.Center.Media.Core.DataViewModels.Layout;
    using Gorba.Center.Media.Core.Extensions;
    using Gorba.Center.Media.Core.Interaction;
    using Gorba.Center.Media.Core.Models;
    using Gorba.Center.Media.Core.ProjectManagement;
    using Gorba.Center.Media.Core.Resources;
    using Gorba.Common.Configuration.Infomedia.AhdlcRenderer;
    using Gorba.Common.Formats.AlphaNT.Fonts;
    using Gorba.Motion.Infomedia.AhdlcRenderer;
    using Gorba.Motion.Infomedia.AhdlcRenderer.Engines;
    using Gorba.Motion.Infomedia.AhdlcRenderer.Renderer;

    using Microsoft.Practices.ServiceLocation;

    using NLog;

    using Font = Gorba.Common.Configuration.Infomedia.Layout.Font;

    /// <summary>
    /// Interaction logic for LayoutPreviewRenderer.xaml
    /// </summary>
    public partial class LedPreviewRenderer
    {
        /// <summary>
        /// The elements property.
        /// </summary>
        public static readonly DependencyProperty ElementsProperty = DependencyProperty.Register(
            "Elements",
            typeof(ExtendedObservableCollection<GraphicalElementDataViewModelBase>),
            typeof(LedPreviewRenderer),
            new PropertyMetadata(
                default(ExtendedObservableCollection<GraphicalElementDataViewModelBase>),
                OnElementsChanged));

        private readonly DispatcherTimer deferPropertyGridUpdateTimer;

        private readonly GraphicsRenderer graphicsRenderer;

        private bool isUpdating;

        private IAhdlcRenderContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="LedPreviewRenderer"/> class.
        /// </summary>
        public LedPreviewRenderer()
        {
            this.InitializeComponent();
            this.graphicsRenderer = new GraphicsRenderer(this.LedGridDisplay);
            this.LedGridDisplay.Loaded += (sender, args) => this.UpdateDisplay();
            this.deferPropertyGridUpdateTimer = new DispatcherTimer
                                                {
                                                    Interval = TimeSpan.FromMilliseconds(500),
                                                    IsEnabled = false,
                                                };
            this.deferPropertyGridUpdateTimer.Tick += this.OnTimerElapsed;
        }

        /// <summary>
        /// Gets or sets the elements.
        /// </summary>
        public ExtendedObservableCollection<GraphicalElementDataViewModelBase> Elements
        {
            get
            {
                return (ExtendedObservableCollection<GraphicalElementDataViewModelBase>)GetValue(ElementsProperty);
            }

            set
            {
                SetValue(ElementsProperty, value);
            }
        }

        /// <summary>
        /// Gets the refresh LedDisplay interaction request
        /// </summary>
        public IInteractionRequest RefreshLedDisplayRequest
        {
            get
            {
                return InteractionManager<UpdateLedDisplayPrompt>.Current.GetOrCreateInteractionRequest();
            }
        }

        /// <summary>
        /// Gets the update led display command.
        /// </summary>
        public ICommand UpdateLedDisplayCommand
        {
            get
            {
                return new RelayCommand(this.UpdateDisplay);
            }
        }

        private IAhdlcRenderContext Context
        {
            get
            {
                return this.context
                       ?? (this.context = new RenderContext((ViewModels.LedPreviewRenderer)this.DataContext));
            }
        }

        private static void OnElementsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (LedPreviewRenderer)d;
            control.Elements.CollectionChanged += control.ElementsCollectionChanged;
            foreach (var element in control.Elements)
            {
                element.PropertyChanged -= control.OnElementPropertyChanged;
                element.PropertyChanged += control.OnElementPropertyChanged;
            }

            control.UpdateDisplay();
        }

        private void OnTimerElapsed(object sender, EventArgs e)
        {
            this.deferPropertyGridUpdateTimer.Stop();
            this.isUpdating = false;
        }

        private void UpdateDisplay()
        {
            var viewModel = (ViewModels.LedPreviewRenderer)this.DataContext;
            this.LedGridDisplay.ClearDisplay();
            var isMonochrome = false;
            if (viewModel.Parent.Parent.MediaApplicationState.CurrentPhysicalScreen != null)
            {
                isMonochrome = viewModel.Parent.Parent.MediaApplicationState.CurrentPhysicalScreen.IsMonochromeScreen;
            }

            foreach (var result in this.Elements.Reverse())
            {
                var textElement = result as TextElementDataViewModel;
                var rectangleElement = result as RectangleElementDataViewModel;
                if (textElement != null && isMonochrome)
                {
                    if (textElement.Font.Color.Value.Equals(
                        "Black",
                        StringComparison.InvariantCultureIgnoreCase)
                        || textElement.Font.Color.Value.Equals("#FF000000"))
                    {
                        this.LedGridDisplay.IsInverted = true;
                    }
                    else
                    {
                        this.LedGridDisplay.IsInverted = false;
                    }
                }
                else if (rectangleElement != null && isMonochrome)
                {
                    if (rectangleElement.Color.Value.Equals(
                        "Black",
                        StringComparison.InvariantCultureIgnoreCase)
                        || rectangleElement.Color.Value.Equals("#FF000000"))
                    {
                        this.LedGridDisplay.IsInverted = true;
                    }
                    else
                    {
                        this.LedGridDisplay.IsInverted = false;
                    }
                }
                else
                {
                    this.LedGridDisplay.IsInverted = false;
                }

                try
                {
                    if (result.Visible.Value)
                    {
                        this.graphicsRenderer.Render(result.GetComponent(), this.Context);
                    }
                }
                catch (ArgumentOutOfRangeException e)
                {
                    var message = string.Format(
                        MediaStrings.LedEditor_RenderFontExceptionMessage,
                        result.ElementName.Value);
                    var errorPrompt = new ConnectionExceptionPrompt(
                        e,
                        message,
                        MediaStrings.LedEditor_RenderExceptionTitle);
                    InteractionManager<ConnectionExceptionPrompt>.Current.Raise(errorPrompt);
                }
            }
        }

        private void ElementsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null && e.OldItems.Count > 0)
            {
                foreach (var oldItem in e.OldItems)
                {
                    ((GraphicalElementDataViewModelBase)oldItem).PropertyChanged -= this.OnElementPropertyChanged;
                }
            }

            if (e.NewItems != null && e.NewItems.Count > 0)
            {
                foreach (var newItem in e.NewItems.OfType<GraphicalElementDataViewModelBase>())
                {
                    newItem.PropertyChanged += this.OnElementPropertyChanged;
                }
            }

            this.UpdateDisplay();
        }

        private void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty" || e.PropertyName == "ElementName")
            {
                return;
            }

            if (this.isUpdating
                && e.PropertyName != "X"
                && e.PropertyName != "Y"
                && e.PropertyName != "Width"
                && e.PropertyName != "Height")
            {
                return;
            }

            this.isUpdating = true;
            this.deferPropertyGridUpdateTimer.Start();
            this.UpdateDisplay();
        }

        private class RenderContext : IAhdlcRenderContext
        {
            private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

            private readonly ViewModels.LedPreviewRenderer viewModel;

            private readonly Dictionary<string, IFont> fonts = new Dictionary<string, IFont>();

            public RenderContext(ViewModels.LedPreviewRenderer viewModel)
            {
                this.viewModel = viewModel;
                this.Config = new AhdlcRendererConfig();
            }

            public long MillisecondsCounter
            {
                get
                {
                    return 0;
                }
            }

            public AhdlcRendererConfig Config { get; private set; }

            public int AlternationCounter
            {
                get
                {
                    return 0;
                }
            }

            public TimeSpan? AlternationInterval { get; set; }

            public void AddItem(ComponentBase component, bool changed)
            {
            }

            public IFont GetFont(Font font)
            {
                var applicationState = this.viewModel.Parent.Parent.MediaApplicationState;
                IFont fontFile;
                if (!this.fonts.TryGetValue(font.Face, out fontFile))
                {
                    var fontResource =
                        applicationState.CurrentProject.Resources.FirstOrDefault(
                            r => r.Type == ResourceType.Font && r.Facename == font.Face);
                    if (fontResource == null)
                    {
                        Logger.Warn("Resource for font '{0}' not found.", font.Face);
                        return null;
                    }

                    var resourceManager = ServiceLocator.Current.GetInstance<IResourceManager>();
                    fontFile = new FontFile(resourceManager.GetResourcePath(fontResource.Hash));
                    this.fonts.Add(font.Face, fontFile);
                }

                if (applicationState.CurrentPhysicalScreen.IsMonochromeScreen)
                {
                    return fontFile;
                }

                var color = font.GetColor();
                var outline = font.GetOutlineColor();
                if (!outline.HasValue)
                {
                    return new ColoredFont(fontFile, color);
                }

                var fullName = string.Format("{0}<#>{1}<$>{2}", font.Face, font.Color, font.OutlineColor);
                IFont outlineFont;
                if (!this.fonts.TryGetValue(fullName, out outlineFont))
                {
                    outlineFont = new OutlinedColoredFont(fontFile, font.GetColor(), outline.Value);
                    this.fonts.Add(fullName, outlineFont);
                }

                return outlineFont;
            }
        }
    }
}
