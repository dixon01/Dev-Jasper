// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AnimateGridLengthAnimation.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The AnimateGridLengthAnimation.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Extensions
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media.Animation;

    /// <summary>
    /// The AnimateGridLengthAnimation.
    /// </summary>
    public static class AnimateGridLengthAnimation
    {
        /// <summary>
        /// Animate expand/collapse of a grid column. 
        /// </summary>
        /// <param name="gridColumn">The grid column to expand/collapse.</param>
        /// <param name="expand">If true, expand, otherwise collapse.</param>
        /// <param name="expandedWidth">The expanded width.</param>
        /// <param name="collapsedWidth">The width when collapsed.</param>
        /// <param name="minWidth">The minimum width of the column.</param>
        /// <param name="seconds">The seconds component of the duration.</param>
        /// <param name="milliseconds">The milliseconds component of the duration.</param>
        public static void AnimateGridColumnExpandCollapse(
            ColumnDefinition gridColumn, 
            bool expand, 
            double expandedWidth, 
            double collapsedWidth,
            double minWidth, 
            int seconds, 
            int milliseconds)
        {
            if (expand && gridColumn.ActualWidth >= expandedWidth)
            {
                // It's as wide as it needs to be.
                return;
            }

            if (!expand && Math.Abs(gridColumn.ActualWidth - collapsedWidth) < 0.0001)
            {
                // It's already collapsed.
                return;
            }

            var storyBoard = new Storyboard();

            var animation = new GridLengthAnimation
                            {
                                From = new GridLength(gridColumn.ActualWidth),
                                To = new GridLength(expand ? expandedWidth : collapsedWidth),
                                Duration = new TimeSpan(0, 0, 0, seconds, milliseconds)
                            };

            // Set delegate that will fire on completion.
            animation.Completed += delegate
            {
                // Set the animation to null on completion. This allows the grid to be resized manually
                gridColumn.BeginAnimation(ColumnDefinition.WidthProperty, null);

                // Set the final value manually.
                gridColumn.Width = new GridLength(expand ? expandedWidth : collapsedWidth);

                // Set the minimum width.
                gridColumn.MinWidth = minWidth;
            };

            storyBoard.Children.Add(animation);

            Storyboard.SetTarget(animation, gridColumn);
            Storyboard.SetTargetProperty(animation, new PropertyPath(ColumnDefinition.WidthProperty));
            storyBoard.Children.Add(animation);

            // Begin the animation.
            storyBoard.Begin();
        }

        /// <summary>
        /// Animate expand/collapse of a grid row. 
        /// </summary>
        /// <param name="gridRow">The grid row to expand/collapse.</param>
        /// <param name="expand">If true, expand, otherwise collapse.</param>
        /// <param name="expandedHeight">The expanded height.</param>
        /// <param name="collapsedHeight">The collapsed height.</param>
        /// <param name="minHeight">The minimum height.</param>
        /// <param name="duration">The time component of the duration.</param>
        public static void AnimateGridRowExpandCollapse(
            RowDefinition gridRow, 
            bool expand, 
            double expandedHeight, 
            double collapsedHeight,
            double minHeight, 
            TimeSpan duration)
        {
            if (expand && gridRow.ActualHeight >= expandedHeight)
            {
                // It's as high as it needs to be.
                return;
            }

            if (!expand && Math.Abs(gridRow.ActualHeight - collapsedHeight) < 0.0001)
            {
                // It's already collapsed.
                return;
            }

            var storyBoard = new Storyboard();

            var animation = new GridLengthAnimation
                            {
                                From = new GridLength(gridRow.ActualHeight),
                                To = new GridLength(expand ? expandedHeight : collapsedHeight),
                                Duration = duration
                            };

            // Set delegate that will fire on completioon.
            animation.Completed += delegate
            {
                // Set the animation to null on completion. This allows the grid to be resized manually
                gridRow.BeginAnimation(RowDefinition.HeightProperty, null);

                // Set the final height.
                gridRow.Height = new GridLength(expand ? expandedHeight : collapsedHeight);

                // Set the minimum height.
                gridRow.MinHeight = minHeight;
            };

            storyBoard.Children.Add(animation);

            Storyboard.SetTarget(animation, gridRow);
            Storyboard.SetTargetProperty(animation, new PropertyPath(RowDefinition.HeightProperty));
            storyBoard.Children.Add(animation);

            // Begin the animation.
            storyBoard.Begin();
        }
    }
}