// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RadGridViewExtenders.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the RadGridViewExtenders type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Diag.Core.Extensions
{
    using System.Collections.Specialized;
    using System.Windows;

    using Telerik.Windows.Controls;

    /// <summary>
    /// Extensions for <see cref="RadGridView"/>.
    /// </summary>
    public class RadGridViewExtenders : DependencyObject
    {
        /// <summary>
        /// Property that allows to enable or disable auto-scroll to the end of a <see cref="RadGridView"/>
        /// when new items are added.
        /// </summary>
        public static readonly DependencyProperty AutoScrollToEndProperty =
            DependencyProperty.RegisterAttached(
                "AutoScrollToEnd",
                typeof(bool),
                typeof(RadGridViewExtenders),
                new UIPropertyMetadata(default(bool), OnAutoScrollToEndChanged));

        private static readonly DependencyProperty AutoScrollToEndHandlerProperty =
            DependencyProperty.RegisterAttached(
                "AutoScrollToEndHandler",
                typeof(NotifyCollectionChangedEventHandler),
                typeof(RadGridViewExtenders));

        /// <summary>
        /// Returns the value of the AutoScrollToEndProperty
        /// </summary>
        /// <param name="obj">The dependency-object of which value should be returned</param>
        /// <returns>The value of the given property</returns>
        public static bool GetAutoScrollToEnd(DependencyObject obj)
        {
            return (bool)obj.GetValue(AutoScrollToEndProperty);
        }

        /// <summary>
        /// Sets the value of the AutoScrollToEndProperty
        /// </summary>
        /// <param name="obj">The dependency-object of which value should be set</param>
        /// <param name="value">The value which should be assigned to the AutoScrollToEndProperty</param>
        public static void SetAutoScrollToEnd(DependencyObject obj, bool value)
        {
            obj.SetValue(AutoScrollToEndProperty, value);
        }

        /// <summary>
        /// This method will be called when the AutoScrollToEnd
        /// property was changed
        /// </summary>
        /// <param name="source">The sender (the ListBox)</param>
        /// <param name="eventArgs">Some additional information</param>
        public static void OnAutoScrollToEndChanged(
            DependencyObject source, DependencyPropertyChangedEventArgs eventArgs)
        {
            var gridView = source as RadGridView;
            if (gridView == null)
            {
                return;
            }

            var items = gridView.Items;
            var data = items.SourceCollection as INotifyCollectionChanged;
            if (data == null)
            {
                return;
            }

            NotifyCollectionChangedEventHandler scrollToEndHandler;
            if (!(bool)eventArgs.NewValue)
            {
                scrollToEndHandler =
                    source.GetValue(AutoScrollToEndHandlerProperty) as NotifyCollectionChangedEventHandler;
                if (scrollToEndHandler != null)
                {
                    data.CollectionChanged -= scrollToEndHandler;
                    source.SetValue(AutoScrollToEndHandlerProperty, null);
                }

                return;
            }

            scrollToEndHandler = (s, e) =>
                {
                    if (items.Count == 0)
                    {
                        return;
                    }

                    var lastItem = items[items.Count - 1];
                    gridView.ScrollIntoViewAsync(lastItem, null);
                };

            // save the event handler so we can use it for un-registration afterwards
            source.SetValue(AutoScrollToEndHandlerProperty, scrollToEndHandler);
            data.CollectionChanged += scrollToEndHandler;
        }
    }
}
