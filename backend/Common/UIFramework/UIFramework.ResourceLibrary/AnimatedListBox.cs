// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AnimatedListBox.cs" company="">
//   
// </copyright>
// <summary>
//   The animated list box.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Luminator.UIFramework.ResourceLibrary
{
    using System;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    ///     The animated list box.
    /// </summary>
    [TemplatePart(Name = "PART_AnimatedScrollViewer", Type = typeof (AnimatedScrollViewer))]
    public class AnimatedListBox : ListBox
    {
        #region Static Fields

        /// <summary>
        ///     The scroll to selected item property.
        /// </summary>
        public static readonly DependencyProperty ScrollToSelectedItemProperty =
            DependencyProperty.Register(
                "ScrollToSelectedItem", typeof (bool), typeof (AnimatedListBox), new PropertyMetadata(false));

        /// <summary>
        ///     The selected index offset property.
        /// </summary>
        public static readonly DependencyProperty SelectedIndexOffsetProperty =
            DependencyProperty.Register(
                "SelectedIndexOffset", typeof (int), typeof (AnimatedListBox), new PropertyMetadata(0));

        #endregion

        #region Fields

        /// <summary>
        ///     The _scroll viewer.
        /// </summary>
        private AnimatedScrollViewer _scrollViewer;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes static members of the <see cref="AnimatedListBox" /> class.
        /// </summary>
        static AnimatedListBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof (AnimatedListBox), new FrameworkPropertyMetadata(typeof (AnimatedListBox)));
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     A description of the property.
        /// </summary>
        public bool ScrollToSelectedItem
        {
            get { return (bool) this.GetValue(ScrollToSelectedItemProperty); }

            set { this.SetValue(ScrollToSelectedItemProperty, value); }
        }

        /// <summary>
        ///     Use this property to choose the scroll to an item that is not selected, but is X above or below the selected item
        /// </summary>
        public int SelectedIndexOffset
        {
            get { return (int) this.GetValue(SelectedIndexOffsetProperty); }

            set { this.SetValue(SelectedIndexOffsetProperty, value); }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The on apply template.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            var scrollViewerHolder =
                this.GetTemplateChild("PART_AnimatedScrollViewer") as AnimatedScrollViewer;
            if (scrollViewerHolder != null)
            {
                this._scrollViewer = scrollViewerHolder;
            }

            this.SelectionChanged += this.AnimatedListBox_SelectionChanged;
            this.Loaded += this.AnimatedListBox_Loaded;
            this.LayoutUpdated += this.AnimatedListBox_LayoutUpdated;
        }

        /// <summary>
        ///     The update scroll position.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        public void updateScrollPosition(object sender)
        {
            var thisLB = (AnimatedListBox) sender;

            if (thisLB != null)
            {
                if (thisLB.ScrollToSelectedItem)
                {
                    double scrollTo = 0;
                    for (int i = 0; i < (thisLB.SelectedIndex + thisLB.SelectedIndexOffset); i++)
                    {
                        var tempItem =
                            thisLB.ItemContainerGenerator.ContainerFromItem(thisLB.Items[i]) as ListBoxItem;

                        if (tempItem != null)
                        {
                            scrollTo += tempItem.ActualHeight;
                        }
                    }

                    this._scrollViewer.TargetVerticalOffset = scrollTo;
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        ///     The animated list box_ layout updated.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="e">
        ///     The e.
        /// </param>
        private void AnimatedListBox_LayoutUpdated(object sender, EventArgs e)
        {
            this.updateScrollPosition(sender);
        }

        /// <summary>
        ///     The animated list box_ loaded.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="e">
        ///     The e.
        /// </param>
        private void AnimatedListBox_Loaded(object sender, RoutedEventArgs e)
        {
            this.updateScrollPosition(sender);
        }

        /// <summary>
        ///     The animated list box_ selection changed.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="e">
        ///     The e.
        /// </param>
        private void AnimatedListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.updateScrollPosition(sender);
        }

        #endregion
    }
}