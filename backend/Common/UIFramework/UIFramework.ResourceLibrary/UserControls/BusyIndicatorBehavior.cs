// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BusyIndicatorBehavior.cs" company="Luminator USA">
//   Copyright (c) 2013
//   All Rights Reserved
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Luminator.UIFramework.ResourceLibrary.UserControls
{
    using System;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;

    /// <summary>
    ///     The busy indicator behavior.
    /// </summary>
    public static class BusyIndicatorBehavior
    {
        #region Static Fields

        /// <summary>
        ///     The add margins property.
        /// </summary>
        public static readonly DependencyProperty AddMarginsProperty = DependencyProperty.RegisterAttached(
            "AddMargins", typeof(bool), typeof(BusyIndicatorBehavior), new UIPropertyMetadata(false));

        /// <summary>
        ///     The busy state property.
        /// </summary>
        public static readonly DependencyProperty BusyStateProperty = DependencyProperty.RegisterAttached(
            "BusyState", typeof(bool), typeof(BusyIndicatorBehavior), new UIPropertyMetadata(false, OnBusyStateChanged));

        /// <summary>
        ///     The dim background property.
        /// </summary>
        public static readonly DependencyProperty DimBackgroundProperty = DependencyProperty.RegisterAttached(
            "DimBackground", typeof(bool), typeof(BusyIndicatorBehavior), new UIPropertyMetadata(true, OnDimBackgroundChanged));

        /// <summary>
        ///     The dim transition duration property.
        /// </summary>
        public static readonly DependencyProperty DimTransitionDurationProperty = DependencyProperty.RegisterAttached(
            "DimTransitionDuration", typeof(Duration), typeof(BusyIndicatorBehavior), new UIPropertyMetadata(new Duration(TimeSpan.FromSeconds(1.0))));

        /// <summary>
        ///     The dimmer brush property.
        /// </summary>
        public static readonly DependencyProperty DimmerBrushProperty = DependencyProperty.RegisterAttached(
            "DimmerBrush", typeof(Brush), typeof(BusyIndicatorBehavior), new UIPropertyMetadata(Brushes.Black));

        /// <summary>
        ///     The dimmer opacity property.
        /// </summary>
        public static readonly DependencyProperty DimmerOpacityProperty = DependencyProperty.RegisterAttached(
            "DimmerOpacity", typeof(double), typeof(BusyIndicatorBehavior), new UIPropertyMetadata(0.5));

        /// <summary>
        ///     The target visual property.
        /// </summary>
        public static readonly DependencyProperty TargetVisualProperty = DependencyProperty.RegisterAttached(
            "TargetVisual", typeof(UIElement), typeof(BusyIndicatorBehavior), new UIPropertyMetadata(null));

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The get add margins.
        /// </summary>
        /// <param name="dependencyObject">
        /// The dependency object.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool GetAddMargins(DependencyObject dependencyObject)
        {
            return (bool)dependencyObject.GetValue(AddMarginsProperty);
        }

        /// <summary>
        /// The get busy state.
        /// </summary>
        /// <param name="dependencyObject">
        /// The dependency object.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool GetBusyState(DependencyObject dependencyObject)
        {
            return (bool)dependencyObject.GetValue(BusyStateProperty);
        }

        /// <summary>
        /// The get dim background.
        /// </summary>
        /// <param name="dependencyObject">
        /// The dependency object.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool GetDimBackground(DependencyObject dependencyObject)
        {
            return (bool)dependencyObject.GetValue(DimBackgroundProperty);
        }

        /// <summary>
        /// The get dim transition duration.
        /// </summary>
        /// <param name="dependencyObject">
        /// The dependency object.
        /// </param>
        /// <returns>
        /// The <see cref="Duration"/>.
        /// </returns>
        public static Duration GetDimTransitionDuration(DependencyObject dependencyObject)
        {
            return (Duration)dependencyObject.GetValue(DimTransitionDurationProperty);
        }

        /// <summary>
        /// The get dimmer brush.
        /// </summary>
        /// <param name="dependencyObject">
        /// The dependency object.
        /// </param>
        /// <returns>
        /// The <see cref="Brush"/>.
        /// </returns>
        public static Brush GetDimmerBrush(DependencyObject dependencyObject)
        {
            return (Brush)dependencyObject.GetValue(DimmerBrushProperty);
        }

        /// <summary>
        /// The get dimmer opacity.
        /// </summary>
        /// <param name="dependencyObject">
        /// The dependency object.
        /// </param>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        public static double GetDimmerOpacity(DependencyObject dependencyObject)
        {
            return (double)dependencyObject.GetValue(DimmerOpacityProperty);
        }

        /// <summary>
        /// The get target visual.
        /// </summary>
        /// <param name="dependencyObject">
        /// The dependency object.
        /// </param>
        /// <returns>
        /// The <see cref="UIElement"/>.
        /// </returns>
        public static UIElement GetTargetVisual(DependencyObject dependencyObject)
        {
            return (UIElement)dependencyObject.GetValue(TargetVisualProperty);
        }

        /// <summary>
        /// The set add margins.
        /// </summary>
        /// <param name="dependencyObject">
        /// The dependency object.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        public static void SetAddMargins(DependencyObject dependencyObject, bool value)
        {
            dependencyObject.SetValue(AddMarginsProperty, value);
        }

        /// <summary>
        /// The set busy state.
        /// </summary>
        /// <param name="dependencyObject">
        /// The dependency object.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        public static void SetBusyState(DependencyObject dependencyObject, bool value)
        {
            dependencyObject.SetValue(BusyStateProperty, value);
        }

        /// <summary>
        /// The set dim background.
        /// </summary>
        /// <param name="dependencyObject">
        /// The dependency object.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        public static void SetDimBackground(DependencyObject dependencyObject, bool value)
        {
            dependencyObject.SetValue(DimBackgroundProperty, value);
        }

        /// <summary>
        /// The set dim transition duration.
        /// </summary>
        /// <param name="dependencyObject">
        /// The dependency object.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        public static void SetDimTransitionDuration(DependencyObject dependencyObject, Duration value)
        {
            dependencyObject.SetValue(DimTransitionDurationProperty, value);
        }

        /// <summary>
        /// The set dimmer brush.
        /// </summary>
        /// <param name="dependencyObject">
        /// The dependency object.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        public static void SetDimmerBrush(DependencyObject dependencyObject, Brush value)
        {
            dependencyObject.SetValue(DimmerBrushProperty, value);
        }

        /// <summary>
        /// The set dimmer opacity.
        /// </summary>
        /// <param name="dependencyObject">
        /// The dependency Object.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        public static void SetDimmerOpacity(DependencyObject dependencyObject, double value)
        {
            dependencyObject.SetValue(DimmerOpacityProperty, value);
        }

        /// <summary>
        /// The set target visual.
        /// </summary>
        /// <param name="dependencyObject">
        /// The dependency Object.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        public static void SetTargetVisual(DependencyObject dependencyObject, UIElement value)
        {
            dependencyObject.SetValue(TargetVisualProperty, value);
        }

        #endregion

        #region Methods

        /// <summary>
        /// The on busy state changed.
        /// </summary>
        /// <param name="dependencyObject">
        /// The dependency object.
        /// </param>
        /// <param name="dependencyPropertyChangedEventArgs">
        /// The dependency property changed event args.
        /// </param>
        private static void OnBusyStateChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var isBusy = (bool)dependencyPropertyChangedEventArgs.NewValue;
            var wasBusy = (bool)dependencyPropertyChangedEventArgs.OldValue;

            if (isBusy == wasBusy)
            {
                return;
            }

            DependencyObject hostGridObject = GetTargetVisual(dependencyObject) ?? dependencyObject;
            Debug.Assert(hostGridObject != null, "hostGridObject cannot be null");

            var hostGrid = hostGridObject as Grid;
            if (hostGrid == null)
            {
                throw new InvalidCastException(
                    string.Format(
                        "The object being attached to must be of type {0}. Try embedding your visual inside a {0} control, and attaching the behavior to the {0} instead.", 
                        typeof(Grid).Name));
            }

            if (isBusy)
            {
                Debug.Assert(LogicalTreeHelper.FindLogicalNode(hostGrid, "BusyIndicator") == null, "hostGrid should be null");

                bool dimBackground = GetDimBackground(dependencyObject);
                var grid = new Grid { Name = "BusyIndicator", Opacity = 0.0 };
                if (dimBackground)
                {
                    grid.Cursor = Cursors.Wait;
                    grid.ForceCursor = true;

                    InputManager.Current.PreProcessInput += OnPreProcessInput;
                }

                grid.SetBinding(FrameworkElement.WidthProperty, new Binding("ActualWidth") { Source = hostGrid });
                grid.SetBinding(FrameworkElement.HeightProperty, new Binding("ActualHeight") { Source = hostGrid });
                for (int i = 1; i <= 3; ++i)
                {
                    grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                    grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                }

                var viewbox = new Viewbox
                                  {
                                      HorizontalAlignment = HorizontalAlignment.Center, 
                                      Stretch = Stretch.Uniform, 
                                      StretchDirection = StretchDirection.Both, 
                                      Child = new CircularProgressBar()
                                  };
                grid.SetValue(Panel.ZIndexProperty, 1000);
                grid.SetValue(Grid.RowSpanProperty, Math.Max(1, hostGrid.RowDefinitions.Count));
                grid.SetValue(Grid.ColumnSpanProperty, Math.Max(1, hostGrid.ColumnDefinitions.Count));
                if (GetAddMargins(dependencyObject))
                {
                    viewbox.SetValue(Grid.RowProperty, 1);
                    viewbox.SetValue(Grid.ColumnProperty, 1);
                }
                else
                {
                    viewbox.SetValue(Grid.RowSpanProperty, 3);
                    viewbox.SetValue(Grid.ColumnSpanProperty, 3);
                }

                viewbox.SetValue(Panel.ZIndexProperty, 1);

                var dimmer = new Rectangle
                                 {
                                     Name = "Dimmer", 
                                     Opacity = GetDimmerOpacity(dependencyObject), 
                                     Fill = GetDimmerBrush(dependencyObject), 
                                     Visibility = dimBackground ? Visibility.Visible : Visibility.Collapsed
                                 };
                dimmer.SetValue(Grid.RowSpanProperty, 3);
                dimmer.SetValue(Grid.ColumnSpanProperty, 3);
                dimmer.SetValue(Panel.ZIndexProperty, 0);
                grid.Children.Add(dimmer);

                grid.Children.Add(viewbox);

                grid.BeginAnimation(UIElement.OpacityProperty, new DoubleAnimation(1.0, GetDimTransitionDuration(dependencyObject)));

                hostGrid.Children.Add(grid);
            }
            else
            {
                var grid = (Grid)LogicalTreeHelper.FindLogicalNode(hostGrid, "BusyIndicator");

                Debug.Assert(grid != null, "grid cannot be null");

                grid.Name = string.Empty;

                var fadeOutAnimation = new DoubleAnimation(0.0, GetDimTransitionDuration(dependencyObject));
                fadeOutAnimation.Completed += (sender, args) => OnFadeOutAnimationCompleted(dependencyObject, hostGrid, grid);
                grid.BeginAnimation(UIElement.OpacityProperty, fadeOutAnimation);
            }
        }

        /// <summary>
        /// The on dim background changed.
        /// </summary>
        /// <param name="d">
        /// The dependencyObject.
        /// </param>
        /// <param name="e">
        /// The dependencyPropertyChangedEventArgs.
        /// </param>
        private static void OnDimBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var shouldDimBackground = (bool)e.NewValue;
            var wasDimmingBackground = (bool)e.OldValue;

            if (shouldDimBackground == wasDimmingBackground)
            {
                return;
            }

            if (!GetBusyState(d))
            {
                return;
            }

            DependencyObject hostGridObject = GetTargetVisual(d) ?? d;
            Debug.Assert(hostGridObject != null, "hostGridObject cannot be null");

            var hostGrid = hostGridObject as Grid;
            if (hostGrid != null)
            {
                var grid = (Grid)LogicalTreeHelper.FindLogicalNode(hostGrid, "BusyIndicator");

                if (grid != null)
                {
                    var dimmer = (Rectangle)LogicalTreeHelper.FindLogicalNode(grid, "Dimmer");

                    if (dimmer != null)
                    {
                        dimmer.Visibility = shouldDimBackground ? Visibility.Visible : Visibility.Collapsed;
                    }

                    if (shouldDimBackground)
                    {
                        grid.Cursor = Cursors.Wait;
                        grid.ForceCursor = true;

                        InputManager.Current.PreProcessInput += OnPreProcessInput;
                    }
                    else
                    {
                        grid.Cursor = Cursors.Arrow;
                        grid.ForceCursor = false;

                        InputManager.Current.PreProcessInput -= OnPreProcessInput;
                    }
                }
            }
        }

        /// <summary>
        /// The on fade out animation completed.
        /// </summary>
        /// <param name="d">
        /// The dependencyObject.
        /// </param>
        /// <param name="hostGrid">
        /// The host grid.
        /// </param>
        /// <param name="busyIndicator">
        /// The busy indicator.
        /// </param>
        private static void OnFadeOutAnimationCompleted(DependencyObject d, Panel hostGrid, UIElement busyIndicator)
        {
            bool dimBackground = GetDimBackground(d);

            hostGrid.Children.Remove(busyIndicator);

            if (dimBackground)
            {
                InputManager.Current.PreProcessInput -= OnPreProcessInput;
            }
        }

        /// <summary>
        /// The on pre process input.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The dependencyPropertyChangedEventArgs.
        /// </param>
        private static void OnPreProcessInput(object sender, PreProcessInputEventArgs e)
        {
            if (e.StagingItem.Input.Device != null)
            {
                e.Cancel();
            }
        }

        #endregion
    }
}