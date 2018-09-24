// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AnimatedScrollViewer.cs" company="">
//   
// </copyright>
// <summary>
//   The animated scroll viewer.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Luminator.UIFramework.ResourceLibrary
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;
    using System.Windows.Media.Animation;

    /// <summary>
    ///     The animated scroll viewer.
    /// </summary>
    [TemplatePart(Name = "PART_AniVerticalScrollBar", Type = typeof (ScrollBar))]
    [TemplatePart(Name = "PART_AniHorizontalScrollBar", Type = typeof (ScrollBar))]
    public class AnimatedScrollViewer : ScrollViewer
    {
        #region Static Fields

        /// <summary>
        ///     The can keyboard scroll property.
        /// </summary>
        public static readonly DependencyProperty CanKeyboardScrollProperty =
            DependencyProperty.Register(
                "CanKeyboardScroll",
                typeof (bool),
                typeof (AnimatedScrollViewer),
                new FrameworkPropertyMetadata(true));

        /// <summary>
        ///     The horizontal scroll offset property.
        /// </summary>
        public static readonly DependencyProperty HorizontalScrollOffsetProperty =
            DependencyProperty.Register(
                "HorizontalScrollOffset",
                typeof (double),
                typeof (AnimatedScrollViewer),
                new PropertyMetadata(0.0, OnHorizontalScrollOffsetChanged));

        /// <summary>
        ///     The scrolling spline property.
        /// </summary>
        public static readonly DependencyProperty ScrollingSplineProperty =
            DependencyProperty.Register(
                "ScrollingSpline",
                typeof (KeySpline),
                typeof (AnimatedScrollViewer),
                new PropertyMetadata(new KeySpline(0.024, 0.914, 0.717, 1)));

        /// <summary>
        ///     The scrolling time property.
        /// </summary>
        public static readonly DependencyProperty ScrollingTimeProperty = DependencyProperty.Register(
            "ScrollingTime",
            typeof (TimeSpan),
            typeof (AnimatedScrollViewer),
            new PropertyMetadata(new TimeSpan(0, 0, 0, 0, 500)));

        /// <summary>
        ///     The target horizontal offset property.
        /// </summary>
        public static readonly DependencyProperty TargetHorizontalOffsetProperty =
            DependencyProperty.Register(
                "TargetHorizontalOffset",
                typeof (double),
                typeof (AnimatedScrollViewer),
                new PropertyMetadata(0.0, OnTargetHorizontalOffsetChanged));

        /// <summary>
        ///     The target vertical offset property.
        /// </summary>
        public static readonly DependencyProperty TargetVerticalOffsetProperty =
            DependencyProperty.Register(
                "TargetVerticalOffset",
                typeof (double),
                typeof (AnimatedScrollViewer),
                new PropertyMetadata(0.0, OnTargetVerticalOffsetChanged));

        /// <summary>
        ///     The vertical scroll offset property.
        /// </summary>
        public static readonly DependencyProperty VerticalScrollOffsetProperty =
            DependencyProperty.Register(
                "VerticalScrollOffset",
                typeof (double),
                typeof (AnimatedScrollViewer),
                new PropertyMetadata(0.0, OnVerticalScrollOffsetChanged));

        #endregion

        #region Fields

        /// <summary>
        ///     The _ani horizontal scroll bar.
        /// </summary>
        private ScrollBar _aniHorizontalScrollBar;

        /// <summary>
        ///     The _ani vertical scroll bar.
        /// </summary>
        private ScrollBar _aniVerticalScrollBar;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes static members of the <see cref="AnimatedScrollViewer" /> class.
        /// </summary>
        static AnimatedScrollViewer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof (AnimatedScrollViewer), new FrameworkPropertyMetadata(typeof (AnimatedScrollViewer)));
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets a value indicating whether can keyboard scroll.
        /// </summary>
        public bool CanKeyboardScroll
        {
            get { return (bool) this.GetValue(CanKeyboardScrollProperty); }

            set { this.SetValue(CanKeyboardScrollProperty, value); }
        }

        /// <summary>
        ///     This is the actual horizontal offset property we're going use as an animation helper
        /// </summary>
        public double HorizontalScrollOffset
        {
            get { return (double) this.GetValue(HorizontalScrollOffsetProperty); }

            set { this.SetValue(HorizontalScrollOffsetProperty, value); }
        }

        /// <summary>
        ///     A property to allow users to describe a custom spline for the scrolling
        ///     animation.
        /// </summary>
        public KeySpline ScrollingSpline
        {
            get { return (KeySpline) this.GetValue(ScrollingSplineProperty); }

            set { this.SetValue(ScrollingSplineProperty, value); }
        }

        /// <summary>
        ///     A property for changing the time it takes to scroll to a new
        ///     position.
        /// </summary>
        public TimeSpan ScrollingTime
        {
            get { return (TimeSpan) this.GetValue(ScrollingTimeProperty); }

            set { this.SetValue(ScrollingTimeProperty, value); }
        }

        /// <summary>
        ///     This is the HorizontalOffset that we'll be animating to
        /// </summary>
        public double TargetHorizontalOffset
        {
            get { return (double) this.GetValue(TargetHorizontalOffsetProperty); }

            set { this.SetValue(TargetHorizontalOffsetProperty, value); }
        }

        /// <summary>
        ///     This is the VerticalOffset that we'd like to animate to
        /// </summary>
        public double TargetVerticalOffset
        {
            get { return (double) this.GetValue(TargetVerticalOffsetProperty); }

            set { this.SetValue(TargetVerticalOffsetProperty, value); }
        }

        /// <summary>
        ///     This is the actual VerticalOffset we're going to use as an animation helper
        /// </summary>
        public double VerticalScrollOffset
        {
            get { return (double) this.GetValue(VerticalScrollOffsetProperty); }

            set { this.SetValue(VerticalScrollOffsetProperty, value); }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The on apply template.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            var aniVScroll = this.GetTemplateChild("PART_AniVerticalScrollBar") as ScrollBar;
            if (aniVScroll != null)
            {
                this._aniVerticalScrollBar = aniVScroll;
            }

            this._aniVerticalScrollBar.ValueChanged +=
                this.VScrollBar_ValueChanged;

            var aniHScroll = this.GetTemplateChild("PART_AniHorizontalScrollBar") as ScrollBar;
            if (aniHScroll != null)
            {
                this._aniHorizontalScrollBar = aniHScroll;
            }

            this._aniHorizontalScrollBar.ValueChanged +=
                this.HScrollBar_ValueChanged;

            this.PreviewMouseWheel += this.CustomPreviewMouseWheel;
            this.PreviewKeyDown += this.AnimatedScrollViewer_PreviewKeyDown;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     The on horizontal scroll offset changed.
        /// </summary>
        /// <param name="d">
        ///     The d.
        /// </param>
        /// <param name="e">
        ///     The e.
        /// </param>
        private static void OnHorizontalScrollOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var thisSViewer = (AnimatedScrollViewer) d;
            thisSViewer.ScrollToHorizontalOffset((double) e.NewValue);
        }

        /// <summary>
        ///     The on target horizontal offset changed.
        /// </summary>
        /// <param name="d">
        ///     The d.
        /// </param>
        /// <param name="e">
        ///     The e.
        /// </param>
        private static void OnTargetHorizontalOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var thisScroller = (AnimatedScrollViewer) d;

            if ((double) e.NewValue != thisScroller._aniHorizontalScrollBar.Value)
            {
                thisScroller._aniHorizontalScrollBar.Value = (double) e.NewValue;
            }

            thisScroller.animateScroller(thisScroller);
        }

        /// <summary>
        ///     The on target vertical offset changed.
        /// </summary>
        /// <param name="d">
        ///     The d.
        /// </param>
        /// <param name="e">
        ///     The e.
        /// </param>
        private static void OnTargetVerticalOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var thisScroller = (AnimatedScrollViewer) d;

            if ((double) e.NewValue != thisScroller._aniVerticalScrollBar.Value)
            {
                thisScroller._aniVerticalScrollBar.Value = (double) e.NewValue;
            }

            thisScroller.animateScroller(thisScroller);
        }

        /// <summary>
        ///     The on vertical scroll offset changed.
        /// </summary>
        /// <param name="d">
        ///     The d.
        /// </param>
        /// <param name="e">
        ///     The e.
        /// </param>
        private static void OnVerticalScrollOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var thisSViewer = (AnimatedScrollViewer) d;
            thisSViewer.ScrollToVerticalOffset((double) e.NewValue);
        }

        /// <summary>
        ///     The animated scroll viewer_ preview key down.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="e">
        ///     The e.
        /// </param>
        private void AnimatedScrollViewer_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            var thisScroller = (AnimatedScrollViewer) sender;

            if (thisScroller.CanKeyboardScroll)
            {
                Key keyPressed = e.Key;
                double newVerticalPos = thisScroller.TargetVerticalOffset;
                double newHorizontalPos = thisScroller.TargetHorizontalOffset;
                bool isKeyHandled = false;

                // Vertical Key Strokes code
                if (keyPressed == Key.Down)
                {
                    newVerticalPos = this.NormalizeScrollPos(
                        thisScroller, newVerticalPos + 16.0, Orientation.Vertical);
                    isKeyHandled = true;
                }
                else if (keyPressed == Key.PageDown)
                {
                    newVerticalPos = this.NormalizeScrollPos(
                        thisScroller, newVerticalPos + thisScroller.ViewportHeight, Orientation.Vertical);
                    isKeyHandled = true;
                }
                else if (keyPressed == Key.Up)
                {
                    newVerticalPos = this.NormalizeScrollPos(
                        thisScroller, newVerticalPos - 16.0, Orientation.Vertical);
                    isKeyHandled = true;
                }
                else if (keyPressed == Key.PageUp)
                {
                    newVerticalPos = this.NormalizeScrollPos(
                        thisScroller, newVerticalPos - thisScroller.ViewportHeight, Orientation.Vertical);
                    isKeyHandled = true;
                }

                if (newVerticalPos != thisScroller.TargetVerticalOffset)
                {
                    thisScroller.TargetVerticalOffset = newVerticalPos;
                }

                // Horizontal Key Strokes Code
                if (keyPressed == Key.Right)
                {
                    newHorizontalPos = this.NormalizeScrollPos(
                        thisScroller, newHorizontalPos + 16, Orientation.Horizontal);
                    isKeyHandled = true;
                }
                else if (keyPressed == Key.Left)
                {
                    newHorizontalPos = this.NormalizeScrollPos(
                        thisScroller, newHorizontalPos - 16, Orientation.Horizontal);
                    isKeyHandled = true;
                }

                if (newHorizontalPos != thisScroller.TargetHorizontalOffset)
                {
                    thisScroller.TargetHorizontalOffset = newHorizontalPos;
                }

                e.Handled = isKeyHandled;
            }
        }

        /// <summary>
        ///     The custom preview mouse wheel.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="e">
        ///     The e.
        /// </param>
        private void CustomPreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            double mouseWheelChange = e.Delta;

            var thisScroller = (AnimatedScrollViewer) sender;
            double newVOffset = thisScroller.TargetVerticalOffset - (mouseWheelChange/3);
            if (newVOffset < 0)
            {
                thisScroller.TargetVerticalOffset = 0;
            }
            else if (newVOffset > thisScroller.ScrollableHeight)
            {
                thisScroller.TargetVerticalOffset = thisScroller.ScrollableHeight;
            }
            else
            {
                thisScroller.TargetVerticalOffset = newVOffset;
            }

            e.Handled = true;
        }

        /// <summary>
        ///     The h scroll bar_ value changed.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="e">
        ///     The e.
        /// </param>
        private void HScrollBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            AnimatedScrollViewer thisScroller = this;

            double oldTargetHOffset = e.OldValue;
            double newTargetHOffset = e.NewValue;

            if (newTargetHOffset != thisScroller.TargetHorizontalOffset)
            {
                double deltaVOffset = Math.Round(newTargetHOffset - oldTargetHOffset, 3);

                if (deltaVOffset == 1)
                {
                    thisScroller.TargetHorizontalOffset = oldTargetHOffset + thisScroller.ViewportWidth;
                }
                else if (deltaVOffset == -1)
                {
                    thisScroller.TargetHorizontalOffset = oldTargetHOffset - thisScroller.ViewportWidth;
                }
                else if (deltaVOffset == 0.1)
                {
                    thisScroller.TargetHorizontalOffset = oldTargetHOffset + 16.0;
                }
                else if (deltaVOffset == -0.1)
                {
                    thisScroller.TargetHorizontalOffset = oldTargetHOffset - 16.0;
                }
                else
                {
                    thisScroller.TargetHorizontalOffset = newTargetHOffset;
                }
            }
        }

        /// <summary>
        ///     The normalize scroll pos.
        /// </summary>
        /// <param name="thisScroll">
        ///     The this scroll.
        /// </param>
        /// <param name="scrollChange">
        ///     The scroll change.
        /// </param>
        /// <param name="o">
        ///     The o.
        /// </param>
        /// <returns>
        ///     The <see cref="double" />.
        /// </returns>
        private double NormalizeScrollPos(AnimatedScrollViewer thisScroll, double scrollChange, Orientation o)
        {
            double returnValue = scrollChange;

            if (scrollChange < 0)
            {
                returnValue = 0;
            }

            if (o == Orientation.Vertical && scrollChange > thisScroll.ScrollableHeight)
            {
                returnValue = thisScroll.ScrollableHeight;
            }
            else if (o == Orientation.Horizontal && scrollChange > thisScroll.ScrollableWidth)
            {
                returnValue = thisScroll.ScrollableWidth;
            }

            return returnValue;
        }

        /// <summary>
        ///     The v scroll bar_ value changed.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="e">
        ///     The e.
        /// </param>
        private void VScrollBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            AnimatedScrollViewer thisScroller = this;
            double oldTargetVOffset = e.OldValue;
            double newTargetVOffset = e.NewValue;

            if (newTargetVOffset != thisScroller.TargetVerticalOffset)
            {
                double deltaVOffset = Math.Round(newTargetVOffset - oldTargetVOffset, 3);

                if (deltaVOffset == 1)
                {
                    thisScroller.TargetVerticalOffset = oldTargetVOffset + thisScroller.ViewportHeight;
                }
                else if (deltaVOffset == -1)
                {
                    thisScroller.TargetVerticalOffset = oldTargetVOffset - thisScroller.ViewportHeight;
                }
                else if (deltaVOffset == 0.1)
                {
                    thisScroller.TargetVerticalOffset = oldTargetVOffset + 16.0;
                }
                else if (deltaVOffset == -0.1)
                {
                    thisScroller.TargetVerticalOffset = oldTargetVOffset - 16.0;
                }
                else
                {
                    thisScroller.TargetVerticalOffset = newTargetVOffset;
                }
            }
        }

        /// <summary>
        ///     The animate scroller.
        /// </summary>
        /// <param name="objectToScroll">
        ///     The object to scroll.
        /// </param>
        private void animateScroller(object objectToScroll)
        {
            var thisScrollViewer = objectToScroll as AnimatedScrollViewer;

            var targetTime = new Duration(thisScrollViewer.ScrollingTime);
            KeyTime targetKeyTime = thisScrollViewer.ScrollingTime;
            KeySpline targetKeySpline = thisScrollViewer.ScrollingSpline;

            var animateHScrollKeyFramed = new DoubleAnimationUsingKeyFrames();
            var animateVScrollKeyFramed = new DoubleAnimationUsingKeyFrames();

            var HScrollKey1 = new SplineDoubleKeyFrame(
                thisScrollViewer.TargetHorizontalOffset, targetKeyTime, targetKeySpline);
            var VScrollKey1 = new SplineDoubleKeyFrame(
                thisScrollViewer.TargetVerticalOffset, targetKeyTime, targetKeySpline);
            animateHScrollKeyFramed.KeyFrames.Add(HScrollKey1);
            animateVScrollKeyFramed.KeyFrames.Add(VScrollKey1);

            thisScrollViewer.BeginAnimation(HorizontalScrollOffsetProperty, animateHScrollKeyFramed);
            thisScrollViewer.BeginAnimation(VerticalScrollOffsetProperty, animateVScrollKeyFramed);

            CommandBindingCollection testCollection = thisScrollViewer.CommandBindings;
            int blah = testCollection.Count;
        }

        #endregion
    }
}