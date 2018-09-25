// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateOnDateChangedBehavior.cs" company="">
//   Copyright (c) 2013
//   Luminator Technology Group
//   All Rights Reserved
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Luminator.UIFramework.ResourceLibrary.Behaviors
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Data;
    using System.Windows.Input;
    using System.Windows.Interactivity;

    using Luminator.UIFramework.ResourceLibrary.Utils;

    /// <summary>
    /// The update on date changed behavior.
    /// </summary>
    public class UpdateOnDateChangedBehavior : Behavior<DatePicker>
    {
        private Calendar calendar;
        #region Methods

        /// <summary>
        ///     The on attached.
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();
            this.SetWatermark();
            this.AssociatedObject.SelectedDateChanged += this.AssociatedObjectOnSelectedDateChanged;
           // this.AssociatedObject.CalendarOpened += this.AssociatedObject_CalendarOpened;
            
        }

        void AssociatedObject_CalendarOpened(object sender, System.Windows.RoutedEventArgs e)
        {
            this.HideBackButton();
        }

        private void HideBackButton()
        {
            if (this.AssociatedObject != null)
            {
                var popup = this.AssociatedObject.Template.FindName("PART_Popup", this.AssociatedObject) as Popup;
                if (popup != null && popup.Child is Calendar)
                {
                    // ((Calendar)popup.Child).DisplayMode = CalendarMode.Decade;
                    this.calendar = ((Calendar)popup.Child) as Calendar;
                    var calendarItem = this.calendar.Template.FindName("PART_CalendarItem", this.calendar) as CalendarItem;
                    if (calendarItem != null)
                    {
                        var backButton = calendarItem.Template.FindName("PART_PreviousButton", calendarItem) as Button;
                        if (backButton != null)
                        {
                            backButton.Click += this.backButton_Click;
                        }
                    }
                }
            }
        }

        void backButton_Click(object sender, RoutedEventArgs e)
        {
            this.SetWatermark();
        }

        /// <summary>
        ///     The on detaching.
        /// </summary>
        protected override void OnDetaching()
        {
            base.OnDetaching();

            this.AssociatedObject.SelectedDateChanged -= this.AssociatedObjectOnSelectedDateChanged;
            this.AssociatedObject.CalendarOpened += this.AssociatedObject_CalendarOpened;
        }

        /// <summary>
        /// The associated object on selected date changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="selectionChangedEventArgs">
        /// The selection changed event args.
        /// </param>
        private void AssociatedObjectOnSelectedDateChanged(object sender, SelectionChangedEventArgs selectionChangedEventArgs)
        {
           // this.SetWatermark();
            BindingExpression binding = this.AssociatedObject.GetBindingExpression(DatePicker.SelectedDateProperty);
            if (binding != null)
            {
                binding.UpdateSource();
            }
            Mouse.Capture(null);
        }

        /// <summary>
        /// The set watermark.
        /// </summary>
        private void SetWatermark()
        {
            if (this.AssociatedObject != null)
            {
                var datePickerTextBox = WpfUtils.GetChildOfType<DatePickerTextBox>(this.AssociatedObject);
                if (datePickerTextBox != null)
                {
                    var wm = datePickerTextBox.Template.FindName("PART_Watermark", datePickerTextBox) as ContentControl;
                    if (wm != null && (this.AssociatedObject.SelectedDate == null || this.AssociatedObject.SelectedDate.HasValue && this.AssociatedObject.SelectedDate <  DateTime.Now.ToUniversalTime()))
                    {
                        this.AssociatedObject.SelectedDate = null;
                        wm.Content = "Immediate!";
                    }
                }
            }
        }

        #endregion
    }
}