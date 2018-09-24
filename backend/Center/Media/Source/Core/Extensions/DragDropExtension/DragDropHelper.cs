// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DragDropHelper.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The DragDropHelper.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Extensions.DragDropExtension
{
    using System;
    using System.Collections;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    /// <summary>
    /// The DragDropReorderExtension.
    /// </summary>
    public static class DragDropHelper
    {
        #region Dependency Properties

        /// <summary>
        /// The data format
        /// </summary>
        public static readonly DataFormat DataFormat = DataFormats.GetDataFormat("DragDropData");

        /// <summary>
        /// The property for the drag adorner template
        /// </summary>
        public static readonly DependencyProperty DragAdornerTemplateProperty =
            DependencyProperty.RegisterAttached("DragAdornerTemplate", typeof(DataTemplate), typeof(DragDropHelper));

        /// <summary>
        /// The property for the drag adorner template selector
        /// </summary>
        public static readonly DependencyProperty DragAdornerTemplateSelectorProperty =
            DependencyProperty.RegisterAttached(
                "DragAdornerTemplateSelector",
                typeof(DataTemplateSelector),
                typeof(DragDropHelper),
                new PropertyMetadata(default(DataTemplateSelector)));

        /// <summary>
        /// a dependency property
        /// </summary>
        public static readonly DependencyProperty UseDefaultDragAdornerProperty =
            DependencyProperty.RegisterAttached(
                "UseDefaultDragAdorner",
                typeof(bool),
                typeof(DragDropHelper),
                new PropertyMetadata(false));

        /// <summary>
        /// a dependency property
        /// </summary>
        public static readonly DependencyProperty DefaultDragAdornerOpacityProperty =
            DependencyProperty.RegisterAttached(
                "DefaultDragAdornerOpacity",
                typeof(double),
                typeof(DragDropHelper),
                new PropertyMetadata(0.8));

        /// <summary>
        /// a dependency property
        /// </summary>
        public static readonly DependencyProperty UseDefaultEffectDataTemplateProperty =
            DependencyProperty.RegisterAttached(
                "UseDefaultEffectDataTemplate",
                typeof(bool),
                typeof(DragDropHelper),
                new PropertyMetadata(false));

        /// <summary>
        /// a dependency property
        /// </summary>
        public static readonly DependencyProperty EffectNoneAdornerTemplateProperty =
            DependencyProperty.RegisterAttached(
                "EffectNoneAdornerTemplate",
                typeof(DataTemplate),
                typeof(DragDropHelper));

        /// <summary>
        /// a dependency property
        /// </summary>
        public static readonly DependencyProperty EffectCopyAdornerTemplateProperty =
            DependencyProperty.RegisterAttached(
                "EffectCopyAdornerTemplate",
                typeof(DataTemplate),
                typeof(DragDropHelper));

        /// <summary>
        /// a dependency property
        /// </summary>
        public static readonly DependencyProperty EffectMoveAdornerTemplateProperty =
            DependencyProperty.RegisterAttached(
                "EffectMoveAdornerTemplate",
                typeof(DataTemplate),
                typeof(DragDropHelper));

        /// <summary>
        /// a dependency property
        /// </summary>
        public static readonly DependencyProperty EffectLinkAdornerTemplateProperty =
            DependencyProperty.RegisterAttached(
                "EffectLinkAdornerTemplate",
                typeof(DataTemplate),
                typeof(DragDropHelper));

        /// <summary>
        /// a dependency property
        /// </summary>
        public static readonly DependencyProperty EffectAllAdornerTemplateProperty =
            DependencyProperty.RegisterAttached(
                "EffectAllAdornerTemplate",
                typeof(DataTemplate),
                typeof(DragDropHelper));

        /// <summary>
        /// a dependency property
        /// </summary>
        public static readonly DependencyProperty IsDragSourceProperty =
            DependencyProperty.RegisterAttached(
                "IsDragSource",
                typeof(bool),
                typeof(DragDropHelper),
                new UIPropertyMetadata(false, IsDragSourceChanged));

        /// <summary>
        /// a dependency property
        /// </summary>
        public static readonly DependencyProperty IsDropTargetProperty =
            DependencyProperty.RegisterAttached(
                "IsDropTarget",
                typeof(bool),
                typeof(DragDropHelper),
                new UIPropertyMetadata(false, IsDropTargetChanged));

        /// <summary>
        /// a dependency property
        /// </summary>
        public static readonly DependencyProperty DragHandlerProperty =
            DependencyProperty.RegisterAttached("DragHandler", typeof(IDragSource), typeof(DragDropHelper));

        /// <summary>
        /// a dependency property
        /// </summary>
        public static readonly DependencyProperty DropHandlerProperty =
            DependencyProperty.RegisterAttached("DropHandler", typeof(IDropTarget), typeof(DragDropHelper));

        /// <summary>
        /// a dependency property
        /// </summary>
        public static readonly DependencyProperty DragSourceIgnoreProperty =
            DependencyProperty.RegisterAttached(
                "DragSourceIgnore",
                typeof(bool),
                typeof(DragDropHelper),
                new PropertyMetadata(false));

        /// <summary>
        /// DragMouseAnchorPoint defines the horizontal and vertical proportion 
        /// at which the pointer will anchor on the DragAdorner.
        /// </summary>
        public static readonly DependencyProperty DragMouseAnchorPointProperty =
            DependencyProperty.RegisterAttached(
                "DragMouseAnchorPoint",
                typeof(Point),
                typeof(DragDropHelper),
                new PropertyMetadata(new Point(0, 1)));

        /// <summary>
        /// a dependency property
        /// </summary>
        public static readonly DependencyProperty EffectScrollAdornerTemplateProperty =
            DependencyProperty.RegisterAttached(
                "EffectScrollAdornerTemplate",
                typeof(DataTemplate),
                typeof(DragDropHelper));

        #endregion

        #region fields

        private static IDragSource defaultDragHandler;
        private static IDropTarget defaultDropHandler;
        private static DragAdorner dragAdorner;
        private static DragAdorner effectAdorner;
        private static DragInfo dragInfo;
        private static bool dragInProgress;
        private static DropTargetAdorner dropTargetAdorner;
        private static object clickSupressItem;
        private static Point adornerPos;
        private static Size adornerSize;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the default drag handler
        /// </summary>
        public static IDragSource DefaultDragHandler
        {
            get
            {
                if (defaultDragHandler == null)
                {
                    defaultDragHandler = new DefaultDragHandler();
                }

                return defaultDragHandler;
            }

            set
            {
                defaultDragHandler = value;
            }
        }

        /// <summary>
        /// Gets or sets the default drop handler
        /// </summary>
        public static IDropTarget DefaultDropHandler
        {
            get
            {
                if (defaultDropHandler == null)
                {
                    defaultDropHandler = new DefaultDropHandler();
                }

                return defaultDropHandler;
            }

            set
            {
                defaultDropHandler = value;
            }
        }

        private static DragAdorner DragAdorner
        {
            get
            {
                return dragAdorner;
            }

            set
            {
                if (dragAdorner != null)
                {
                    dragAdorner.Detatch();
                }

                dragAdorner = value;
            }
        }

        private static DragAdorner EffectAdorner
        {
            get
            {
                return effectAdorner;
            }

            set
            {
                if (effectAdorner != null)
                {
                    effectAdorner.Detatch();
                }

                effectAdorner = value;
            }
        }

        private static DropTargetAdorner DropTargetAdorner
        {
            get
            {
                return dropTargetAdorner;
            }

            set
            {
                if (dropTargetAdorner != null)
                {
                    dropTargetAdorner.Detach();
                }

                dropTargetAdorner = value;
            }
        }

        #endregion

        #region Accessors

        /// <summary>
        /// The method responsible for retrieving the drag adorner template
        /// </summary>
        /// <param name="target">the element target</param>
        /// <returns>the data template</returns>
        public static DataTemplate GetDragAdornerTemplate(UIElement target)
        {
            return (DataTemplate)target.GetValue(DragAdornerTemplateProperty);
        }

        /// <summary>
        /// The method responsible for changing the drag adorner template
        /// </summary>
        /// <param name="target">the target element</param>
        /// <param name="value">the data template</param>
        public static void SetDragAdornerTemplate(UIElement target, DataTemplate value)
        {
            target.SetValue(DragAdornerTemplateProperty, value);
        }

        /// <summary>
        /// The setter for drag adorner template selector
        /// </summary>
        /// <param name="element">the element</param>
        /// <param name="value">the value</param>
        public static void SetDragAdornerTemplateSelector(DependencyObject element, DataTemplateSelector value)
        {
            element.SetValue(DragAdornerTemplateSelectorProperty, value);
        }

        /// <summary>
        /// the getter for drag adorner template selector
        /// </summary>
        /// <param name="element">the element</param>
        /// <returns>the data template selector</returns>
        public static DataTemplateSelector GetDragAdornerTemplateSelector(DependencyObject element)
        {
            return (DataTemplateSelector)element.GetValue(DragAdornerTemplateSelectorProperty);
        }

        /// <summary>
        /// The Getter for the default drag adorner
        /// </summary>
        /// <param name="target">the target</param>
        /// <returns>a boolean</returns>
        public static bool GetUseDefaultDragAdorner(UIElement target)
        {
            return (bool)target.GetValue(UseDefaultDragAdornerProperty);
        }

        /// <summary>
        /// The setter for the default drag adorner
        /// </summary>
        /// <param name="target">the target</param>
        /// <param name="value">a boolean</param>
        public static void SetUseDefaultDragAdorner(UIElement target, bool value)
        {
            target.SetValue(UseDefaultDragAdornerProperty, value);
        }

        /// <summary>
        /// The Getter for the default drag adorner opacity
        /// </summary>
        /// <param name="target">the target</param>
        /// <returns>a double</returns>
        public static double GetDefaultDragAdornerOpacity(UIElement target)
        {
            return (double)target.GetValue(DefaultDragAdornerOpacityProperty);
        }

        /// <summary>
        /// The setter for the default drag adorner opacity
        /// </summary>
        /// <param name="target">the target</param>
        /// <param name="value">a double</param>
        public static void SetDefaultDragAdornerOpacity(UIElement target, double value)
        {
            target.SetValue(DefaultDragAdornerOpacityProperty, value);
        }

        /// <summary>
        /// The Getter for the use default effect data template
        /// </summary>
        /// <param name="target">the target</param>
        /// <returns>a boolean</returns>
        public static bool GetUseDefaultEffectDataTemplate(UIElement target)
        {
            return (bool)target.GetValue(UseDefaultEffectDataTemplateProperty);
        }

        /// <summary>
        /// The setter for the use default effect data template
        /// </summary>
        /// <param name="target">the target</param>
        /// <param name="value">the value</param>
        public static void SetUseDefaultEffectDataTemplate(UIElement target, bool value)
        {
            target.SetValue(UseDefaultEffectDataTemplateProperty, value);
        }

        /// <summary>
        /// a getter 
        /// </summary>
        /// <param name="target">the target</param>
        /// <returns>the data template</returns>
        public static DataTemplate GetEffectNoneAdornerTemplate(UIElement target)
        {
            var template = (DataTemplate)target.GetValue(EffectNoneAdornerTemplateProperty);

            if (template == null)
            {
                if (!GetUseDefaultEffectDataTemplate(target))
                {
                    return null;
                }

                var imageSourceFactory = new FrameworkElementFactory(typeof(Image));
                imageSourceFactory.SetValue(Image.SourceProperty, IconFactory.EffectNone);
                imageSourceFactory.SetValue(FrameworkElement.HeightProperty, 12.0);
                imageSourceFactory.SetValue(FrameworkElement.WidthProperty, 12.0);

                template = new DataTemplate { VisualTree = imageSourceFactory };
            }

            return template;
        }

        /// <summary>
        /// a setter
        /// </summary>
        /// <param name="target">the target</param>
        /// <param name="value">the value</param>
        public static void SetEffectNoneAdornerTemplate(UIElement target, DataTemplate value)
        {
            target.SetValue(EffectNoneAdornerTemplateProperty, value);
        }

        /// <summary>
        /// a getter 
        /// </summary>
        /// <param name="target">the target</param>
        /// <param name="destinationText">the destination text</param>
        /// <returns>the data template</returns>
        public static DataTemplate GetEffectCopyAdornerTemplate(UIElement target, string destinationText)
        {
            var template = (DataTemplate)target.GetValue(EffectCopyAdornerTemplateProperty);

            if (template == null)
            {
                template = CreateDefaultEffectDataTemplate(target, IconFactory.EffectCopy, "Copy to", destinationText);
            }

            return template;
        }

        /// <summary>
        /// a setter
        /// </summary>
        /// <param name="target">the target</param>
        /// <param name="value">the value</param>
        public static void SetEffectCopyAdornerTemplate(UIElement target, DataTemplate value)
        {
            target.SetValue(EffectCopyAdornerTemplateProperty, value);
        }

        /// <summary>
        /// a getter 
        /// </summary>
        /// <param name="target">the target</param>
        /// <param name="destinationText">the destination text</param>
        /// <returns>the data template</returns>
        public static DataTemplate GetEffectMoveAdornerTemplate(UIElement target, string destinationText)
        {
            var template = (DataTemplate)target.GetValue(EffectMoveAdornerTemplateProperty);

            if (template == null)
            {
                template = CreateDefaultEffectDataTemplate(target, IconFactory.EffectMove, "Move to", destinationText);
            }

            return template;
        }

        /// <summary>
        /// a setter
        /// </summary>
        /// <param name="target">the target</param>
        /// <param name="value">the value</param>
        public static void SetEffectMoveAdornerTemplate(UIElement target, DataTemplate value)
        {
            target.SetValue(EffectMoveAdornerTemplateProperty, value);
        }

        /// <summary>
        /// a getter 
        /// </summary>
        /// <param name="target">the target</param>
        /// <param name="destinationText">the destination text</param>
        /// <returns>the data template</returns>
        public static DataTemplate GetEffectLinkAdornerTemplate(UIElement target, string destinationText)
        {
            var template = (DataTemplate)target.GetValue(EffectLinkAdornerTemplateProperty);

            if (template == null)
            {
                template = CreateDefaultEffectDataTemplate(target, IconFactory.EffectLink, "Link to", destinationText);
            }

            return template;
        }

        /// <summary>
        /// a setter
        /// </summary>
        /// <param name="target">the target</param>
        /// <param name="value">the value</param>
        public static void SetEffectLinkAdornerTemplate(UIElement target, DataTemplate value)
        {
            target.SetValue(EffectLinkAdornerTemplateProperty, value);
        }

        /// <summary>
        /// a getter 
        /// </summary>
        /// <param name="target">the target</param>
        /// <returns>the data template</returns>
        public static DataTemplate GetEffectAllAdornerTemplate(UIElement target)
        {
            var template = (DataTemplate)target.GetValue(EffectAllAdornerTemplateProperty);

            // TODO: Add default template
            return template;
        }

        /// <summary>
        /// a setter
        /// </summary>
        /// <param name="target">the target</param>
        /// <param name="value">the value</param>
        public static void SetEffectAllAdornerTemplate(UIElement target, DataTemplate value)
        {
            target.SetValue(EffectAllAdornerTemplateProperty, value);
        }

        /// <summary>
        /// a getter 
        /// </summary>
        /// <param name="target">the target</param>
        /// <returns>the data template</returns>
        public static DataTemplate GetEffectScrollAdornerTemplate(UIElement target)
        {
            var template = (DataTemplate)target.GetValue(EffectScrollAdornerTemplateProperty);

            // TODO: Add default template
            return template;
        }

        /// <summary>
        /// a setter
        /// </summary>
        /// <param name="target">the target</param>
        /// <param name="value">the value</param>
        public static void SetEffectScrollAdornerTemplate(UIElement target, DataTemplate value)
        {
            target.SetValue(EffectScrollAdornerTemplateProperty, value);
        }

        /// <summary>
        /// a getter 
        /// </summary>
        /// <param name="target">the target</param>
        /// <returns>a boolean</returns>
        public static bool GetIsDragSource(UIElement target)
        {
            return (bool)target.GetValue(IsDragSourceProperty);
        }

        /// <summary>
        /// a setter
        /// </summary>
        /// <param name="target">the target</param>
        /// <param name="value">the value</param>
        public static void SetIsDragSource(UIElement target, bool value)
        {
            target.SetValue(IsDragSourceProperty, value);
        }

        /// <summary>
        /// a getter 
        /// </summary>
        /// <param name="target">the target</param>
        /// <returns>a boolean</returns>
        public static bool GetIsDropTarget(UIElement target)
        {
            return (bool)target.GetValue(IsDropTargetProperty);
        }

        /// <summary>
        /// a setter
        /// </summary>
        /// <param name="target">the target</param>
        /// <param name="value">the value</param>
        public static void SetIsDropTarget(UIElement target, bool value)
        {
            target.SetValue(IsDropTargetProperty, value);
        }

        /// <summary>
        /// a getter 
        /// </summary>
        /// <param name="target">the target</param>
        /// <returns>the drag source</returns>
        public static IDragSource GetDragHandler(UIElement target)
        {
            return (IDragSource)target.GetValue(DragHandlerProperty);
        }

        /// <summary>
        /// a setter
        /// </summary>
        /// <param name="target">the target</param>
        /// <param name="value">the value</param>
        public static void SetDragHandler(UIElement target, IDragSource value)
        {
            target.SetValue(DragHandlerProperty, value);
        }

        /// <summary>
        /// a getter 
        /// </summary>
        /// <param name="target">the target</param>
        /// <returns>the drop target</returns>
        public static IDropTarget GetDropHandler(UIElement target)
        {
            return (IDropTarget)target.GetValue(DropHandlerProperty);
        }

        /// <summary>
        /// a setter
        /// </summary>
        /// <param name="target">the target</param>
        /// <param name="value">the value</param>
        public static void SetDropHandler(UIElement target, IDropTarget value)
        {
            target.SetValue(DropHandlerProperty, value);
        }

        /// <summary>
        /// a getter 
        /// </summary>
        /// <param name="target">the target</param>
        /// <returns>a boolean</returns>
        public static bool GetDragSourceIgnore(UIElement target)
        {
            return (bool)target.GetValue(DragSourceIgnoreProperty);
        }

        /// <summary>
        /// a setter
        /// </summary>
        /// <param name="target">the target</param>
        /// <param name="value">the value</param>
        public static void SetDragSourceIgnore(UIElement target, bool value)
        {
            target.SetValue(DragSourceIgnoreProperty, value);
        }

        /// <summary>
        /// a getter 
        /// </summary>
        /// <param name="target">the target</param>
        /// <returns>a point</returns>
        public static Point GetDragMouseAnchorPoint(UIElement target)
        {
            return (Point)target.GetValue(DragMouseAnchorPointProperty);
        }

        /// <summary>
        /// a setter
        /// </summary>
        /// <param name="target">the target</param>
        /// <param name="value">the value</param>
        public static void SetDragMouseAnchorPoint(UIElement target, Point value)
        {
            target.SetValue(DragMouseAnchorPointProperty, value);
        }

        #endregion

        private static void IsDragSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var uiElement = (UIElement)d;

            if ((bool)e.NewValue)
            {
                uiElement.PreviewMouseLeftButtonDown += DragSourcePreviewMouseLeftButtonDown;
                uiElement.PreviewMouseLeftButtonUp += DragSourcePreviewMouseLeftButtonUp;
                uiElement.PreviewMouseMove += DragSourcePreviewMouseMove;
                uiElement.QueryContinueDrag += DragSourceQueryContinueDrag;
            }
            else
            {
                uiElement.PreviewMouseLeftButtonDown -= DragSourcePreviewMouseLeftButtonDown;
                uiElement.PreviewMouseLeftButtonUp -= DragSourcePreviewMouseLeftButtonUp;
                uiElement.PreviewMouseMove -= DragSourcePreviewMouseMove;
                uiElement.QueryContinueDrag -= DragSourceQueryContinueDrag;
            }
        }

        private static void IsDropTargetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var uiElement = (UIElement)d;

            if ((bool)e.NewValue)
            {
                uiElement.AllowDrop = true;

                if (uiElement is ItemsControl)
                {
                    // use normal events for ItemsControls
                    uiElement.DragEnter += DropTargetPreviewDragEnter;
                    uiElement.DragLeave += DropTargetPreviewDragLeave;
                    uiElement.DragOver += DropTargetPreviewDragOver;
                    uiElement.Drop += DropTargetPreviewDrop;
                    uiElement.GiveFeedback += DropTargetGiveFeedback;
                }
                else
                {
                    // issue #85: try using preview events for all other elements than ItemsControls
                    uiElement.PreviewDragEnter += DropTargetPreviewDragEnter;
                    uiElement.PreviewDragLeave += DropTargetPreviewDragLeave;
                    uiElement.PreviewDragOver += DropTargetPreviewDragOver;
                    uiElement.PreviewDrop += DropTargetPreviewDrop;
                    uiElement.PreviewGiveFeedback += DropTargetGiveFeedback;
                }
            }
            else
            {
                uiElement.AllowDrop = false;

                if (uiElement is ItemsControl)
                {
                    uiElement.DragEnter -= DropTargetPreviewDragEnter;
                    uiElement.DragLeave -= DropTargetPreviewDragLeave;
                    uiElement.DragOver -= DropTargetPreviewDragOver;
                    uiElement.Drop -= DropTargetPreviewDrop;
                    uiElement.GiveFeedback -= DropTargetGiveFeedback;
                }
                else
                {
                    uiElement.PreviewDragEnter -= DropTargetPreviewDragEnter;
                    uiElement.PreviewDragLeave -= DropTargetPreviewDragLeave;
                    uiElement.PreviewDragOver -= DropTargetPreviewDragOver;
                    uiElement.PreviewDrop -= DropTargetPreviewDrop;
                    uiElement.PreviewGiveFeedback -= DropTargetGiveFeedback;
                }

                Mouse.OverrideCursor = null;
            }
        }

        private static void CreateDragAdorner()
        {
            var template = GetDragAdornerTemplate(dragInfo.VisualSource);
            var templateSelector = GetDragAdornerTemplateSelector(dragInfo.VisualSource);

            UIElement adornment = null;

            var useDefaultDragAdorner = GetUseDefaultDragAdorner(dragInfo.VisualSource);

            if (template == null && templateSelector == null && useDefaultDragAdorner)
            {
                template = new DataTemplate();

                var factory = new FrameworkElementFactory(typeof(Image));

                var bs = CaptureScreen(dragInfo.VisualSourceItem, dragInfo.VisualSourceFlowDirection);
                factory.SetValue(Image.SourceProperty, bs);
                factory.SetValue(RenderOptions.EdgeModeProperty, EdgeMode.Aliased);
                factory.SetValue(RenderOptions.BitmapScalingModeProperty, BitmapScalingMode.HighQuality);
                if (dragInfo.VisualSourceItem is FrameworkElement)
                {
                    factory.SetValue(
                        FrameworkElement.WidthProperty,
                        ((FrameworkElement)dragInfo.VisualSourceItem).ActualWidth);
                    factory.SetValue(
                        FrameworkElement.HeightProperty,
                        ((FrameworkElement)dragInfo.VisualSourceItem).ActualHeight);
                    factory.SetValue(FrameworkElement.HorizontalAlignmentProperty, HorizontalAlignment.Left);
                    factory.SetValue(FrameworkElement.VerticalAlignmentProperty, VerticalAlignment.Top);
                }

                template.VisualTree = factory;
            }

            if (template != null || templateSelector != null)
            {
                if (dragInfo.Data is IEnumerable && !(dragInfo.Data is string))
                {
                    if (((IEnumerable)dragInfo.Data).Cast<object>().Count() <= 10)
                    {
                        var itemsControl = new ItemsControl
                                           {
                                               ItemsSource = (IEnumerable)dragInfo.Data,
                                               ItemTemplate = template,
                                               ItemTemplateSelector = templateSelector
                                           };

                        // The ItemsControl doesn't display unless we create a border to contain it.
                        // Not quite sure why this is...
                        var border = new Border { Child = itemsControl };
                        adornment = border;
                    }
                }
                else
                {
                    var contentPresenter = new ContentPresenter
                                           {
                                               Content = dragInfo.Data,
                                               ContentTemplate = template,
                                               ContentTemplateSelector = templateSelector
                                           };
                    adornment = contentPresenter;
                }
            }

            if (adornment != null)
            {
                if (useDefaultDragAdorner)
                {
                    adornment.Opacity = GetDefaultDragAdornerOpacity(dragInfo.VisualSource);
                }

                var parentWindow = dragInfo.VisualSource.GetVisualAncestor<Window>();
                var rootElement = parentWindow != null ? parentWindow.Content as UIElement : null;
                if (rootElement == null && Application.Current != null && Application.Current.MainWindow != null)
                {
                    rootElement = (UIElement)Application.Current.MainWindow.Content;
                }

                // i don't want the windows forms reference
                // if (rootElement == null) {
                //     var elementHost = m_DragInfo.VisualSource.GetVisualAncestor<ElementHost>();
                //     rootElement = elementHost != null ? elementHost.Child : null;
                // }
                if (rootElement == null)
                {
                    rootElement = dragInfo.VisualSource.GetVisualAncestor<UserControl>();
                }

                DragAdorner = new DragAdorner(rootElement, adornment);
            }
        }

        private static BitmapSource CaptureScreen(
            Visual target, 
            FlowDirection flowDirection, 
            double dpiX = 96.0, 
            double dpiY = 96.0)
        {
            if (target == null)
            {
                return null;
            }

            var bounds = VisualTreeHelper.GetDescendantBounds(target);

            var rtb = new RenderTargetBitmap(
                (int)(bounds.Width * dpiX / 96.0),
                (int)(bounds.Height * dpiY / 96.0),
                dpiX,
                dpiY,
                PixelFormats.Pbgra32);

            var dv = new DrawingVisual();
            using (var ctx = dv.RenderOpen())
            {
                var vb = new VisualBrush(target);
                if (flowDirection == FlowDirection.RightToLeft)
                {
                    var transformGroup = new TransformGroup();
                    transformGroup.Children.Add(new ScaleTransform(-1, 1));
                    transformGroup.Children.Add(new TranslateTransform(bounds.Size.Width - 1, 0));
                    ctx.PushTransform(transformGroup);
                }

                ctx.DrawRectangle(vb, null, new Rect(new Point(), bounds.Size));
            }

            rtb.Render(dv);

            return rtb;
        }

        private static void CreateEffectAdorner(DropInfo dropInfo)
        {
            var template = GetEffectAdornerTemplate(dragInfo.VisualSource, dropInfo.Effects, dropInfo.DestinationText);

            if (template != null)
            {
                var rootElement = (UIElement)Application.Current.MainWindow.Content;

                var contentPresenter = new ContentPresenter { Content = dragInfo.Data, ContentTemplate = template };

                UIElement adornment = contentPresenter;

                EffectAdorner = new DragAdorner(rootElement, adornment);
            }
        }

        private static DataTemplate CreateDefaultEffectDataTemplate(
            UIElement target, 
            BitmapImage effectIcon, 
            string effectText, 
            string destinationText)
        {
            if (!GetUseDefaultEffectDataTemplate(target))
            {
                return null;
            }

            // Add icon
            var imageFactory = new FrameworkElementFactory(typeof(Image));
            imageFactory.SetValue(Image.SourceProperty, effectIcon);
            imageFactory.SetValue(FrameworkElement.HeightProperty, 12.0);
            imageFactory.SetValue(FrameworkElement.WidthProperty, 12.0);
            imageFactory.SetValue(FrameworkElement.MarginProperty, new Thickness(0.0, 0.0, 3.0, 0.0));

            // Add effect text
            var effectTextBlockFactory = new FrameworkElementFactory(typeof(TextBlock));
            effectTextBlockFactory.SetValue(TextBlock.TextProperty, effectText);
            effectTextBlockFactory.SetValue(TextBlock.FontSizeProperty, 11.0);
            effectTextBlockFactory.SetValue(TextBlock.ForegroundProperty, Brushes.Blue);

            // Add destination text
            var destinationTextBlockFactory = new FrameworkElementFactory(typeof(TextBlock));
            destinationTextBlockFactory.SetValue(TextBlock.TextProperty, destinationText);
            destinationTextBlockFactory.SetValue(TextBlock.FontSizeProperty, 11.0);
            destinationTextBlockFactory.SetValue(TextBlock.ForegroundProperty, Brushes.DarkBlue);
            destinationTextBlockFactory.SetValue(TextBlock.MarginProperty, new Thickness(3, 0, 0, 0));
            destinationTextBlockFactory.SetValue(TextBlock.FontWeightProperty, FontWeights.DemiBold);

            // Create containing panel
            var stackPanelFactory = new FrameworkElementFactory(typeof(StackPanel));
            stackPanelFactory.SetValue(StackPanel.OrientationProperty, Orientation.Horizontal);
            stackPanelFactory.SetValue(FrameworkElement.MarginProperty, new Thickness(2.0));
            stackPanelFactory.AppendChild(imageFactory);
            stackPanelFactory.AppendChild(effectTextBlockFactory);
            stackPanelFactory.AppendChild(destinationTextBlockFactory);

            // Add border
            var borderFactory = new FrameworkElementFactory(typeof(Border));
            var stopCollection = new GradientStopCollection
                                 {
                                     new GradientStop(Colors.White, 0.0),
                                     new GradientStop(Colors.AliceBlue, 1.0)
                                 };
            var gradientBrush = new LinearGradientBrush(stopCollection)
            {
                StartPoint = new Point(0, 0),
                EndPoint = new Point(0, 1)
            };
            borderFactory.SetValue(Panel.BackgroundProperty, gradientBrush);
            borderFactory.SetValue(Border.BorderBrushProperty, Brushes.DimGray);
            borderFactory.SetValue(Border.CornerRadiusProperty, new CornerRadius(3.0));
            borderFactory.SetValue(Border.BorderThicknessProperty, new Thickness(1.0));
            borderFactory.AppendChild(stackPanelFactory);

            // Finally add content to template
            var template = new DataTemplate { VisualTree = borderFactory };
            return template;
        }

        private static DataTemplate GetEffectAdornerTemplate(
            UIElement target, 
            DragDropEffects effect, 
            string destinationText)
        {
            switch (effect)
            {
                case DragDropEffects.All:
                    return null;
                case DragDropEffects.Copy:
                    return GetEffectCopyAdornerTemplate(target, destinationText);
                case DragDropEffects.Link:
                    return GetEffectLinkAdornerTemplate(target, destinationText);
                case DragDropEffects.Move:
                    return GetEffectMoveAdornerTemplate(target, destinationText);
                case DragDropEffects.None:
                    return GetEffectNoneAdornerTemplate(target);
                case DragDropEffects.Scroll:
                    return null;
                default:
                    return null;
            }
        }

        private static void Scroll(DependencyObject o, DragEventArgs e)
        {
            var scrollViewer = o.GetVisualDescendent<ScrollViewer>();

            if (scrollViewer != null)
            {
                var position = e.GetPosition(scrollViewer);
                var scrollMargin = Math.Min(scrollViewer.FontSize * 2, scrollViewer.ActualHeight / 2);

                if (position.X >= scrollViewer.ActualWidth - scrollMargin &&
                    scrollViewer.HorizontalOffset < scrollViewer.ExtentWidth - scrollViewer.ViewportWidth)
                {
                    scrollViewer.LineRight();
                }
                else if (position.X < scrollMargin && scrollViewer.HorizontalOffset > 0)
                {
                    scrollViewer.LineLeft();
                }
                else if (position.Y >= scrollViewer.ActualHeight - scrollMargin &&
                         scrollViewer.VerticalOffset < scrollViewer.ExtentHeight - scrollViewer.ViewportHeight)
                {
                    scrollViewer.LineDown();
                }
                else if (position.Y < scrollMargin && scrollViewer.VerticalOffset > 0)
                {
                    scrollViewer.LineUp();
                }
            }
        }

        private static IDragSource TryGetDragHandler(DragInfo dragInfoToBeSearched, UIElement sender)
        {
            IDragSource dragHandler = null;
            if (dragInfoToBeSearched != null && dragInfoToBeSearched.VisualSource != null)
            {
                dragHandler = GetDragHandler(dragInfoToBeSearched.VisualSource);
            }

            if (dragHandler == null && sender != null)
            {
                dragHandler = GetDragHandler(sender);
            }

            return dragHandler ?? DefaultDragHandler;
        }

        private static void DragSourcePreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Ignore the click if clickCount != 1 or the user has clicked on a scrollbar.
            var elementPosition = e.GetPosition((IInputElement)sender);
            if (e.ClickCount != 1
                || HitTestUtilities.HitTest4Type<RangeBase>(sender, elementPosition)
                || HitTestUtilities.HitTest4Type<TextBoxBase>(sender, elementPosition)
                || HitTestUtilities.HitTest4Type<PasswordBox>(sender, elementPosition)
                || HitTestUtilities.HitTest4Type<ComboBox>(sender, elementPosition)
                || HitTestUtilities.HitTest4GridViewColumnHeader(sender, elementPosition)
                || HitTestUtilities.HitTest4DataGridTypes(sender, elementPosition)
                || HitTestUtilities.IsNotPartOfSender(sender, e)
                || GetDragSourceIgnore((UIElement)sender))
            {
                dragInfo = null;
                return;
            }

            dragInfo = new DragInfo(sender, e);

            var dragHandler = TryGetDragHandler(dragInfo, sender as UIElement);
            if (!dragHandler.CanStartDrag(dragInfo))
            {
                dragInfo = null;
                return;
            }

            // If the sender is a list box that allows multiple selections, ensure that clicking on an 
            // already selected item does not change the selection, otherwise dragging multiple items 
            // is made impossible.
            var itemsControl = sender as ItemsControl;

            if (dragInfo.VisualSourceItem != null && itemsControl != null && itemsControl.CanSelectMultipleItems())
            {
                var selectedItems = itemsControl.GetSelectedItems().Cast<object>().ToList();

                if (selectedItems.Count() > 1 && selectedItems.Contains(dragInfo.SourceItem))
                {
                    clickSupressItem = dragInfo.SourceItem;
                    e.Handled = true;
                }
            }
        }

        private static void DragSourcePreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // If we prevented the control's default selection handling in DragSource_PreviewMouseLeftButtonDown
            // by setting 'e.Handled = true' and a drag was not initiated, manually set the selection here.
            var itemsControl = sender as ItemsControl;

            if (itemsControl != null && dragInfo != null && clickSupressItem == dragInfo.SourceItem)
            {
                if ((Keyboard.Modifiers & ModifierKeys.Control) != 0)
                {
                    itemsControl.SetItemSelected(dragInfo.SourceItem, false);
                }
                else
                {
                    itemsControl.SetSelectedItem(dragInfo.SourceItem);
                }
            }

            dragInfo = null;

            clickSupressItem = null;
        }

        private static void DragSourcePreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (dragInfo != null && !dragInProgress)
            {
                var dragStart = dragInfo.DragStartPosition;
                var position = e.GetPosition((IInputElement)sender);

                if (Math.Abs(position.X - dragStart.X) > SystemParameters.MinimumHorizontalDragDistance ||
                    Math.Abs(position.Y - dragStart.Y) > SystemParameters.MinimumVerticalDragDistance)
                {
                    var dragHandler = TryGetDragHandler(dragInfo, sender as UIElement);
                    if (dragHandler.CanStartDrag(dragInfo))
                    {
                        dragHandler.StartDrag(dragInfo);

                        if (dragInfo.Effects != DragDropEffects.None && dragInfo.Data != null)
                        {
                            var data = dragInfo.DataObject;

                            if (data == null)
                            {
                                data = new DataObject(DataFormat.Name, dragInfo.Data);
                            }
                            else
                            {
                                data.SetData(DataFormat.Name, dragInfo.Data);
                            }

                            try
                            {
                                dragInProgress = true;
                                var result = System.Windows.DragDrop.DoDragDrop(
                                    dragInfo.VisualSource,
                                    data,
                                    dragInfo.Effects);
                                if (result == DragDropEffects.None)
                                {
                                    dragHandler.DragCancelled();
                                }
                            }
                            finally
                            {
                                dragInProgress = false;
                            }

                            dragInfo = null;
                        }
                    }
                }
            }
        }

        private static void DragSourceQueryContinueDrag(object sender, QueryContinueDragEventArgs e)
        {
            if (e.Action == DragAction.Cancel || e.EscapePressed)
            {
                DragAdorner = null;
                EffectAdorner = null;
                DropTargetAdorner = null;
            }
        }

        private static void DropTargetPreviewDragEnter(object sender, DragEventArgs e)
        {
            DropTargetPreviewDragOver(sender, e);

            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private static void DropTargetPreviewDragLeave(object sender, DragEventArgs e)
        {
            DragAdorner = null;
            EffectAdorner = null;
            DropTargetAdorner = null;

            Mouse.OverrideCursor = null;
        }

        private static void DropTargetPreviewDragOver(object sender, DragEventArgs e)
        {
            var elementPosition = e.GetPosition((IInputElement)sender);
            if (HitTestUtilities.HitTest4Type<ScrollBar>(sender, elementPosition)
                || HitTestUtilities.HitTest4GridViewColumnHeader(sender, elementPosition)
                || HitTestUtilities.HitTest4DataGridTypesOnDragOver(sender, elementPosition))
            {
                e.Effects = DragDropEffects.None;
                e.Handled = true;
                return;
            }

            var dropInfo = new DropInfo(sender, e, dragInfo);
            var dropHandler = GetDropHandler((UIElement)sender) ?? DefaultDropHandler;
            var itemsControl = dropInfo.VisualTarget;

            dropHandler.DragOver(dropInfo);

            if (DragAdorner == null && dragInfo != null)
            {
                CreateDragAdorner();
            }

            if (DragAdorner != null)
            {
                var tempAdornerPos = e.GetPosition(DragAdorner.AdornedElement);

                if (tempAdornerPos.X > 0 && tempAdornerPos.Y > 0)
                {
                    adornerPos = tempAdornerPos;
                }

                // Fixed the flickering adorner - Size changes to zero 'randomly'...?
                if (DragAdorner.RenderSize.Width > 0 && DragAdorner.RenderSize.Height > 0)
                {
                    adornerSize = DragAdorner.RenderSize;
                }

                if (dragInfo != null)
                {
                    // move the adorner
                    var offsetX = adornerSize.Width * -GetDragMouseAnchorPoint(dragInfo.VisualSource).X;
                    var offsetY = adornerSize.Height * -GetDragMouseAnchorPoint(dragInfo.VisualSource).Y;
                    adornerPos.Offset(offsetX, offsetY);
                    var maxAdornerPosX = DragAdorner.AdornedElement.RenderSize.Width;
                    var adornerPosRightX = adornerPos.X + adornerSize.Width;
                    if (adornerPosRightX > maxAdornerPosX)
                    {
                        adornerPos.Offset(-adornerPosRightX + maxAdornerPosX, 0);
                    }
                }

                DragAdorner.MousePosition = adornerPos;
                DragAdorner.InvalidateVisual();
            }

            // If the target is an ItemsControl then update the drop target adorner.
            if (itemsControl != null)
            {
                // Display the adorner in the control's ItemsPresenter. If there is no 
                // ItemsPresenter provided by the style, try getting hold of a
                // ScrollContentPresenter and using that.
                var adornedElement =
                  itemsControl.GetVisualDescendent<ItemsPresenter>() ??
                  (UIElement)itemsControl.GetVisualDescendent<ScrollContentPresenter>();

                if (adornedElement != null)
                {
                    if (dropInfo.DropTargetAdorner == null)
                    {
                        DropTargetAdorner = null;
                    }
                    else if (!dropInfo.DropTargetAdorner.IsInstanceOfType(DropTargetAdorner))
                    {
                        DropTargetAdorner = DropTargetAdorner.Create(dropInfo.DropTargetAdorner, adornedElement);
                    }

                    if (DropTargetAdorner != null)
                    {
                        DropTargetAdorner.DropInfo = dropInfo;
                        DropTargetAdorner.InvalidateVisual();
                    }
                }
            }

            // Set the drag effect adorner if there is one
            if (EffectAdorner == null && dragInfo != null)
            {
                CreateEffectAdorner(dropInfo);
            }

            if (EffectAdorner != null)
            {
                var pos = e.GetPosition(EffectAdorner.AdornedElement);
                pos.Offset(20, 20);
                EffectAdorner.MousePosition = pos;
                EffectAdorner.InvalidateVisual();
            }

            e.Effects = dropInfo.Effects;
            e.Handled = !dropInfo.NotHandled;

            Scroll(dropInfo.VisualTarget, e);
        }

        private static void DropTargetPreviewDrop(object sender, DragEventArgs e)
        {
            var dropInfo = new DropInfo(sender, e, dragInfo);
            var dropHandler = GetDropHandler((UIElement)sender) ?? DefaultDropHandler;
            var dragHandler = TryGetDragHandler(dragInfo, sender as UIElement);

            DragAdorner = null;
            EffectAdorner = null;
            DropTargetAdorner = null;

            dropHandler.Drop(dropInfo);
            dragHandler.Dropped(dropInfo);

            Mouse.OverrideCursor = null;
            e.Handled = true;
        }

        private static void DropTargetGiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            if (EffectAdorner != null)
            {
                e.UseDefaultCursors = false;
                e.Handled = true;
            }
            else
            {
                e.UseDefaultCursors = true;
                e.Handled = true;
            }
        }
    }
}