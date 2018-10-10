// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InteractionAction.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the InteractionAction type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Framework.Interaction
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Interactivity;
    using System.Windows.Markup;

    using Gorba.Center.Common.Wpf.Framework.Helpers;
    using Gorba.Center.Common.Wpf.Framework.Notifications;

    /// <summary>
    /// Defines an action to be executed on interaction.
    /// </summary>
    public class InteractionAction : TriggerAction<Panel>
    {
        /// <summary>
        /// The dialog to be displayed.
        /// </summary>
        public static readonly DependencyProperty DialogProperty = DependencyProperty.Register(
            "Dialog",
            typeof(InteractionDialogBase),
            typeof(InteractionAction),
            new PropertyMetadata(null));

        /// <summary>
        /// The dialog width
        /// </summary>
        public static readonly DependencyProperty WidthProperty = DependencyProperty.Register(
            "Width",
            typeof(double),
            typeof(InteractionAction),
            new UIPropertyMetadata(double.NaN));

        /// <summary>
        /// The dialog height
        /// </summary>
        public static readonly DependencyProperty HeightProperty = DependencyProperty.Register(
            "Height",
            typeof(double),
            typeof(InteractionAction),
            new UIPropertyMetadata(double.NaN));

        /// <summary>
        /// the content template
        /// </summary>
        public static readonly DependencyProperty ContentTemplateProperty =
            DependencyProperty.Register(
                "ContentTemplate",
                typeof(DataTemplate),
                typeof(InteractionAction),
                new PropertyMetadata(null));

        /// <summary>
        /// determines if the popup is modal
        /// </summary>
        public static readonly DependencyProperty ModalProperty = DependencyProperty.Register(
            "Modal",
            typeof(bool),
            typeof(InteractionAction),
            new UIPropertyMetadata(true));

        /// <summary>
        /// Determines if the popup is toggled by the opening control.
        /// </summary>
        public static readonly DependencyProperty IsToggleProperty = DependencyProperty.Register(
            "IsToggle", typeof(bool), typeof(InteractionAction), new PropertyMetadata(default(bool)));

        /// <summary>
        /// The source name of the control that toggles the popup.
        /// </summary>
        public static readonly DependencyProperty ToggleSourceNameProperty =
            DependencyProperty.Register(
                "ToggleSourceName",
                typeof(string),
                typeof(InteractionAction),
                new PropertyMetadata(default(string)));

        /// <summary>
        /// The horizontal alignment of the popup that is to be triggered
        /// </summary>
        public static readonly DependencyProperty HorizontalAlignmentProperty =
            DependencyProperty.Register(
                "HorizontalAlignment",
                typeof(HorizontalAlignment),
                typeof(InteractionAction),
                new PropertyMetadata(default(HorizontalAlignment)));

        /// <summary>
        /// The vertical alignment of the popup that is to be triggered
        /// </summary>
        public static readonly DependencyProperty VerticalAlignmentProperty =
            DependencyProperty.Register(
                "VerticalAlignment",
                typeof(VerticalAlignment),
                typeof(InteractionAction),
                new PropertyMetadata(default(VerticalAlignment)));

        /// <summary>
        /// The left position of the popup that is to be triggered (in case it's container is a canvas)
        /// </summary>
        public static readonly DependencyProperty CanvasLeftProperty = DependencyProperty.Register(
            "CanvasLeft",
            typeof(double?),
            typeof(InteractionAction),
            new PropertyMetadata(null));

        /// <summary>
        /// The right position of the popup that is to be triggered (in case it's container is a canvas)
        /// </summary>
        public static readonly DependencyProperty CanvasRightProperty = DependencyProperty.Register(
            "CanvasRight",
            typeof(double?),
            typeof(InteractionAction),
            new PropertyMetadata(null));

        /// <summary>
        /// The top position of the popup that is to be triggered (in case it's container is a canvas)
        /// </summary>
        public static readonly DependencyProperty CanvasTopProperty = DependencyProperty.Register(
            "CanvasTop",
            typeof(double?),
            typeof(InteractionAction),
            new PropertyMetadata(null));

        /// <summary>
        /// The bottom position of the popup that is to be triggered (in case it's container is a canvas)
        /// </summary>
        public static readonly DependencyProperty CanvasBottomProperty = DependencyProperty.Register(
            "CanvasBottom",
            typeof(double?),
            typeof(InteractionAction),
            new PropertyMetadata(null));

        /// <summary>
        /// the tag property
        /// </summary>
        public static readonly DependencyProperty TagProperty = DependencyProperty.Register(
            "Tag",
            typeof(object),
            typeof(InteractionAction),
            new PropertyMetadata(default(object)));

        /// <summary>
        /// the parent property
        /// </summary>
        public static readonly DependencyProperty ParentProperty = DependencyProperty.Register(
            "Parent",
            typeof(UIElement),
            typeof(InteractionAction),
            new PropertyMetadata(default(UIElement)));

        private static int openDialogCounter;

        private ContentControl popup;

        private List<InteractionDialogBase> children;

        private InteractionDialogBase mouseDownDialog;

        private bool mouseWasDown;

        /// <summary>
        /// Gets a value indicating whether a modal dialog is open
        /// </summary>
        public static bool HasOpenModalDialog
        {
            get
            {
                return openDialogCounter > 0;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to skip the next mouse up
        /// </summary>
        public static bool SkipNextMouseUp { get; set; }

        /// <summary>
        /// Gets or sets the toggling source control name.
        /// </summary>
        public string ToggleSourceName
        {
            get
            {
                return (string)this.GetValue(ToggleSourceNameProperty);
            }

            set
            {
                this.SetValue(ToggleSourceNameProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the popup is toggled.
        /// </summary>
        public bool IsToggle
        {
            get
            {
                return (bool)this.GetValue(IsToggleProperty);
            }

            set
            {
                this.SetValue(IsToggleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the dialog.
        /// </summary>
        /// <value>
        /// The dialog.
        /// </value>
        public InteractionDialogBase Dialog
        {
            get { return (InteractionDialogBase)this.GetValue(DialogProperty); }
            set { this.SetValue(DialogProperty, value); }
        }

        /// <summary>
        /// Gets or sets the dialog width
        /// </summary>
        public double Width
        {
            get { return (double)this.GetValue(WidthProperty); }
            set { this.SetValue(WidthProperty, value); }
        }

        /// <summary>
        /// Gets or sets the horizontal alignment of the dialog
        /// </summary>
        public HorizontalAlignment HorizontalAlignment
        {
            get
            {
                return (HorizontalAlignment)this.GetValue(HorizontalAlignmentProperty);
            }

            set
            {
                this.SetValue(HorizontalAlignmentProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the vertical alignment of the dialog
        /// </summary>
        public VerticalAlignment VerticalAlignment
        {
            get
            {
                return (VerticalAlignment)this.GetValue(VerticalAlignmentProperty);
            }

            set
            {
                this.SetValue(VerticalAlignmentProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the left position of the dialog (in case it's parent is a canvas)
        /// </summary>
        public double? CanvasLeft
        {
            get
            {
                return (double?)this.GetValue(CanvasLeftProperty);
            }

            set
            {
                this.SetValue(CanvasLeftProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the right position of the dialog (in case it's parent is a canvas)
        /// </summary>
        public double? CanvasRight
        {
            get
            {
                return (double?)this.GetValue(CanvasRightProperty);
            }

            set
            {
                this.SetValue(CanvasRightProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the top position of the dialog (in case it's parent is a canvas)
        /// </summary>
        public double? CanvasTop
        {
            get
            {
                return (double?)this.GetValue(CanvasTopProperty);
            }

            set
            {
                this.SetValue(CanvasTopProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the bottom position of the dialog (in case it's parent is a canvas)
        /// </summary>
        public double? CanvasBottom
        {
            get
            {
                return (double?)this.GetValue(CanvasBottomProperty);
            }

            set
            {
                this.SetValue(CanvasBottomProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the dialog height
        /// </summary>
        public double Height
        {
            get { return (double)this.GetValue(HeightProperty); }
            set { this.SetValue(HeightProperty, value); }
        }

        /// <summary>
        /// Gets or sets the Content Template
        /// </summary>
        public DataTemplate ContentTemplate
        {
            get { return (DataTemplate)this.GetValue(ContentTemplateProperty); }
            set { this.SetValue(ContentTemplateProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the Popup is modal.
        /// </summary>
        public bool Modal
        {
            get { return (bool)this.GetValue(ModalProperty); }
            set { this.SetValue(ModalProperty, value); }
        }

        /// <summary>
        /// Gets or sets the Tag for the Dialog
        /// </summary>
        public object Tag
        {
            get
            {
                return this.GetValue(TagProperty);
            }

            set
            {
                this.SetValue(TagProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the Dialog Parent
        /// </summary>
        public UIElement Parent
        {
            get { return (UIElement)this.GetValue(ParentProperty); }

            set { this.SetValue(ParentProperty, value); }
        }

        /// <summary>
        /// Closes the Dialog
        /// </summary>
        public void Close()
        {
            this.Dialog.Close();
        }

        /// <summary>
        /// Invokes the specified parameter.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        protected override void Invoke(object parameter)
        {
            var args = parameter as InteractionRequestEventArgs;
            if (args == null)
            {
                throw new ArgumentOutOfRangeException(
                    "parameter", @"The parameter must be a valid InteractionRequestEventArgs");
            }

            this.SetDialog(args.Entity, args.Callback);
        }

        private void SetDialog(Notification notification, Action callback)
        {
            if (this.IsToggle && this.popup != null && this.Dialog.DataContext != null
                && this.Dialog.DataContext.GetType() == notification.GetType())
            {
                this.Dialog.Close();
                return;
            }

            var popupInstance = this.GetPopup(notification);

            openDialogCounter++;

            EventHandler handler = null;
            handler = (s, e) =>
            {
                this.Dialog.Closed -= handler;
                this.AssociatedObject.Children.Remove(this.popup);

                if (popupInstance.Content != null)
                {
                    ((Grid)popupInstance.Content).Children.Clear();
                    popupInstance.Content = null;
                }

                var dataContext = this.Dialog.DataContext as PromptNotification;
                if (dataContext != null)
                {
                    dataContext.IsOpen = false;
                }

                callback();

                this.popup = null;
                this.children = new List<InteractionDialogBase>();

                openDialogCounter--;
            };

            this.Dialog.Closed += handler;

            if (this.Modal)
            {
                var window = Window.GetWindow(this.AssociatedObject);
                if (window != null)
                {
                    Mouse.AddPreviewMouseDownHandler(window, this.OnMouseDown);
                    Mouse.AddPreviewMouseUpHandler(window, this.OnMouseUp);
                }
            }

            notification.PropertyChanged += (sender, e) =>
                {
                    if (e.PropertyName == "IsAcknowledged")
                    {
                        if (notification.IsAcknowledged)
                        {
                            this.Dialog.Close();
                        }
                    }
                };
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            // workaround for FileDialog: on double click a MouseUp is passed to the control behind the dialog
            this.mouseWasDown = true;

            if (object.ReferenceEquals(e.Source, this.Dialog))
            {
                this.mouseDownDialog = this.Dialog;
                return;
            }

            var pos = e.GetPosition(this.Dialog);
            if (!(pos.X < 0)
                && !(pos.X >= this.Dialog.ActualWidth)
                && !(pos.Y < 0)
                && !(pos.Y >= this.Dialog.ActualHeight))
            {
                this.mouseDownDialog = this.Dialog;
            }
        }

        [SuppressMessage("StyleCopPlus.StyleCopPlusRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        private void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            // workaround for FileDialog: on double click a MouseUp is passed to the control behind the dialog
            if (!this.mouseWasDown)
            {
                return;
            }

            this.mouseWasDown = false;

            if (object.ReferenceEquals(this.mouseDownDialog, this.Dialog))
            {
                this.mouseDownDialog = null;
                return;
            }

            if (object.ReferenceEquals(e.Source, this.Dialog))
            {
                return;
            }

            var prompt = this.Dialog.DataContext as PromptNotification;
            if (prompt != null && prompt.SuppressMouseEvents)
            {
                return;
            }

            if (this.IsToggle && !string.IsNullOrEmpty(this.ToggleSourceName))
            {
                var source = e.OriginalSource as Control;
                if (source != null && source.Name == this.ToggleSourceName)
                {
                    var toggleWindow = Window.GetWindow(this.AssociatedObject);
                    if (toggleWindow != null)
                    {
                        Mouse.RemovePreviewMouseUpHandler(toggleWindow, this.OnMouseUp);
                    }

                    return;
                }
            }

            var pos = e.GetPosition(this.Dialog);
            if (!(pos.X < 0)
                && !(pos.X >= this.Dialog.ActualWidth)
                && !(pos.Y < 0)
                && !(pos.Y >= this.Dialog.ActualHeight))
            {
                return;
            }

            if (this.children == null || this.children.Count == 0)
            {
                this.children = ChildControlFinder.FindChilds<InteractionDialogBase>(this.Dialog);
            }

            if (this.children != null)
            {
                foreach (var child in this.children)
                {
                    var childDataContext = child.DataContext as PromptNotification;
                    if (childDataContext != null && childDataContext.IsOpen)
                    {
                        return;
                    }
                }
            }

            if (SkipNextMouseUp)
            {
                SkipNextMouseUp = false;
                return;
            }

            var window = Window.GetWindow(this.AssociatedObject);
            if (window != null)
            {
                Mouse.RemovePreviewMouseDownHandler(window, this.OnMouseDown);
                Mouse.RemovePreviewMouseUpHandler(window, this.OnMouseUp);
            }

            this.Dialog.Close();
        }

        private ContentControl GetPopup(Notification notification)
        {
            if (this.popup == null)
            {
                if (this.Dialog == null)
                {
                    this.Dialog = new InteractionDialogBase();
                }

                this.Dialog.DataContext = notification;
                if (this.ContentTemplate != null)
                {
                    this.Dialog.MessageTemplate = this.ContentTemplate;
                }

                var grid = new Grid();
                grid.Children.Add(this.Dialog);
                this.popup = new ContentControl
                {
                    Content = grid,
                    Width = this.Width,
                    Height = this.Height,
                    HorizontalAlignment = this.HorizontalAlignment,
                    VerticalAlignment = this.VerticalAlignment,
                };

                if (this.Parent != null)
                {
                    ((IAddChild)this.Parent).AddChild(this.popup);
                }
                else
                {
                    this.AssociatedObject.Children.Add(this.popup);
                }

                this.Dialog.Tag = this.Tag;

                if (this.CanvasLeft.HasValue)
                {
                    Canvas.SetLeft(this.popup, this.CanvasLeft.Value);
                }

                if (this.CanvasRight.HasValue)
                {
                    Canvas.SetRight(this.popup, this.CanvasRight.Value);
                }

                if (this.CanvasTop.HasValue)
                {
                    Canvas.SetTop(this.popup, this.CanvasTop.Value);
                }

                if (this.CanvasBottom.HasValue)
                {
                    Canvas.SetBottom(this.popup, this.CanvasBottom.Value);
                }
            }

            return this.popup;
        }
    }
}