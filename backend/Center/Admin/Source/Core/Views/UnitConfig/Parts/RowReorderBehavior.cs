// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RowReorderBehavior.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The row reorder behavior for grid view.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Views.UnitConfig.Parts
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Shapes;

    using Telerik.Windows.Controls;
    using Telerik.Windows.Controls.GridView;
    using Telerik.Windows.DragDrop;
    using Telerik.Windows.DragDrop.Behaviors;

    /// <summary>
    /// The row reorder behavior for a <see cref="RadGridView"/>.
    /// </summary>
    public class RowReorderBehavior
    {
        /// <summary>
        /// Using a DependencyProperty as the backing store for IsEnabled.  This enables animation, styling, binding,...
        /// </summary>
        public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached(
            "IsEnabled",
            typeof(bool),
            typeof(RowReorderBehavior),
            new PropertyMetadata(new PropertyChangedCallback(OnIsEnabledPropertyChanged)));

        private const string DropPositionFeedbackElementName = "DragBetweenItemsFeedback";
        private static Dictionary<RadGridView, RowReorderBehavior> instances;
        private ContentPresenter dropPositionFeedbackPresenter;
        private Grid dropPositionFeedbackPresenterHost;

        private RadGridView associatedObject;

        static RowReorderBehavior()
        {
            instances = new Dictionary<RadGridView, RowReorderBehavior>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RowReorderBehavior"/> class.
        /// </summary>
        public RowReorderBehavior()
        {
        }

        /// <summary>
        /// Gets or sets the associated object.
        /// </summary>
        public RadGridView AssociatedObject
        {
            get
            {
                return this.associatedObject;
            }

            set
            {
                this.associatedObject = value;
            }
        }

        /// <summary>
        /// Gets the IsEnabled value.
        /// </summary>
        /// <param name="obj">
        /// The dependency object.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool GetIsEnabled(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsEnabledProperty);
        }

        /// <summary>
        /// The set is enabled.
        /// </summary>
        /// <param name="obj">
        /// The dependency object.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        public static void SetIsEnabled(DependencyObject obj, bool value)
        {
            RowReorderBehavior behavior = GetAttachedBehavior(obj as RadGridView);

            behavior.AssociatedObject = obj as RadGridView;

            if (value)
            {
                behavior.Initialize();
            }
            else
            {
                behavior.CleanUp();
            }

            obj.SetValue(IsEnabledProperty, value);
        }

        /// <summary>
        /// The on is enabled property changed.
        /// </summary>
        /// <param name="dependencyObject">
        /// The dependency object.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        public static void OnIsEnabledPropertyChanged(
            DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs e)
        {
            SetIsEnabled(dependencyObject, (bool)e.NewValue);
        }

        /// <summary>
        /// The get drop position from point.
        /// </summary>
        /// <param name="absoluteMousePosition">
        /// The absolute mouse position.
        /// </param>
        /// <param name="row">
        /// The row.
        /// </param>
        /// <returns>
        /// The <see cref="DropPosition"/>.
        /// </returns>
        public virtual DropPosition GetDropPositionFromPoint(Point absoluteMousePosition, GridViewRow row)
        {
            if (row != null)
            {
                return absoluteMousePosition.Y < row.ActualHeight / 2 ? DropPosition.Before : DropPosition.After;
            }

            return DropPosition.Inside;
        }

        /// <summary>
        /// The initialize.
        /// </summary>
        protected virtual void Initialize()
        {
            this.AssociatedObject.RowLoaded -= this.AssociatedObjectRowLoaded;
            this.AssociatedObject.RowLoaded += this.AssociatedObjectRowLoaded;
            this.UnsubscribeFromDragDropEvents();
            this.SubscribeToDragDropEvents();

            this.AssociatedObject.Dispatcher.BeginInvoke((Action)(() =>
            {
                this.dropPositionFeedbackPresenter = new ContentPresenter
                                                         {
                                                             Name = DropPositionFeedbackElementName,
                                                             HorizontalAlignment =
                                                                 HorizontalAlignment.Left,
                                                             VerticalAlignment = VerticalAlignment.Top,
                                                             RenderTransformOrigin = new Point(0.5, 0.5)
                                                         };

                this.AttachDropPositionFeedback();
            }));
        }

        /// <summary>
        /// Cleans up the attached events
        /// </summary>
        protected virtual void CleanUp()
        {
            this.AssociatedObject.RowLoaded -= this.AssociatedObjectRowLoaded;
            this.UnsubscribeFromDragDropEvents();

            this.DetachDropPositionFeedback();
        }

        private static RowReorderBehavior GetAttachedBehavior(RadGridView gridview)
        {
            if (!instances.ContainsKey(gridview))
            {
                instances[gridview] = new RowReorderBehavior { AssociatedObject = gridview };
            }

            return instances[gridview];
        }

        private static UIElement CreateDefaultDropPositionFeedback()
        {
            var grid = new Grid
                           {
                               Height = 8,
                               HorizontalAlignment = HorizontalAlignment.Stretch,
                               IsHitTestVisible = false,
                               VerticalAlignment = VerticalAlignment.Stretch
                           };
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(8) });
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            var ellipse = new Ellipse
                              {
                                  Stroke = new SolidColorBrush(Colors.Orange),
                                  StrokeThickness = 2,
                                  Fill = new SolidColorBrush(Colors.Orange),
                                  HorizontalAlignment = HorizontalAlignment.Stretch,
                                  VerticalAlignment = VerticalAlignment.Stretch,
                                  Width = 8,
                                  Height = 8
                              };
            var rectangle = new Rectangle
                                {
                                    Fill = new SolidColorBrush(Colors.Orange),
                                    RadiusX = 2,
                                    RadiusY = 2,
                                    VerticalAlignment = VerticalAlignment.Stretch,
                                    HorizontalAlignment = HorizontalAlignment.Stretch,
                                    Height = 2
                                };
            Grid.SetColumn(ellipse, 0);
            Grid.SetColumn(rectangle, 1);
            grid.Children.Add(ellipse);
            grid.Children.Add(rectangle);

            Canvas.SetZIndex(grid, 10000);

            return grid;
        }

        private void AssociatedObjectRowLoaded(object sender, RowLoadedEventArgs e)
        {
            if (e.Row is GridViewHeaderRow || e.Row is GridViewNewRow || e.Row is GridViewFooterRow)
            {
                return;
            }

            var row = e.Row as GridViewRow;
            this.InitializeRowDragAndDrop(row);
        }

        private void InitializeRowDragAndDrop(GridViewRow row)
        {
            if (row == null)
            {
                return;
            }

            DragDropManager.RemoveDragOverHandler(row, this.OnRowDragOver);
            DragDropManager.AddDragOverHandler(row, this.OnRowDragOver);
        }

        private void SubscribeToDragDropEvents()
        {
            DragDropManager.AddDragInitializeHandler(this.AssociatedObject, this.OnDragInitialize);
            DragDropManager.AddGiveFeedbackHandler(this.AssociatedObject, this.OnGiveFeedback);
            DragDropManager.AddDropHandler(this.AssociatedObject, this.OnDrop);
            DragDropManager.AddDragDropCompletedHandler(this.AssociatedObject, this.OnDragDropCompleted);
        }

        private void UnsubscribeFromDragDropEvents()
        {
            DragDropManager.RemoveDragInitializeHandler(this.AssociatedObject, this.OnDragInitialize);
            DragDropManager.RemoveGiveFeedbackHandler(this.AssociatedObject, this.OnGiveFeedback);
            DragDropManager.RemoveDropHandler(this.AssociatedObject, this.OnDrop);
            DragDropManager.RemoveDragDropCompletedHandler(this.AssociatedObject, this.OnDragDropCompleted);
        }

        private void OnDragDropCompleted(object sender, DragDropCompletedEventArgs e)
        {
            this.HideDropPositionFeedbackPresenter();
        }

        private void OnDragInitialize(object sender, DragInitializeEventArgs e)
        {
            var sourceRow = e.OriginalSource as GridViewRow
                            ?? (e.OriginalSource as FrameworkElement).ParentOfType<GridViewRow>();
            if (sourceRow != null && sourceRow.Name != "PART_RowResizer")
            {
                var details = new DropIndicationDetails();
                var item = sourceRow.Item;
                details.CurrentDraggedItem = item;

                IDragPayload dragPayload = DragDropPayloadManager.GeneratePayload(null);

                dragPayload.SetData("DraggedItem", item);
                dragPayload.SetData("DropDetails", details);

                e.Data = dragPayload;

                e.DragVisual = new DragVisual
                                   {
                                       Content = details,
                                       ContentTemplate =
                                           this.AssociatedObject.Resources["DraggedItemTemplate"] as
                                           DataTemplate
                                   };
                e.DragVisualOffset = e.RelativeStartPoint;
                e.AllowedEffects = DragDropEffects.All;
            }
        }

        private void OnGiveFeedback(object sender, Telerik.Windows.DragDrop.GiveFeedbackEventArgs e)
        {
            e.SetCursor(Cursors.Arrow);
            e.Handled = true;
        }

        private void OnDrop(object sender, Telerik.Windows.DragDrop.DragEventArgs e)
        {
            var draggedItem = DragDropPayloadManager.GetDataFromObject(e.Data, "DraggedItem");
            var details = DragDropPayloadManager.GetDataFromObject(e.Data, "DropDetails") as DropIndicationDetails;

            if (details == null || draggedItem == null)
            {
                return;
            }

            if (e.Effects == DragDropEffects.Move || e.Effects == DragDropEffects.All)
            {
                ((sender as RadGridView).ItemsSource as IList).Remove(draggedItem);
            }

            if (e.Effects != DragDropEffects.None)
            {
                var collection = (sender as RadGridView).ItemsSource as IList;
                int index = details.DropIndex < 0 ? 0 : details.DropIndex;
                index = details.DropIndex > collection.Count - 1 ? collection.Count : index;

                collection.Insert(index, draggedItem);
            }

            this.HideDropPositionFeedbackPresenter();
        }

        private void OnRowDragOver(object sender, Telerik.Windows.DragDrop.DragEventArgs e)
        {
            var row = sender as GridViewRow;
            var details = DragDropPayloadManager.GetDataFromObject(e.Data, "DropDetails") as DropIndicationDetails;

            if (details == null || row == null)
            {
                return;
            }

            details.CurrentDraggedOverItem = row.DataContext;

            if (details.CurrentDraggedItem == details.CurrentDraggedOverItem)
            {
                e.Effects = DragDropEffects.None;
                e.Handled = true;
                return;
            }

            details.CurrentDropPosition = this.GetDropPositionFromPoint(e.GetPosition(row), row);
            int dropIndex = (this.AssociatedObject.Items as IList).IndexOf(row.DataContext);
            int draggedItemIdex =
                (this.AssociatedObject.Items as IList).IndexOf(
                    DragDropPayloadManager.GetDataFromObject(e.Data, "DraggedItem"));

            if (dropIndex >= row.GridViewDataControl.Items.Count - 1
                && details.CurrentDropPosition == DropPosition.After)
            {
                details.DropIndex = dropIndex;
                this.ShowDropPositionFeedbackPresenter(this.AssociatedObject, row, details.CurrentDropPosition);
                return;
            }

            dropIndex = draggedItemIdex > dropIndex ? dropIndex : dropIndex - 1;
            details.DropIndex = details.CurrentDropPosition == DropPosition.Before ? dropIndex : dropIndex + 1;

            this.ShowDropPositionFeedbackPresenter(this.AssociatedObject, row, details.CurrentDropPosition);
        }

        private bool IsDropPositionFeedbackAvailable()
        {
            return
                  this.dropPositionFeedbackPresenterHost != null &&
                  this.dropPositionFeedbackPresenter != null;
        }

        private void ShowDropPositionFeedbackPresenter(
            GridViewDataControl gridView,
            GridViewRow row,
            DropPosition lastRowDropPosition)
        {
            if (!this.IsDropPositionFeedbackAvailable())
            {
                return;
            }

            var verticalOffset = this.GetDropPositionFeedbackOffset(row, lastRowDropPosition);
            this.dropPositionFeedbackPresenter.Visibility = Visibility.Visible;
            this.dropPositionFeedbackPresenter.Width = row.ActualWidth;
            this.dropPositionFeedbackPresenter.RenderTransform = new TranslateTransform { Y = verticalOffset };
        }

        private void HideDropPositionFeedbackPresenter()
        {
            this.dropPositionFeedbackPresenter.RenderTransform = new TranslateTransform
            {
                X = 0,
                Y = 0
            };
            this.dropPositionFeedbackPresenter.Visibility = Visibility.Collapsed;
        }

        private double GetDropPositionFeedbackOffset(GridViewRow row, DropPosition dropPosition)
        {
            var verticalOffset =
                row.TransformToVisual(this.dropPositionFeedbackPresenterHost).Transform(new Point(0, 0)).Y;
            if (dropPosition == DropPosition.After)
            {
                verticalOffset += row.ActualHeight;
            }

            verticalOffset -= this.dropPositionFeedbackPresenter.ActualHeight / 2.0;
            return verticalOffset;
        }

        private void DetachDropPositionFeedback()
        {
            if (this.IsDropPositionFeedbackAvailable())
            {
                this.dropPositionFeedbackPresenterHost.Children.Remove(this.dropPositionFeedbackPresenter);
                this.dropPositionFeedbackPresenter = null;
            }
        }

        private void AttachDropPositionFeedback()
        {
            this.dropPositionFeedbackPresenterHost = this.AssociatedObject.ParentOfType<Grid>();

            if (this.dropPositionFeedbackPresenterHost != null)
            {
                this.dropPositionFeedbackPresenter.Content = CreateDefaultDropPositionFeedback();
                if (this.dropPositionFeedbackPresenterHost != null
                    && this.dropPositionFeedbackPresenterHost.FindName(this.dropPositionFeedbackPresenter.Name) == null)
                {
                    this.dropPositionFeedbackPresenterHost.Children.Add(this.dropPositionFeedbackPresenter);
                }
            }

            this.HideDropPositionFeedbackPresenter();
        }

        /// <summary>
        /// The drop indication details.
        /// </summary>
        public class DropIndicationDetails : ViewModelBase
        {
            private object currentDraggedItem;
            private DropPosition currentDropPosition;
            private object currentDraggedOverItem;

            /// <summary>
            /// Gets or sets the current dragged over item.
            /// </summary>
            public object CurrentDraggedOverItem
            {
                get
                {
                    return this.currentDraggedOverItem;
                }

                set
                {
                    if (this.currentDraggedOverItem != value)
                    {
                        this.currentDraggedOverItem = value;
                        this.OnPropertyChanged("CurrentDraggedOverItem");
                    }
                }
            }

            /// <summary>
            /// Gets or sets the drop index.
            /// </summary>
            public int DropIndex { get; set; }

            /// <summary>
            /// Gets or sets the current drop position.
            /// </summary>
            public DropPosition CurrentDropPosition
            {
                get
                {
                    return this.currentDropPosition;
                }

                set
                {
                    if (this.currentDropPosition != value)
                    {
                        this.currentDropPosition = value;
                        this.OnPropertyChanged("CurrentDropPosition");
                    }
                }
            }

            /// <summary>
            /// Gets or sets the current dragged item.
            /// </summary>
            public object CurrentDraggedItem
            {
                get
                {
                    return this.currentDraggedItem;
                }

                set
                {
                    if (this.currentDraggedItem != value)
                    {
                        this.currentDraggedItem = value;
                        this.OnPropertyChanged("CurrentDraggedItem");
                    }
                }
            }
        }
    }
}
